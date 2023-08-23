using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitBase : NetworkBehaviour
{
    [SerializeField] private Health health;

    //server event ,whatever has health when a base dies its units diewith it
    public static event Action<int> ServerOnPlayerDie;

    //Want to know the base that gets spawned and the one thats despawned
    //We will add bases to a list so we can keep track of bases.
    public static event Action<UnitBase> ServerOnBaseSpawned;
    public static event Action<UnitBase> ServerOnBaseDeSpawned;



    #region Server

    public override void OnStartServer()
    {
        health.ServerOnDie += ServerHandleOnDie;

        ServerOnBaseDeSpawned?.Invoke(this);
    }

    public override void OnStopServer()
    {
        health.ServerOnDie -= ServerHandleOnDie;

        ServerOnBaseDeSpawned?.Invoke(this);
    }

    [Server]
    public void ServerHandleOnDie()
    {
        ServerOnPlayerDie?.Invoke(connectionToClient.connectionId);

        NetworkServer.Destroy(gameObject);
    }

    #endregion

    #region Client

    #endregion
}
