using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitProjectile : NetworkBehaviour
{
    [SerializeField] private Rigidbody rb = null;
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
    [Server] // so we can destroy the object over time
    private void DestroySelf()
    {
        NetworkServer.Destroy(gameObject);
    }
}
