using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Targeter : NetworkBehaviour
{
    /// <summary>
    /// This will be on all units
    /// we'll loop over selected units and get a point, we'll look at the targeter and set a target. 
    /// This data needs to be sent over the network. 
    /// </summary>
    // Start is called before the first frame update

   private Targetable target;

    public Targetable GetTarget() // an easy way to get the target for targeting
    {
        return target;
    }

    public override void OnStartServer()
    {
        GameOverHandler.ServerOnGameOver += ServerHandleGameOver;
    }

    public override void OnStopServer()
    {
        GameOverHandler.ServerOnGameOver -= ServerHandleGameOver;
    }

    [Command]
    public void CmdSetTarget(GameObject targetGameObject)
    {
        if (!targetGameObject.TryGetComponent<Targetable>(out Targetable newTarget)) { return; }
        target = newTarget;

    }
        [Server]
        public void ClearTarget()
        {
            target = null;
        }
    
 
    [Server]
    private void ServerHandleGameOver()
    {
        ClearTarget();
    }



}
