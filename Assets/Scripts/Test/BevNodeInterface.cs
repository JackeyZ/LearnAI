using System.Collections;

public enum RunStatus
{
    Completed,
    Failure,
    Running,
}

/// <summary>
/// 基础节点借口
/// </summary>
public interface IBehaviourTreeNode
{
    RunStatus status { get; set; }
    string nodeName { get; set; }
    bool Enter(object input);
    bool Leave(object input);
    bool Tick(object input, object output);
    IBehaviourTreeNode parent { get; set; }
    IBehaviourTreeNode Clone();
}

/************************************************************************/
/* 组合节点                                                             */
/************************************************************************/
public interface ICompositeNode : IBehaviourTreeNode
{
    /// <summary>
    /// 增加节点
    /// </summary>
    /// <param name="node"></param>
    void AddNode(IBehaviourTreeNode node);

    /// <summary>
    /// 移除节点
    /// </summary>
    /// <param name="node"></param>
    void RemoveNode(IBehaviourTreeNode node);

    /// <summary>
    /// 是否有节点
    /// </summary>
    /// <param name="node"></param>
    /// <returns></returns>
    bool HasNode(IBehaviourTreeNode node);

    /// <summary>
    /// 增加条件节点
    /// </summary>
    /// <param name="node"></param>
    void AddCondition(IConditionNode node);

    /// <summary>
    /// 移除条件节点
    /// </summary>
    /// <param name="node"></param>
    void RemoveCondition(IConditionNode node);

    /// <summary>
    /// 是否有条件节点
    /// </summary>
    /// <param name="node"></param>
    /// <returns></returns>
    bool HasCondition(IConditionNode node);

    ArrayList nodeList { get; }
    ArrayList conditionList { get; }
}

/************************************************************************/
/* 选择节点                                                             */
/************************************************************************/
public interface ISelectorNode : ICompositeNode
{

}

/************************************************************************/
/*顺序节点                                                              */
/************************************************************************/
public interface ISequenceNode : ICompositeNode
{

}

/************************************************************************/
/* 平行(并列)节点                                                             */
/************************************************************************/
public interface IParallelNode : ICompositeNode
{

}

//////////////////////////////////////////////////////////////////////////

/************************************************************************/
/* 装饰结点                                                             */
/************************************************************************/
public interface IDecoratorNode : IBehaviourTreeNode
{

}

/************************************************************************/
/* 条件节点                                                             */
/************************************************************************/
public interface IConditionNode
{
    string nodeName { get; set; }
    bool ExternalCondition();
}

/************************************************************************/
/* 行为节点                                                             */
/************************************************************************/
public interface IActionNode : IBehaviourTreeNode
{

}

public interface IBehaviourTree
{

}
