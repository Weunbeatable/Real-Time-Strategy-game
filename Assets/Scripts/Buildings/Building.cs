using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : NetworkBehaviour

{
    //Spawning is done by client dragging building through UI
    // so these calls need to be communicated to the server. 
    // Server should store list of unit placements for everyone
    // to ensure proper building placement. We cant just trust client. 
    [SerializeField] private GameObject buildingPreview = null;
    [SerializeField] private Sprite icon = null;
    [SerializeField] private int id = -1;
    [SerializeField] private int price = 100;

    //Buildings related to server
    public static event Action<Building> ServerOnBuildingSpawned;
    public static event Action<Building> ServerOnBuildingDeSpawned;

    //storing buildings on client
    public static event Action<Building> AuthorityOnBuildingSpawned;
    public static event Action<Building> AuthorityOnBuildingDeSpawned;

    // we want some getters to have access to icons for UI and to check ID
    public GameObject GetBuildingPreview() => buildingPreview;// return using lambda expression
    public Sprite GetIcon()
    {
        return icon;
    }

    public int GetId()
    {
        return id;
    }

    public int GetPrice()
    {
        return price;
    }

    #region Server


    public override void OnStartServer()
    {
        ServerOnBuildingSpawned?.Invoke(this);
    }

    public override void OnStopServer()
    {
        ServerOnBuildingDeSpawned?.Invoke(this);
    }

    #endregion

    #region Client
    public override void OnStartAuthority()
    {
        // if we're server or don't have authority return
        AuthorityOnBuildingSpawned?.Invoke(this);
    }

    public override void OnStopClient()
    {
        if (!isClientOnly || !isOwned) { return; }
        AuthorityOnBuildingDeSpawned?.Invoke(this);
    }
   /* [Client]
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
    }*/
    #endregion
}
