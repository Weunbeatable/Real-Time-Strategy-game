using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RTSPlayer : NetworkBehaviour
{
    // we dont need a list as our building array wont chagne in size when game starts
    [SerializeField] private Building[] buildings = new Building[0];
    public event Action<int> ClientOnResourcesUpdated; // 

    //Place to store our resources
    // syncVar to hook and sync value changing, when value is synced we'll call upudate method
    [SyncVar(hook = nameof(ClientHandleResourcesUpdated))]
    private int resources = 500;

    //list of our units
   [SerializeField] private List<Unit> myUnits = new List<Unit>();
    private List<Building> myBuildings = new List<Building>();

    #region Server

    public int GetResources() => resources;
    public List<Unit> getMyUnits()
    {
        return myUnits;
    }

    public List<Building> GetBuildings()
    {
        return myBuildings;
    }

    [Server]
    public void SetResources(int newResources)
    {
        resources = newResources;
    }
    public override void OnStartServer()
    {
        //subscription to events 
        Unit.ServerOnUnitSpawned += ServerHandleUnitsSpawned;
        Unit.ServerOnUnitDeSpawned += ServerHandleUnitsDeSpawned;
        Building.ServerOnBuildingSpawned += ServerHandleBuildingsSpawned;
        Building.ServerOnBuildingDeSpawned += ServerHandleBuildingsDeSpawned;
    }

    public override void OnStopServer()
    {
        //unsubscribing from events
        Unit.ServerOnUnitSpawned -= ServerHandleUnitsSpawned;
        Unit.ServerOnUnitDeSpawned -= ServerHandleUnitsDeSpawned;
        Building.ServerOnBuildingSpawned -= ServerHandleBuildingsSpawned;
        Building.ServerOnBuildingDeSpawned -= ServerHandleBuildingsDeSpawned;
    }

    [Command]
    // Can't send over building but we can send its ID, and position
    public void CmdTryPlaceBuilding(int buildingId, Vector3 position)
    {
        //We need to figure out which building using a list
        Building buildingToPlace = null;

        foreach (Building building in buildings)
        {
            //loop over all buildings and check for a matchign ID
                if(building.GetId() == buildingId)
            {
                buildingToPlace = building;
                break; //once we find the right building we can leave the loop 
            }
        }
        // In case we get an invalid ID just leave
        if(buildingToPlace == null) { return; }

        //now we can spawn on server
       GameObject buildingInstance  =  
            Instantiate(buildingToPlace.gameObject, position, buildingToPlace.transform.rotation);

        // spawn on network
        NetworkServer.Spawn(buildingInstance, connectionToClient);
    }

    //Whoever host is will have list filled up. 
    private void ServerHandleBuildingsDeSpawned(Building building)
    {
        if (building.connectionToClient.connectionId != connectionToClient.connectionId) { return; }

        myBuildings.Add(building);
    }

    private void ServerHandleBuildingsSpawned(Building building)
    {
        if (building.connectionToClient.connectionId != connectionToClient.connectionId) { return; }

        myBuildings.Remove(building);
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

    private void ClientHandleResourcesUpdated(int oldResources, int newResources)
    {
        ClientOnResourcesUpdated?.Invoke(newResources);
    }
    public override void OnStartAuthority()
    {
        //Check if machine is running as a server
        if (NetworkServer.active || !isOwned) { return; }
        Unit.AuthorityOnUnitSpawned += AuthorityHandleUnitsSpawned;
        Unit.AuthorityOnUnitDeSpawned += AuthorityHandleUnitsDeSpawned;
        Building.AuthorityOnBuildingSpawned += AuthorityHandleBuildingssSpawned;
        Building.AuthorityOnBuildingSpawned += AuthorityHandleBuildingssDeSpawned;
    }
    public override void OnStopClient()
    {
        if (!isClientOnly) { return; }
        Unit.AuthorityOnUnitSpawned -= AuthorityHandleUnitsSpawned;
        Unit.AuthorityOnUnitDeSpawned -= AuthorityHandleUnitsDeSpawned;
        Building.AuthorityOnBuildingSpawned -= AuthorityHandleBuildingssSpawned;
        Building.AuthorityOnBuildingSpawned -= AuthorityHandleBuildingssDeSpawned;
    }
    private void AuthorityHandleUnitsSpawned(Unit unit)
    {
        myUnits.Add(unit); // don't need if check because we can't even see it anyways
    }
    private void AuthorityHandleUnitsDeSpawned(Unit unit)
    {
        myUnits.Remove(unit);
    }

    private void AuthorityHandleBuildingssSpawned(Building building)
    {
        myBuildings.Add(building); // don't need if check because we can't even see it anyways
    }
    private void AuthorityHandleBuildingssDeSpawned(Building building)
    {
        myBuildings.Remove(building);
    }
    #endregion
}
