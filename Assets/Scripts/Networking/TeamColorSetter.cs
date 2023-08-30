using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamColorSetter : NetworkBehaviour
{
    [SerializeField] private Renderer[] colorRenderers = new Renderer[0];

    //We want to sync the colors RPC happens too late
    [SyncVar(hook = nameof(HandleTeamColorUpdated))]
    private Color teamcColor = new Color();
    #region Server

    public override void OnStartServer()
    {
        //finding which player is which
        RTSPlayer player = connectionToClient.identity.GetComponent<RTSPlayer>();

        //assigning team color
        teamcColor = player.GetTeamColor();

    }

    #endregion

    #region Client
    //rendering color
    private void HandleTeamColorUpdated(Color oldColor, Color newColor)
    {
        //apply new color
        foreach (Renderer renderer in colorRenderers)
        {
            renderer.material.SetColor("_BaseColor", newColor);
        }
    }
    #endregion
}
