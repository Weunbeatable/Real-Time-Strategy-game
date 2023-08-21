using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : NetworkBehaviour
{
    [SerializeField] private int maxHealth = 100;

    [SyncVar(hook = nameof(HandleHalthUpdated))] // when new health is recieved, we'll send it over to client
    private int currentHealth; // only the server should change this, and client should update

    public event Action ServerOnDie;
    public event Action<int, int> ClientOnHealthUpdated; // Client sided, we'll pass current and max health so UI can display accurate health

    #region Server

    public override void OnStartServer() // will set health to max
    {
        base.OnStartServer();

        currentHealth = maxHealth;
        //dealing ddamage is server based and client should update UI
    }
    [Server]
    //Will take damage, listen for current health, if above 0 you're fine if 0 you die, if less than 0
    // set to 0 then die, upon death trigget an event so listenrs can then play their respective response.
    public void DealDamage(int damageAmount)
    {
        if(currentHealth == 0) { return; }
        currentHealth = Mathf.Max(currentHealth - damageAmount, 0);  // will set to whatever is bigger out of these 2 values;

        if(currentHealth != 0) { return; }

        ServerOnDie?.Invoke();

        Debug.Log("dang you died");
    }

    #endregion

    #region Client

    private void HandleHalthUpdated(int oldhealth, int newhealth)
    {
        // will check for health updating. 
        // This script is nice because we can modify this without caring about health display.
        ClientOnHealthUpdated?.Invoke(oldhealth, maxHealth);
    }

    #endregion
}
