using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UnitSpawner : NetworkBehaviour ,IPointerClickHandler
{
    //When a base dies we want reference to it and trigger some action
    [SerializeField] private Health health = null;
    [SerializeField] private GameObject unitPrefab = null;
    [SerializeField] private Transform unitSpawnLocation;



    #region Server

    //When object is alive
    public override void OnStartServer()
    {
        health.ServerOnDie += ServerHandleOnDie;
    }

    //When object is !alive handle what happens when we die on server
    public override void OnStopServer()
    {
        health.ServerOnDie -= ServerHandleOnDie;
    }

    [Server]
    private void ServerHandleOnDie()
    {
        NetworkServer.Destroy(gameObject);
    }

    [Command]
    private void CmdSpawnUnit()
    {
        GameObject unitInstance = Instantiate(
            unitPrefab,
            unitSpawnLocation.position,
            unitSpawnLocation.rotation);

        //To sapwn the object on the network we need to use mirrors custom network server code
        //Spawn an registered prefab (registered with network manager)
        // we also want to register it to a client and give the client ownership of the unit.
        NetworkServer.Spawn(unitInstance, connectionToClient);
    }
    #endregion

    #region Client
    // when we click on a gameobject we'll call the method. 
    public void OnPointerClick(PointerEventData eventData)
    {
      if(eventData.button != PointerEventData.InputButton.Left) { return; }
        // dont want to bother telling server to do something if its not our base i.e. we dont own it
        if (!isOwned) { return; }

        CmdSpawnUnit();
    }
    #endregion

}
