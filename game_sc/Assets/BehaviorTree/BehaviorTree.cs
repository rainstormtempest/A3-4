using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviorTree
{
    //行为状态
    public enum State
    {
        INVALID,
        SUCCESS,
        FAILURE,
        RUNNING,
        ABORTED,
    };

    # region 行为的基类--叶子结点,直接使用：条件和动作
    public class Behavior
    {
        protected State m_Status;

        public Behavior()
        {
            m_Status = State.INVALID;
        }

        //行为事件
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

    # region 装饰器,一个只有一个子节点的行为树分支
    public class Decorator : Behavior
    {
        protected Behavior m_Child;

        public Decorator(Behavior child)
        {
            m_Child = child;
        }
    }
    //1.重复行为节点
    public class Repeat : Decorator
    {
        protected int m_Limit;//重复次数
        protected int m_Counter;//计数器

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

    #region 复合行为,具有多个子节点的行为树分支
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
    //1.顺序器(And-&):按照设计顺序执行子节点行为直到所有子节点全部完成或者到某一个失败为止
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
    //2.选择器(Or-|):依次执行每个子行为直到某个子节点已经成功执行或返回RUNNING状态
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
    //3.并行器:同时执行所有子节点并在指定规则中停止执行
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

                //优先处理失败比较保险
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

        public virtual void Terminate(State status) //终止所有运行中的子节点
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

    #region 辅助行为,为行为树运行增加外部条件
    //1.过滤器--在特殊条件下拒绝执行子行为的行为树分支
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
    //2.监视器--满足前提条件才能进行
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


    //运行器
    public class Driver
    {
        //执行行为树
        public void Running(Behavior root)
        {
            //驱动当前节点(行为)
            root.Tick();
        }
    }
}

