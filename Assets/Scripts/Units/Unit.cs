using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Unit : NetworkBehaviour
{
    [SerializeField] private UnitMovement unitMovement = null;
    [SerializeField] private UnityEvent onSelected = null;
    [SerializeField] private UnityEvent onDeselected = null;

    // so we can check when a unit is spawned that we're adding it to our list of units
    public static event Action<Unit> ServerOnUnitSpawned;
    public static event Action<Unit> ServerOnUnitDeSpawned;

    public static event Action<Unit> AuthorityOnUnitSpawned;
    public static event Action<Unit> AuthorityOnUnitDeSpawned;

    public UnitMovement GetUnitMovements()
    {
        return unitMovement;
    }

    #region Server

    // called on server when units spawned
    public override void OnStartServer()
    {
        //raising event of unit spawning (only on server)
        ServerOnUnitDeSpawned?.Invoke(this);
    }

    //called on server when units despwned
    public override void OnStopServer()
    {
        ServerOnUnitDeSpawned?.Invoke(this);
    }
    /// <summary>
    /// We want client to store a list of its own units so clients
    /// know which units it can multi select, attack with etc
    /// This should be for the client method and not server
    /// This action should not take place on server, if we do we'll end
    /// up with duplicates.
    /// </summary>
    #endregion
    #region Client 

    public override void OnStartClient()
    {
        // if we're server or don't have authority return
        if (!isClientOnly || !isOwned) { return; }
        AuthorityOnUnitSpawned?.Invoke(this);
    }

    public override void OnStopClient()
    {
        if (!isClientOnly || !isOwned) { return; }
        AuthorityOnUnitDeSpawned?.Invoke(this);
    }
    [Client]
    public void Select()
    {
        if (!isOwned) { return; }
        onSelected?.Invoke();
    }
    [Client]
    public void Deselect()
    {
        if (!isOwned) { return; }
        onDeselected?.Invoke();
    }
    #endregion

}
