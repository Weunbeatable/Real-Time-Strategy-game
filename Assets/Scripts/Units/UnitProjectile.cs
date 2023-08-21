using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitProjectile : NetworkBehaviour
{
    [SerializeField] private Rigidbody rb = null;
    [SerializeField] private int damageToDeal = 20;
    [SerializeField] private float destroyAfterSeconds = 5f;
    [SerializeField] private float launchForce = 10f;
    // sicne they move in a striaght line we don't need to sync movement unlike tanks
    void Start()
    {
        rb.velocity = transform.forward * launchForce;
    }

    public override void OnStartServer()
    {
        Invoke(nameof(DestroySelf), destroyAfterSeconds); // method will destroy object on server after set time
    }
    [Server] // this should only be called on server client shouldn't worry about damage. just UI

    // when projectile enters something if it belongs to us return, so it phases through our own buildings.
    // if you hit someone else deal damage,
    // destroy self will call when hitting anything that doesn't belong to yourself like terrain, or enemy.
    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent<NetworkIdentity>(out NetworkIdentity networkIdentity))
        {
            // check if it belongs to us
            // if the thing we hit has a network identity, then we should check the connection to the client. 
            if(networkIdentity.connectionToClient == connectionToClient) { return; }
        }

        if(other.TryGetComponent<Health>(out Health health))
        {
            health.DealDamage(damageToDeal);
        }

        DestroySelf();
    }

    [Server] // so we can destroy the object over time
    private void DestroySelf()
    {
        NetworkServer.Destroy(gameObject);
    }
}
