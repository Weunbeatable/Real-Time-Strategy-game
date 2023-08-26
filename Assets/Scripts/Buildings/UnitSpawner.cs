using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UnitSpawner : NetworkBehaviour ,IPointerClickHandler
{
    //When a base dies we want reference to it and trigger some action
    [SerializeField] private Health health = null;
    [SerializeField] private Unit unitPrefab = null;
    [SerializeField] private Transform unitSpawnLocation = null;
    [SerializeField] private TMP_Text remainingUnitsText = null;
    [SerializeField] private Image unitProgressImage = null;
    [SerializeField] private int maxUnitQueue = 5;
    [SerializeField] private float spawnMoveRange = 7;
    [SerializeField] private float unitSpawnDuration = 5f;

    
    [SyncVar(hook = nameof(ClientHandleQueuedUnitsUpdated))]
    private int queuedUnits;

    [SyncVar]
    private float unitTimer;

    float progressImageVelocity;

    private void Update()
    {
        if (isServer)
        {
            //update units
            ProduceUnits();
        }

        if (isClient)
        {
            //ensure fill is accurate
            UpdateTimerDisplay();
        }
    }

    

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
    private void ProduceUnits()
    {
        if(queuedUnits == 0) { return; }

        unitTimer += Time.deltaTime;

        if(unitTimer < unitSpawnDuration) { return; }

        //there are some units and weve reached timer
        GameObject unitInstance = Instantiate(
         unitPrefab.gameObject,
         unitSpawnLocation.position,
         unitSpawnLocation.rotation);

        //To sapwn the object on the network we need to use mirrors custom network server code
        //Spawn an registered prefab (registered with network manager)
        // we also want to register it to a client and give the client ownership of the unit.
        NetworkServer.Spawn(unitInstance, connectionToClient);

        // a random vec 3 with size 1 and radius of whatever our range is
        Vector3 spawnOffset = UnityEngine.Random.insideUnitSphere *spawnMoveRange;
        // so it doesn't go up and down
        spawnOffset.y = unitSpawnLocation.position.y;

        UnitMovement unitMovement = unitInstance.GetComponent<UnitMovement>();
        unitMovement.ServerMove(unitSpawnLocation.position + spawnOffset);

        queuedUnits--;
        unitTimer = 0;
    }
    private void UpdateTimerDisplay()
    {
        // progress to fill up duration if all the way around snap to top
        float newProgress = unitTimer / unitSpawnDuration;

        if(newProgress < unitProgressImage.fillAmount)
        {
            unitProgressImage.fillAmount = newProgress;
        }

        else
        {
            // if it hasn't finished looping smoothly interpolate between values.
            unitProgressImage.fillAmount = Mathf.SmoothDamp(
                unitProgressImage.fillAmount,
                newProgress,
                ref progressImageVelocity,
                0.1f
                );
        }
    }

    [Server]
    private void ServerHandleOnDie()
    {
       NetworkServer.Destroy(gameObject);
    }

    [Command]
    private void CmdSpawnUnit()
    {
       // can't spawn anymore units
     if(queuedUnits == maxUnitQueue) { return; }

        //you have enough munnies?
        RTSPlayer player = connectionToClient.identity.GetComponent<RTSPlayer>();

        if(player.GetResources() < unitPrefab.GetRersourceCost()) { return; }

        queuedUnits++;

        player.SetResources(player.GetResources() - unitPrefab.GetRersourceCost());
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

    private void ClientHandleQueuedUnitsUpdated(int oldUnits, int newUnits)
    {
        // when clients are synced from updated server text will change.
       remainingUnitsText.text = newUnits.ToString();
    }
    #endregion

}
