using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class UnitMovement : NetworkBehaviour
{
    [SerializeField] private NavMeshAgent agent = null;

    private Camera mainCamera;

    #region Server
    [Command]
    private void CmdMove(Vector3 position)
    {
        //Note !NavMesh instead of NavMesh, wanted to check if not clicking not if clicking
        if(!NavMesh.SamplePosition(position, out NavMeshHit hit, 1f, NavMesh.AllAreas)) { return; } // out means were passing in some data and its passing some out in return


        agent.SetDestination(hit.position);
    }
    #endregion

    #region Client

    public override void OnStartAuthority()
    {
        mainCamera = Camera.main;
       
    }

    [ClientCallback]
    // update is called on client and server, we only want it to run on our client so we need an attribute
    // ClientCallback
    private void Update() 
    {
        if (!isOwned) { return; } // if we don't own the object or

        if (!Mouse.current.rightButton.wasPressedThisFrame) { return; } // if we don't press anything, don't do anything. 
        Debug.Log("process recieved");
      Ray ray =  mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue()); // grab cursor pos

       if(!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity)) { return; } // if you click out of the clicabkle area i.e. didn't hit anything, then return.

        CmdMove(hit.point);
    }

    #endregion
}
