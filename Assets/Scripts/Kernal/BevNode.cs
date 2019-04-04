using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BevRunningStatus
{
    None,
    Executing,      //运行中
    Succeed,        //运行成功
    Failure,        //运行失败
}

/// <summary>
/// 行为树基础节点
/// </summary>
public class BevNode {
    protected BevNode parentNode = null;
    protected Condition enterCondition = null;
    protected BevTree ownTree;
    public BevNode(BevTree ownTree, BevNode parentNode = null ,Condition enterCondition = null)
    {
        this.parentNode = parentNode;
        this.enterCondition = enterCondition;
        this.ownTree = ownTree;
    }
    
    public bool CanEnter()
    {
        return enterCondition == null ? true : enterCondition.Pass();
    }
    
    public void Tick()
    {
        if (CanEnter())
        {
            ownTree.CurNode = this;
            OnEnter();
        }
        else
        {
            parentNode.AcceptReturn(this, BevRunningStatus.Failure);
        }
    }
    /// <summary>
    /// 接受子节点返回
    /// </summary>
    public void AcceptReturn(BevNode childNode, BevRunningStatus status)
    {
        ownTree.CurNode = this;
        DoReturn(childNode, status);
    }
    /// <summary>
    /// 返回状态给父节点
    /// </summary>
    /// <param name="childNode"></param>
    /// <param name="status"></param>
    public void ReturnToParent(BevNode childNode, BevRunningStatus status)
    {
        OnExit();
        if(parentNode != null)
        {
            parentNode.AcceptReturn(childNode, status);
        }
    }

    protected IEnumerator DelayTickChild(BevNode childNode, float delayTime = -1)
    {
        if(delayTime == -1)
        {
            delayTime = ownTree.TickTime;
        }
        yield return new WaitForSeconds(delayTime);
        childNode.Tick();
    }

    public void SetEnterCondition(Condition enterCondition)
    {
        this.enterCondition = enterCondition;
    }
    /* 可重写接口 */

    /// <summary>
    /// 子节点返回内容之后调用，根据子节点状态返回状态到父节点
    /// </summary>
    /// <param name="childNode"></param>
    /// <param name="status"></param>
    public virtual void DoReturn(BevNode childNode, BevRunningStatus status)
    {

    }
    protected virtual void OnEnter()
    {

    }

    protected virtual void OnExit()
    {

    }

}

/// <summary>
/// 组合节点
/// </summary>
public class CompositeNode : BevNode
{
    protected List<BevNode> childNodeList;
    public CompositeNode(BevTree ownTree, BevNode parentNode = null) : base(ownTree, parentNode)
    {
        childNodeList = new List<BevNode>();
    }
    public void AddChild(BevNode bevNode)
    {
        childNodeList.Add(bevNode);
    }
    public void RemoveChild(BevNode bevNode)
    {
        childNodeList.Remove(bevNode);
    }
    public void ClearChindren()
    {
        childNodeList.Clear();
    }
}

/// <summary>
/// 行为树节点输入数据
/// </summary>
public class BevNodeInputParam
{
    Dictionary<string, object> _ParamDic;

    public BevNodeInputParam(Dictionary<string, object> inputParames)
    {
        _ParamDic = inputParames;
    }

    public object GetParam(string key)
    {
        return _ParamDic[key];
    }
}
public class BevNodeOutputParam
{
    Dictionary<string, object> _ParamDic;

    public BevNodeOutputParam(Dictionary<string, object> inputParames)
    {
        _ParamDic = inputParames;
    }

    public object GetParam(string key)
    {
        return _ParamDic[key];
    }
}


/// <summary>
/// 选择节点
/// </summary>
public class SelectorNode : CompositeNode
{
    List<int> nodeWeightList = new List<int>();
    public SelectorNode(BevTree ownTree, BevNode parentNode = null) : base(ownTree, parentNode)
    {
    }
    protected override void OnEnter()
    {
        if (childNodeList != null && childNodeList.Count > 0)
        {
            childNodeList[0].Tick();
        }
    }

    /// <summary>
    /// 接受子节点返回
    /// </summary>
    public override void DoReturn(BevNode childNode, BevRunningStatus status)
    {
        if (status == BevRunningStatus.Succeed)
        {
            ReturnToParent(this, BevRunningStatus.Succeed);
        }
        else if (status == BevRunningStatus.Failure)
        {
            int curIndex = childNodeList.IndexOf(childNode);
            if (curIndex == childNodeList.Count - 1)
            {
                ReturnToParent(this, BevRunningStatus.Failure);
            }
            else
            {
                childNodeList[curIndex + 1].Tick();
            }
        }
        else if (status == BevRunningStatus.Executing)
        {
            DelayTickChild(childNode, ownTree.TickTime);
        }
    }
}
/// <summary>
/// 次序节点
/// </summary>
public class SequenceNode : CompositeNode
{
    public SequenceNode(BevTree ownTree, BevNode parentNode = null) : base(ownTree, parentNode)
    {

    }

    protected override void OnEnter()
    {
        if (childNodeList != null && childNodeList.Count > 0)
        {
            childNodeList[0].Tick();
        }
    }

    /// <summary>
    /// 接受子节点返回
    /// </summary>
    public override void DoReturn(BevNode childNode, BevRunningStatus status)
    {
        if(status == BevRunningStatus.Failure)
        {
            ReturnToParent(this, BevRunningStatus.Failure);
        }
        else if (status == BevRunningStatus.Succeed)
        {
            int curIndex = childNodeList.IndexOf(childNode);
            if (curIndex == childNodeList.Count - 1)
            {
                ReturnToParent(this, BevRunningStatus.Succeed);
            }
            else
            {
                childNodeList[curIndex + 1].Tick();
            }
        }
        else if (status == BevRunningStatus.Executing)
        {
            DelayTickChild(childNode);
        }
    }
}
/// <summary>
/// 并行节点(全部动作完成才返回)
/// </summary>
public class ParallelAndNode : CompositeNode
{
    private List<BevRunningStatus> chindNodeStatusList = new List<BevRunningStatus>();
    public ParallelAndNode(BevTree ownTree, BevNode parentNode = null) : base(ownTree, parentNode)
    {

    }

    protected override void OnEnter()
    {
        foreach (var item in childNodeList)
        {
            item.Tick();
        }
    }

    /// <summary>
    /// 接受子节点返回
    /// </summary>
    public override void DoReturn(BevNode childNode, BevRunningStatus status)
    {
        chindNodeStatusList[childNodeList.IndexOf(childNode)] = status;
        if (status == BevRunningStatus.Executing)
        {
            DelayTickChild(childNode);
        }
        else if(status == BevRunningStatus.Failure)
        {
            ReturnToParent(this, BevRunningStatus.Failure);
        }
        else if(status == BevRunningStatus.Succeed)
        {
            for (int i = 0; i < childNodeList.Count; i++)
            {
                if (chindNodeStatusList[i] == BevRunningStatus.Failure)
                {
                    ReturnToParent(this, BevRunningStatus.Failure);
                }
            }
            ReturnToParent(this, BevRunningStatus.Succeed);
        }
    } 
}



/// <summary>
/// 行为节点
/// </summary>
public class ActionNode : BevNode
{
    private BevRunningStatus selfStatus = BevRunningStatus.None;
    private List<BevRunningStatus> chindNodeStatusList = new List<BevRunningStatus>();
    public ActionNode(BevTree ownTree, BevNode parentNode = null) : base(ownTree, parentNode)
    {
    }

    public void OnActionComplete(BevRunningStatus status)
    {
        selfStatus = status;
    }

    protected override void OnEnter()
    {
        if(selfStatus == BevRunningStatus.None)
        {
            selfStatus = BevRunningStatus.Executing;
            Action();
            ownTree.updateNodeEvnet += UpdateNode;
        }
        ReturnToParent(this, selfStatus); 
    }

    protected override void OnExit()
    {
        if(selfStatus != BevRunningStatus.Executing)
        {
            ownTree.updateNodeEvnet -= UpdateNode;
            selfStatus = BevRunningStatus.None;
        }
    }
    protected virtual void Action()
    {

    }
    public virtual void UpdateNode()
    {

    }
   

}