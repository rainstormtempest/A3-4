using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyBehaviorTree
{
    public enum NodeState //�ڵ�(��Ϊ)״̬
    {
        Null,
        Success,//�ɹ�
        Failure,//ʧ��
        Running,//������
        Abort,//��ֹ
    }

    public enum NodeType//�ڵ�(��Ϊ)����
    {
        Null,
        Condition,//�����ڵ�(��Ϊ)
        Action,//�����ڵ�(��Ϊ)
        And,//��ڵ�(��Ϊ)
        Or,//��ڵ�(��Ϊ)
        Parallel,//���нڵ�
    }

    public class Node //�ڵ�
    {
        private string m_Name;
        public string GetName() { return m_Name; }

        //���нڵ��������
        protected int SuccessParallelNum;
        protected int FailureParallelNum;
        public void SetParallelNum(int SuccessParallelNum, int FailureParallelNum)
        {
            this.SuccessParallelNum = SuccessParallelNum;
            this.FailureParallelNum = FailureParallelNum;
        }
        public int GetSuccessParallelNum() { return SuccessParallelNum; }
        public int GetFailureParallelNum() { return FailureParallelNum; }

        //�ڵ�(��Ϊ)״̬
        protected NodeState m_State = NodeState.Abort;
        public NodeState GetNodeState() { return m_State; }
        public void SetNodeState(NodeState state) { m_State = state; }

        //�ڵ�(��Ϊ)����
        protected NodeType m_NodeType;
        public NodeType GetNodeType() { return m_NodeType; }

        //�����ӽڵ�(��Ϊ)
        protected List<Node> m_ChildrenNodes;
        public List<Node> GetChildrenNodes() { return m_ChildrenNodes; }
        public virtual void AddChildNode(Node ChildNode)
        {
            if (m_ChildrenNodes == null)
                m_ChildrenNodes = new List<Node>();
            m_ChildrenNodes.Add(ChildNode);

        }
        public virtual void AddChildNodes(params Node[] ChildNodes)
        {
            if (m_ChildrenNodes == null)
                m_ChildrenNodes = new List<Node>();
            for (int i = 0; i < ChildNodes.Length; i++)
            {
                m_ChildrenNodes.Add(ChildNodes[i]);
            }

        }

        //�ڵ�(��Ϊ)������
        public Node(NodeType nodeType, string name)
        {
            m_NodeType = nodeType;
            m_Name = name;
        }

        //�ڵ�(��Ϊ)�¼�
        public event Action onEnter;
        public virtual void OnEnter() { if (onEnter != null) onEnter(); }
        public event Func<NodeState> onUpdate;
        public virtual NodeState OnUpdate() { return onUpdate(); }
        public event Action onExit;
        public virtual void OnExit() { if (onExit != null) onExit(); }
        public virtual void Reset() { }

        //��ӽڵ�(��Ϊ)�¼�
        public virtual void AddEvent(Action Enter, Func<NodeState> Update, Action Exit)
        {
            if (Enter != null)
                onEnter += Enter;
            if (Update != null)
                onUpdate += Update;
            if (Exit != null)
                onExit += Exit;
        }
    }


    public class Driver //������
    {
        //��ǰʱ������״̬�Ľڵ�(��Ϊ)ջ
        private List<Node> m_CurrentRunningNodes;
        //��һʱ������״̬�Ľڵ�(��Ϊ)ջ
        private List<Node> m_LastRunningNodes;

        //ִ����Ϊ��
        public void Running(Node root)
        {
            //��յ�ǰʱ������״̬�Ľڵ�(��Ϊ)ջ
            if (m_CurrentRunningNodes != null)
            {
                m_CurrentRunningNodes.Clear();
            }
            //������ǰ�ڵ�(��Ϊ)
            Tick(root);
            //�˳���һʱ���ڵ�ǰʱ�̵ķ����нڵ�(��Ϊ)
            ExitAbortNode();
            //��¼��ǰʱ������״̬�Ľڵ�(��Ϊ)ջ
            if (m_LastRunningNodes == null)
            {
                m_LastRunningNodes = new List<Node>();
            }
            m_LastRunningNodes = m_CurrentRunningNodes;
        }

        private void ExitAbortNode()
        {
            if (m_LastRunningNodes != null)
            {
                for (int i = 0; i < m_LastRunningNodes.Count; i++)
                {
                    if (!m_CurrentRunningNodes.Contains(m_LastRunningNodes[i]))
                    {
                        m_LastRunningNodes[i].OnExit();
                        m_LastRunningNodes[i].SetNodeState(NodeState.Abort);
                    }
                }
            }
        }

        private NodeState Tick(Node root) //���ķ���
        {
            if (root.GetNodeState() != NodeState.Running)
                root.OnEnter();
            NodeState state = Update(root);
            root.SetNodeState(state);
            if (state == NodeState.Running)
            {
                if (m_CurrentRunningNodes == null)
                    m_CurrentRunningNodes = new List<Node>();
                m_CurrentRunningNodes.Add(root);
            }
            return state;
        }

        private NodeState Update(Node node) //��Ϊִ�з���
        {
            switch (node.GetNodeType())
            {
                case NodeType.Condition:
                case NodeType.Action:
                    return node.OnUpdate();
                case NodeType.And: //�룺�ɹ��Ž�����һ��
                    List<Node> childs_And = node.GetChildrenNodes();
                    if (childs_And != null)
                    {
                        for (int i = 0; i < childs_And.Count; i++)
                        {
                            NodeState nodeState = Tick(childs_And[i]);
                            if (nodeState != NodeState.Success)
                            {
                                return nodeState;
                            }
                        }
                        return NodeState.Success;
                    }
                    else
                    {
                        Debug.LogError("ĳAnd�ڵ������ӽڵ�");
                        return NodeState.Null;
                    }
                case NodeType.Or://�򣺲�ʧ�ܾ�ִ��
                    List<Node> childs_Or = node.GetChildrenNodes();
                    if (childs_Or != null)
                    {
                        for (int i = 0; i < childs_Or.Count; i++)
                        {
                            NodeState nodeState = Tick(childs_Or[i]);
                            if (nodeState != NodeState.Failure)
                            {
                                Debug.Log(childs_Or[i].GetName());
                                return nodeState;
                            }
                        }
                        return NodeState.Failure;
                    }
                    else
                    {
                        Debug.LogError("ĳOr�ڵ������ӽڵ�");
                        return NodeState.Null;
                    }
                case NodeType.Parallel:
                    List<Node> childs_Parallel = node.GetChildrenNodes();
                    int m_FailureCounter = 0;
                    int m_SuccessCounter = 0;
                    for (int i = 0; i < childs_Parallel.Count; i++)
                    {
                        if (childs_Parallel[i].GetNodeState() != NodeState.Failure && childs_Parallel[i].GetNodeState() != NodeState.Success)
                        {
                            childs_Parallel[i].OnUpdate();
                        }

                        //���ȴ���ʧ�ܱȽϱ���
                        if (childs_Parallel[i].GetNodeState() == NodeState.Failure)
                        {
                            ++m_FailureCounter;
                            if (m_FailureCounter >= node.GetFailureParallelNum())
                            {
                                return NodeState.Failure;
                            }
                        }

                        if (childs_Parallel[i].GetNodeState() == NodeState.Success)
                        {
                            ++m_SuccessCounter;
                            if (m_SuccessCounter >= node.GetSuccessParallelNum())
                            {
                                return NodeState.Success;
                            }
                        }

                    }
                    return NodeState.Running;
                default:
                    Debug.LogError("�����ڸýڵ�����");
                    return NodeState.Null;
            }
        }

    }
}

