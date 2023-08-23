using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameOverDisplay : MonoBehaviour
{
    [SerializeField] private GameObject gamneOverDisplayParent = null;
    [SerializeField] private TMP_Text winnerNameText = null;
    private void Start()
    {
        GameOverHandler.ClientOnGameOver += ClientHandleGameOver;
    }

    private void OnDestroy()
    {
        GameOverHandler.ClientOnGameOver -= ClientHandleGameOver;
    }

    //Need a way to shut down server when done
    //will go back to host screen and all other clients will be kicked
    public void LeaveGame()
    {
        if(NetworkServer.active && NetworkClient.isConnected)
        {
            //stop hosting
            NetworkManager.singleton.StopHost();
        }
        else
        {
            //stop client
            NetworkManager.singleton.StopClient();
        }
    }
    private void ClientHandleGameOver(string winner)
    {
        // when gam eis over and told winner name
        winnerNameText.text = $"{winner} has won! ";

        gamneOverDisplayParent.SetActive(true);
    }

}
