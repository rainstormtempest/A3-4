using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyBehaviorTree
{
    public enum NodeState //节点(行为)状态
    {
        Null,
        Success,//成功
        Failure,//失败
        Running,//进行中
        Abort,//终止
    }

    public enum NodeType//节点(行为)类型
    {
        Null,
        Condition,//条件节点(行为)
        Action,//动作节点(行为)
        And,//与节点(行为)
        Or,//或节点(行为)
        Parallel,//并行节点
    }

    public class Node //节点
    {
        private string m_Name;
        public string GetName() { return m_Name; }

        //并行节点结束条件
        protected int SuccessParallelNum;
        protected int FailureParallelNum;
        public void SetParallelNum(int SuccessParallelNum, int FailureParallelNum)
        {
            this.SuccessParallelNum = SuccessParallelNum;
            this.FailureParallelNum = FailureParallelNum;
        }
        public int GetSuccessParallelNum() { return SuccessParallelNum; }
        public int GetFailureParallelNum() { return FailureParallelNum; }

        //节点(行为)状态
        protected NodeState m_State = NodeState.Abort;
        public NodeState GetNodeState() { return m_State; }
        public void SetNodeState(NodeState state) { m_State = state; }

        //节点(行为)类型
        protected NodeType m_NodeType;
        public NodeType GetNodeType() { return m_NodeType; }

        //所有子节点(行为)
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

        //节点(行为)构造器
        public Node(NodeType nodeType, string name)
        {
            m_NodeType = nodeType;
            m_Name = name;
        }

        //节点(行为)事件
        public event Action onEnter;
        public virtual void OnEnter() { if (onEnter != null) onEnter(); }
        public event Func<NodeState> onUpdate;
        public virtual NodeState OnUpdate() { return onUpdate(); }
        public event Action onExit;
        public virtual void OnExit() { if (onExit != null) onExit(); }
        public virtual void Reset() { }

        //添加节点(行为)事件
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


    public class Driver //驱动器
    {
        //当前时刻运行状态的节点(行为)栈
        private List<Node> m_CurrentRunningNodes;
        //上一时刻运行状态的节点(行为)栈
        private List<Node> m_LastRunningNodes;

        //执行行为树
        public void Running(Node root)
        {
            //清空当前时刻运行状态的节点(行为)栈
            if (m_CurrentRunningNodes != null)
            {
                m_CurrentRunningNodes.Clear();
            }
            //驱动当前节点(行为)
            Tick(root);
            //退出上一时刻在当前时刻的非运行节点(行为)
            ExitAbortNode();
            //记录当前时刻运行状态的节点(行为)栈
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

        private NodeState Tick(Node root) //核心方法
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

        private NodeState Update(Node node) //行为执行方法
        {
            switch (node.GetNodeType())
            {
                case NodeType.Condition:
                case NodeType.Action:
                    return node.OnUpdate();
                case NodeType.And: //与：成功才进行下一步
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
                        Debug.LogError("某And节点下无子节点");
                        return NodeState.Null;
                    }
                case NodeType.Or://或：不失败就执行
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
                        Debug.LogError("某Or节点下无子节点");
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

                        //优先处理失败比较保险
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
                    Debug.LogError("不存在该节点类型");
                    return NodeState.Null;
            }
        }

    }
}

