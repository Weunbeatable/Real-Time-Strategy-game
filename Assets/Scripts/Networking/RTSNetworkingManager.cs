using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RTSNetworkingManager : NetworkManager
{
    [SerializeField] private GameObject unitSpawnerPrefab = null;
    //want to spawn in gameoverhandler whenever new scene is a map. ONLY for maps
    [SerializeField] private GameOverHandler gameOverHandlerPrefab = null;
    // When a unit gets spawned in
    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        base.OnServerAddPlayer(conn);

        //add colors stored in player script
        RTSPlayer player = conn.identity.GetComponent<RTSPlayer>();
        player.SetTeamColor(new Color(
            UnityEngine.Random.Range(0f, 1f),
            UnityEngine.Random.Range(0f, 1f),
            UnityEngine.Random.Range(0f, 1f)
            ));

        //The prefab we want to spawn, its spawned and rotated where the player was spawned in. 
        GameObject unitSpawnerInstance = Instantiate(
            unitSpawnerPrefab, //want to spawn it in for everyone connected
              conn.identity.transform.position,
              conn.identity.transform.rotation);

        //difference between here and unit spawner is we want to spawn the unit spawner, 
        //the owner is the player where that function is above ^.
        //Clients will get spawned and ownership is given to the player. 
        NetworkServer.Spawn(unitSpawnerInstance, conn);
    }

    public override void OnServerSceneChanged(string sceneName)
    {
        // relevant for switching to maps
        // need to grab path in file
        if (SceneManager.GetActiveScene().name.StartsWith("Scene_Map"))
        {
            // game over handler 
            GameOverHandler gameOverHandlerInstance = Instantiate(gameOverHandlerPrefab);

            //network server spawning as an object
            NetworkServer.Spawn(gameOverHandlerInstance.gameObject);
        }
    }
}
