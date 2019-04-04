using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CapsuleTree : BevTree {

    // Use this for initialization
    protected override void Start ()
    {
        base.Start();
    }

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void InitTreeNode()
    {
        _NodeRoot = new SelectorNode(this);
        SelectorNode temp = _NodeRoot as SelectorNode;
        temp.AddChild(new MoveAction(this, temp));
        temp.AddChild(new MoveAction(this, temp));
        _CurNode = _NodeRoot;
    }
}
