using Mirror;
using UnityEngine;
using UnityEngine.AI;

public class UnitMovement : NetworkBehaviour
{
    [SerializeField] private NavMeshAgent agent = null;
    [SerializeField] private Targeter targeter = null;
    [SerializeField] private float chaseRange = 10f;



    #region Server


    public override void OnStartServer()
    {
        // when subscribing to the serveronGameover action we call our custom handlegame over
        GameOverHandler.ServerOnGameOver += HandleGameOver;
    }

    public override void OnStopServer()
    {
        // when subscribing to the serveronGameover action we call to stop our custom handlegame over
        GameOverHandler.ServerOnGameOver -= HandleGameOver;
    }

    [ServerCallback] // makes calls on client but wont give us warnings in console. 
    private void Update()
    {
        //chasing code
        if(targeter.GetTarget() != null)
        {
            Targetable target = targeter.GetTarget();// cachign reference to avoid constantly calling for this function
            if ((target.transform.position - transform.position).sqrMagnitude > chaseRange * chaseRange)//check if in range Square magnitutde instead of distance since its faster as it doesn't do sqr roots
            {
                agent.SetDestination(target.transform.position);//chase
            }
            else if (agent.hasPath) // to avoid calling reset every frame.
            {
                agent.ResetPath(); // stop chasing  
            }
            return; // if we have a target the code below is pretty irrelevant so we can return
        }
        //Logic for no target
        if (!agent.hasPath) { return; } // will prevent trying to clear agent path in the same frame as calculating it

        if(agent.remainingDistance > agent.stoppingDistance) { return; }

        agent.ResetPath();
    }

    [Command]
    public void CmdMove(Vector3 position)
    {
        targeter.ClearTarget();

        //Note !NavMesh instead of NavMesh, wanted to check if not clicking not if clicking
        if (!NavMesh.SamplePosition(position, out NavMeshHit hit, 1f, NavMesh.AllAreas)) { return; } // out means were passing in some data and its passing some out in return


        agent.SetDestination(hit.position);
    }

    [Server]
    private void HandleGameOver()
    {
        agent.ResetPath();
    }

    #endregion

}
