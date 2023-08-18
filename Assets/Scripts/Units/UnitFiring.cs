using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitFiring : NetworkBehaviour
{
    // we want to check our target
    [SerializeField] private Targeter targeter = null;
    [SerializeField] private GameObject projectilePrefab = null;
    // want a place to spawn projectiles from so its not just the middle;
    [SerializeField] private Transform projectileSpawnPoint = null;
    [SerializeField] private float fireRange = 5f;
    [SerializeField] private float fireRate = 1f;
    [SerializeField] private float rotationSpeed = 20f; // range of target so we can rotate to properly face target;

    private float lastFireTime;

        [ServerCallback] // will help to call every frame on server and not just on cclient, and to avoid logging errors on due to running things on client we'll use a server callback
        private void Update()
    {
        //cache targeter reference
        Targetable target = targeter.GetTarget();
        // don't bother firing if theres no target
        if(target == null) { return; }
        // want to check distance from target and when we can shoot again
        if (!CanFireAtTarger()) { return; }

        // figuiring out where we want to aim and rotating between current and target rotation to face target
        Quaternion targetRotation =
            Quaternion.LookRotation(target.transform.position - transform.position);

        transform.rotation = Quaternion.RotateTowards(
            transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);// time.delta time will help for running on different framerates

        // time.time will let us know how long our game has been running
        if(Time.time > (1/ fireRate) +lastFireTime)
        {
            // angle to fire at
            Quaternion projectileRotation = Quaternion.LookRotation(
               target.GetAimAtPoint().position - projectileSpawnPoint.position);// looking from where target is to where we should be aiming at.

            GameObject projectileInstance = Instantiate(projectilePrefab, projectileSpawnPoint.position, projectileRotation);

            //want to spawn on network
            NetworkServer.Spawn(projectileInstance, connectionToClient); // connection to client is whoever owns firing script 
            // can now fire
            lastFireTime = Time.time;
        }
    }

    [Server]// if we get a warning about calling on client then something wrong was done, this call is just a check.
    private bool CanFireAtTarger()
    {
        return (targeter.GetTarget().transform.position - transform.position).sqrMagnitude
            <= fireRange * fireRange; // <= so we don't fire if out of range
    }
}
