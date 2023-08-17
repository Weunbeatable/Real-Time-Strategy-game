using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RTSNetworkingManager : NetworkManager
{
    [SerializeField] private GameObject unitSpawnerPrefab = null;
    // When a unit gets spawned in
    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        base.OnServerAddPlayer(conn);

        //The prefab we want to spawn, its spawned and rotated where the player was spawned in. 
        GameObject unitSpawnerInstance = Instantiate(
            unitSpawnerPrefab, //want to spawn it in for everyone connected
              conn.identity.transform.position,
              conn.identity.transform.rotation);

        //difference between here and unit spawner is we want to spawn the unit spawner, 
        //the owner is the player where that function is above ^.
        //Clients will get spawned and ownership is given to the player. 
        NetworkServer.Spawn(unitSpawnerInstance, conn);
    }
}
