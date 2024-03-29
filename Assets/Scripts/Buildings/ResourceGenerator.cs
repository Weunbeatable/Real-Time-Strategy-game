using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceGenerator : NetworkBehaviour
{
    [SerializeField] private Health health = null;

    // how many seconds before we get some money
    [SerializeField] private int resourcesPerInterval = 10;
    [SerializeField] private float interval = 2f;

    private float timer;
    private RTSPlayer player;

    public override void OnStartServer()
    {
        timer = interval;
        //since it doesn't exist before world we can get it normally
        player = connectionToClient.identity.GetComponent<RTSPlayer>();

        health.ServerOnDie += ServerHandleDie;
        GameOverHandler.ServerOnGameOver += ServerHandleGameOver;       
    }

    public override void OnStopServer()
    {
        health.ServerOnDie -= ServerHandleDie;
        GameOverHandler.ServerOnGameOver -= ServerHandleGameOver;
    }
    [ServerCallback]
    private void Update()
    {
        timer -= Time.deltaTime;

        if(timer <= 0) // ready to give reseources to player and restart timer
        {
            player.SetResources(player.GetResources() + resourcesPerInterval);

            timer += interval;
        }
    }
    private void ServerHandleDie()
    {
        NetworkServer.Destroy(gameObject);
    }
    private void ServerHandleGameOver()
    {
        enabled = false;
    }

}
