using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ResourceDisplay : MonoBehaviour
{
    [SerializeField] TMP_Text resourcesText;

    private RTSPlayer player;

    private void Update()
    {
        if (player == null)
        {
            player = NetworkClient.connection.identity.GetComponent<RTSPlayer>();
        }

        if(player!= null)
        {
            ClientHandleResourceUpdated(player.GetResources());

            player.ClientOnResourcesUpdated += ClientHandleResourceUpdated;
        }
    }

    private void OnDestroy()
    {
        player.ClientOnResourcesUpdated -= ClientHandleResourceUpdated;
    }

    private void ClientHandleResourceUpdated(int resources)
    {
        resourcesText.text = $"Resources: { resources}"; 
    }
}
