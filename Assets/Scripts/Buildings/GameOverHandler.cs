using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverHandler : NetworkBehaviour
{
    // Server doesn't care about player UI so it doesn't have data passed in
    public static event Action ServerOnGameOver;

    // Want to show UI of player
    public static event Action<string> ClientOnGameOver; 

    //When a base is spawned we want to add it to a list and store it
    private List<UnitBase> bases = new List<UnitBase>();


    #region Server
    public override void OnStartServer()
    {
        UnitBase.ServerOnBaseSpawned += ServerHandleBaseSpawned;
        UnitBase.ServerOnBaseDeSpawned += ServerHandleBaseDeSpawned;
    }

    public override void OnStopServer()
    {
        UnitBase.ServerOnBaseSpawned -= ServerHandleBaseSpawned;
        UnitBase.ServerOnBaseDeSpawned -= ServerHandleBaseDeSpawned;
    }
    [Server]
    private void ServerHandleBaseSpawned(UnitBase unitBase)
    {
        bases.Add(unitBase);
    }
    [Server]
    private void ServerHandleBaseDeSpawned(UnitBase unitBase)
    {
        bases.Remove(unitBase);
        //lets check remaining
        if(bases.Count !=1) { return; }

        // Storing player name as int using their unique identifier
        int playerId = bases[0].connectionToClient.connectionId;

        //Otherwise end game
        // will say winner is player 1 or 2 etc
        RpcGameOver($"Player {playerId}");

        ServerOnGameOver?.Invoke();
    }

    #endregion

    #region Client
    //Server should tell client game is over
    [ClientRpc]
    private void RpcGameOver(string winner)
    {
        // when event is triggered we can show some UI saying games over
        ClientOnGameOver?.Invoke(winner);
    }

    #endregion
}
