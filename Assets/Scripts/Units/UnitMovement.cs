using Mirror;
using UnityEngine;
using UnityEngine.AI;

public class UnitMovement : NetworkBehaviour
{
    [SerializeField] private NavMeshAgent agent = null;

   

    #region Server
    [Command]
    public void CmdMove(Vector3 position)
    {
        //Note !NavMesh instead of NavMesh, wanted to check if not clicking not if clicking
        if(!NavMesh.SamplePosition(position, out NavMeshHit hit, 1f, NavMesh.AllAreas)) { return; } // out means were passing in some data and its passing some out in return


        agent.SetDestination(hit.position);
    }
    #endregion

}
