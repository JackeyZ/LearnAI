using System.Collections;
using UnityEngine;

public class BaseNode
{
    public BaseNode() { nodeName_ = this.GetType().Name + "\n"; }

    protected RunStatus status_ = RunStatus.Completed;      //节点状态
    protected string nodeName_;                             //节点名称
    protected IBehaviourTreeNode parent_;                   //父节点

    public virtual RunStatus status { get { return status_; } set { status_ = value; } }
    public virtual string nodeName { get { return nodeName_; } set { nodeName_ = value; } }
    public virtual IBehaviourTreeNode parent { get { return parent_; } set { parent_ = value; } }
    public virtual IBehaviourTreeNode Clone()
    {
        var clone = new BaseNode();
        clone.status_ = status_;
        clone.nodeName_ = nodeName_;
        clone.parent_ = parent_;
        return clone as IBehaviourTreeNode;
    }
}

public class BaseActionNode : IActionNode
{
    public BaseActionNode() { nodeName_ = this.GetType().Name + "\n"; }
    protected RunStatus status_ = RunStatus.Completed;
    protected string nodeName_;
    protected IBehaviourTreeNode parent_;
    public virtual RunStatus status { get { return status_; } set { status_ = value; } }
    public virtual string nodeName { get { return nodeName_; } set { nodeName_ = value; } }
    public virtual IBehaviourTreeNode parent { get { return parent_; } set { parent_ = value; } }
    public virtual IBehaviourTreeNode Clone()
    {
        var clone = new BaseActionNode();
        clone.status_ = status_;
        clone.nodeName_ = nodeName_;
        clone.parent_ = parent_;
        return clone as IBehaviourTreeNode;
    }

    public virtual bool Enter(object input)
    {
        status_ = RunStatus.Running;
        return true;
    }

    public virtual bool Leave(object input)
    {
        status_ = RunStatus.Completed;
        return true;
    }

    public virtual bool Tick(object input, object output)
    {
        return true;
    }
}
public class BaseCondictionNode
{
    protected string nodeName_;
    public virtual string nodeName { get { return nodeName_; } set { nodeName_ = value; } }
    public BaseCondictionNode() { nodeName_ = this.GetType().Name + "\n"; }
    public delegate bool ExternalFunc();
    protected ExternalFunc externalFunc;
    public static ExternalFunc GetExternalFunc(BaseCondictionNode node)
    {
        return node.externalFunc;
    }
}

public class Precondition : BaseCondictionNode, IConditionNode
{
    public Precondition(ExternalFunc func) { externalFunc = func; }
    public Precondition(BaseCondictionNode pre) { externalFunc = BaseCondictionNode.GetExternalFunc(pre); }
    public bool ExternalCondition()
    {
        if (externalFunc != null) return externalFunc();
        else return false;
    }
}

public class PreconditionNOT : BaseCondictionNode, IConditionNode
{
    public PreconditionNOT(ExternalFunc func) { externalFunc = func; }
    public PreconditionNOT(BaseCondictionNode pre) { externalFunc = BaseCondictionNode.GetExternalFunc(pre); }
    public bool ExternalCondition()
    {
        if (externalFunc != null) return !externalFunc();
        else return false;
    }
}

public class BaseCompositeNode : BaseNode
{
    protected ArrayList nodeList_ = new ArrayList();
    protected ArrayList conditionList_ = new ArrayList();
    protected int runningNodeIndex = 0;
    protected bool CheckNodeAndCondition()
    {
        if (nodeList_.Count == 0)
        {
            status_ = RunStatus.Failure;
            Debug.Log("SequenceNode has no node!");
            return false;
        }
        return CheckCondition();
    }
    protected bool CheckCondition()
    {
        foreach (var node in conditionList_)
        {
            var condiction = node as IConditionNode;
            if (!condiction.ExternalCondition())
                return false;
        }
        return true;
    }
    public virtual void AddNode(IBehaviourTreeNode node) { node.parent = (IBehaviourTreeNode)this; nodeList_.Add(node); }
    public virtual void RemoveNode(IBehaviourTreeNode node) { nodeList_.Remove(node); }
    public virtual bool HasNode(IBehaviourTreeNode node) { return nodeList_.Contains(node); }

    public virtual void AddCondition(IConditionNode node) { conditionList_.Add(node); }
    public virtual void RemoveCondition(IConditionNode node) { conditionList_.Remove(node); }
    public virtual bool HasCondition(IConditionNode node) { return conditionList_.Contains(node); }

    public virtual ArrayList nodeList { get { return nodeList_; } }
    public virtual ArrayList conditionList { get { return conditionList_; } }

    public override IBehaviourTreeNode Clone()
    {
        var clone = base.Clone() as BaseCompositeNode;
        clone.nodeList_.AddRange(nodeList_);
        clone.conditionList_.AddRange(conditionList_);
        clone.runningNodeIndex = runningNodeIndex;
        return clone as IBehaviourTreeNode;
    }
}
