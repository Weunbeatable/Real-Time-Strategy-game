using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RTSPlayer : NetworkBehaviour
{
    //list of our units
   [SerializeField] private List<Unit> myUnits = new List<Unit>();

    #region Server

    public List<Unit> getMyUnits()
    {
        return myUnits;
    }
    public override void OnStartServer()
    {
        //subscription to events 
        Unit.ServerOnUnitSpawned += ServerHandleUnitsSpawned;
        Unit.ServerOnUnitDeSpawned += ServerHandleUnitsDeSpawned;
    }

    public override void OnStopServer()
    {
        //unsubscribing from events
        Unit.ServerOnUnitSpawned -= ServerHandleUnitsSpawned;
        Unit.ServerOnUnitDeSpawned -= ServerHandleUnitsDeSpawned;
    }

    private void ServerHandleUnitsSpawned(Unit unit)
    {
        // are these the same people? if they are we'll add units
        if(unit.connectionToClient.connectionId != connectionToClient.connectionId) { return; }

        myUnits.Add(unit);
    }
    private void ServerHandleUnitsDeSpawned(Unit unit)
    {
        if (unit.connectionToClient.connectionId != connectionToClient.connectionId) { return; }

        myUnits.Remove(unit);
    }
    #endregion

    #region Client

    public override void OnStartClient()
    {
        if (!isClientOnly) { return; }
        Unit.AuthorityOnUnitSpawned += AuthorityHandleUnitsSpawned;
        Unit.AuthorityOnUnitDeSpawned += AuthorityHandleUnitsDeSpawned;
    }
    public override void OnStopClient()
    {
        if (!isClientOnly) { return; }
        Unit.AuthorityOnUnitSpawned -= AuthorityHandleUnitsSpawned;
        Unit.AuthorityOnUnitDeSpawned -= AuthorityHandleUnitsDeSpawned;
    }
    private void AuthorityHandleUnitsSpawned(Unit unit)
    {
        if (!isOwned) { return; }
        myUnits.Add(unit); // don't need if check because we can't even see it anyways
    }
    private void AuthorityHandleUnitsDeSpawned(Unit unit)
    {
        if (!isOwned) { return; }
        myUnits.Remove(unit);
    }
    #endregion
}
