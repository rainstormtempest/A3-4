using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviorTree
{
    //��Ϊ״̬
    public enum State
    {
        INVALID,
        SUCCESS,
        FAILURE,
        RUNNING,
        ABORTED,
    };

    # region ��Ϊ�Ļ���--Ҷ�ӽ��,ֱ��ʹ�ã������Ͷ���
    public class Behavior
    {
        protected State m_Status;

        public Behavior()
        {
            m_Status = State.INVALID;
        }

        //��Ϊ�¼�
        public event Action onInitialize;
        public event Func<State> onUpdate;
        public event Action<State> onTerminate;

        public void AddEvent(Action Initialize, Func<State> Update, Action<State> Terminate)
        {
            if (Initialize != null)
                onInitialize += Initialize;
            if (Update != null)
                onUpdate += Update;
            if (Terminate != null)
                onTerminate += Terminate;
        }

        public State Tick()
        {
            if (m_Status != State.RUNNING && onInitialize != null)
                onInitialize();
            m_Status = onUpdate();
            if (m_Status != State.RUNNING && onTerminate != null)
                onTerminate(m_Status);
            return m_Status;
        }

        public virtual void Reset()
        {
            m_Status = State.INVALID;
        }

        public void Abort()
        {
            onTerminate(State.ABORTED);
            m_Status = State.ABORTED;
        }

        public bool IsTerminated()
        {
            return m_Status == State.SUCCESS || m_Status == State.FAILURE;
        }

        public bool IsRunning()
        {
            return m_Status == State.RUNNING;
        }

        public State GetStatus()
        {
            return m_Status;
        }
    }
    #endregion

    # region װ����,һ��ֻ��һ���ӽڵ����Ϊ����֧
    public class Decorator : Behavior
    {
        protected Behavior m_Child;

        public Decorator(Behavior child)
        {
            m_Child = child;
        }
    }
    //1.�ظ���Ϊ�ڵ�
    public class Repeat : Decorator
    {
        protected int m_Limit;//�ظ�����
        protected int m_Counter;//������

        public Repeat(Behavior child, int Limit) : base(child)
        {
            SetLimit(Limit);

            onInitialize += Initialize;
            onUpdate += Update;
        }

        public void SetLimit(int Limit) { m_Limit = Limit; }

        void Initialize()
        {
            m_Counter = 0;
        }

        State Update()
        {
            for (; ; )
            {
                m_Child.Tick();
                if (m_Child.GetStatus() == State.RUNNING) break;
                if (m_Child.GetStatus() == State.FAILURE) return State.FAILURE;
                if (++m_Counter == m_Limit) return State.SUCCESS;
                m_Child.Reset();
            }
            return State.INVALID;
        }
    }
    #endregion

    #region ������Ϊ,���ж���ӽڵ����Ϊ����֧
    public class Composite : Behavior
    {
        protected List<Behavior> m_Children;

        public Composite()
        {
            m_Children = new List<Behavior>();
        }

        public void AddChilds(params Behavior[] childs)
        {
            for (int i = 0; i < childs.Length; i++)
            {
                m_Children.Add(childs[i]);
            }
        }

        public int GetChildCount() { return m_Children.Count; }
    }
    //1.˳����(And-&):�������˳��ִ���ӽڵ���Ϊֱ�������ӽڵ�ȫ����ɻ��ߵ�ĳһ��ʧ��Ϊֹ
    public class Sequence : Composite
    {
        protected Behavior m_CurrentChild;

        protected int CurrentIndex;

        public Sequence()
        {
            onInitialize += Initialize;
            onUpdate += Update;
        }

        public virtual void Initialize()
        {
            CurrentIndex = 0;
            m_CurrentChild = m_Children[CurrentIndex];
        }

        public virtual State Update()
        {
            //Initialize();
            for (; ; )
            {
                State s = m_CurrentChild.Tick();

                if (s != State.SUCCESS)
                {
                    return s;
                }
                else
                {
                    if (++CurrentIndex == m_Children.Count)
                    {
                        return State.SUCCESS;
                    }
                    else
                    {
                        m_CurrentChild = m_Children[CurrentIndex];
                    }
                }
            }
        }
    }
    //2.ѡ����(Or-|):����ִ��ÿ������Ϊֱ��ĳ���ӽڵ��Ѿ��ɹ�ִ�л򷵻�RUNNING״̬
    public class Selector : Composite
    {
        protected Behavior m_CurrentChild;

        protected int CurrentIndex;

        public Selector()
        {
            onInitialize += Initialize;
            onUpdate += Update;
        }

        public virtual void Initialize()
        {
            CurrentIndex = 0;
            m_CurrentChild = m_Children[CurrentIndex];
        }

        public virtual State Update()
        {
            Initialize();
            for (; ; )
            {
                State s = m_CurrentChild.Tick();
                if (s != State.FAILURE)
                {
                    return s;
                }
                else
                {
                    if (++CurrentIndex == m_Children.Count)
                    {
                        return State.FAILURE;
                    }
                    else
                    {
                        m_CurrentChild = m_Children[CurrentIndex];
                    }
                }
            }
        }
    }
    //3.������:ͬʱִ�������ӽڵ㲢��ָ��������ִֹͣ��
    public class Parallel : Composite
    {
        protected int m_SuccessPolicyCount;
        protected int m_FailurePolicyCount;
        protected int m_SuccessCounter;
        protected int m_FailureCounter;

        public Parallel(int SuccessPolicyCount, int FailurePolicyCount)
        {
            m_FailurePolicyCount = FailurePolicyCount;
            m_SuccessPolicyCount = SuccessPolicyCount;

            onUpdate += Update;
            onTerminate += Terminate;
        }

        public virtual State Update()
        {
            for (int i = 0; i < m_Children.Count; i++)
            {
                if (!m_Children[i].IsTerminated())
                {
                    m_Children[i].Tick();
                }

                //���ȴ���ʧ�ܱȽϱ���
                if (m_Children[i].GetStatus() == State.FAILURE)
                {
                    ++m_FailureCounter;
                    if (m_FailureCounter >= m_FailurePolicyCount)
                    {
                        return State.FAILURE;
                    }
                }

                if (m_Children[i].GetStatus() == State.SUCCESS)
                {
                    ++m_SuccessCounter;
                    if (m_SuccessCounter >= m_SuccessPolicyCount)
                    {
                        return State.SUCCESS;
                    }
                }

            }

            return State.RUNNING;
        }

        public virtual void Terminate(State status) //��ֹ���������е��ӽڵ�
        {
            for (int i = 0; i < m_Children.Count; i++)
            {
                Behavior b = m_Children[i];
                if (b.IsRunning())
                {
                    b.Abort();
                }
            }
        }
    }
    #endregion

    #region ������Ϊ,Ϊ��Ϊ�����������ⲿ����
    //1.������--�����������¾ܾ�ִ������Ϊ����Ϊ����֧
    public class Filter : Sequence
    {
        public void AddCondition(Behavior condition)
        {
            m_Children.Insert(0, condition);
        }

        public void AddAction(Behavior action)
        {
            m_Children.Add(action);
        }
    }
    //2.������--����ǰ���������ܽ���
    public class Monitor : Parallel
    {
        public Monitor(int SuccessPolicyCount, int FailurePolicyCount) : base(SuccessPolicyCount, FailurePolicyCount)
        {
        }

        public void AddCondition(Behavior condition)
        {
            m_Children.Insert(0, condition);
        }

        public void AddAction(Behavior action)
        {
            m_Children.Add(action);
        }
    }
    #endregion


    //������
    public class Driver
    {
        //ִ����Ϊ��
        public void Running(Behavior root)
        {
            //������ǰ�ڵ�(��Ϊ)
            root.Tick();
        }
    }
}

