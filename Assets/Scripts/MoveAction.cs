using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MoveAction : ActionNode
{
    private GameObject ownObj;
    private NavMeshAgent navAgent;
    Vector3 targetPos;
    public MoveAction(BevTree ownTree, BevNode parentNode = null) : base(ownTree, parentNode)
    {
        ownObj = ownTree.gameObject;
        navAgent = ownObj.GetComponent<NavMeshAgent>();
    }

    protected override void Action()
    {
        targetPos = ownObj.transform.position + ownObj.transform.forward * 10;
        navAgent.SetDestination(targetPos);
        bool result = navAgent.CalculatePath(ownObj.transform.position + ownObj.transform.forward * 10, navAgent.path);
    }
    public override void UpdateNode()
    {
        if(Vector3.Distance(targetPos, ownObj.transform.position) < 0.2)
        {
            OnActionComplete(BevRunningStatus.Succeed);
        }
    }
    
}
