using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BevTree : MonoBehaviour{
    protected BevNode _NodeRoot;
    protected BevNode _CurNode;
    protected float _TickTime = 0.1f;
    protected bool isInit = false;
    public delegate void UpdateDel();
    public event UpdateDel updateNodeEvnet;
    public BevNode CurNode
    {
        get
        {
            return _CurNode;
        }

        set
        {
            _CurNode = value;
        }
    }

    public float TickTime
    {
        get
        {
            return _TickTime;
        }

        set
        {
            _TickTime = value;
        }
    }

    protected virtual void Awake()
    {
        StartCoroutine("StartTree");
    }

    protected virtual void Start()
    {
        InitTreeNode();
        isInit = true;
    }

    protected virtual void Update()
    {
        if(updateNodeEvnet != null)
        {
            updateNodeEvnet();
        }
    }

    IEnumerator StartTree()
    {
        while(enabled)
        {
            if (isInit == true)
            {
                _CurNode.Tick();
                yield return new WaitForSeconds(TickTime);
            }
            yield return null;
        }
    }


    protected virtual void InitTreeNode()
    {

    }
}
