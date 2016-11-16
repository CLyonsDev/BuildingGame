using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class WaveManager : NetworkBehaviour {

    [SyncVar] public int waveNumber = 1;
    [SyncVar] public bool waveOngoing = false;
    [SyncVar] public bool treesSpawned = false;
    public int zombiesPerWaveBase = 10;

    [SyncVar] public int setDifficulty = 1;

    public float buildTimeBase = 120;
    [SyncVar] public float buildTimeRemaining;

    public GameObject enemy;
    public GameObject[] enemySpawns;

    public GameObject[] enemiesAlive, playersInGame;
    public List<GameObject> playersAlive = new List<GameObject>();

    bool spawnedWarning = false;
    [SyncVar] bool gameOver = false;

    // Use this for initialization
    void Start () {
        buildTimeRemaining = buildTimeBase;
        enemySpawns = GameObject.FindGameObjectsWithTag("EnemySpawn");
	}
	
	// Update is called once per frame
	void Update () {
	    //We need to cycle between spawning waves and build time.
        if(!waveOngoing)
        {
            //We are building. Count down.
            buildTimeRemaining -= Time.deltaTime;

            if (buildTimeRemaining <= 20f)
            {
                if (waveNumber == 1 && !spawnedWarning)
                {
                    spawnedWarning = true;
                    GetComponent<PopupManager>().SpawnHintBox("20 seconds to build. Trees have begun to spawn!", 3f);
                }

                if(!treesSpawned)
                {
                    treesSpawned = true;
                    GetComponent<ResourceManager>().SpawnTrees();
                }
            }

            if (buildTimeRemaining <= 0)
            {
                //We are out of build time!
                buildTimeRemaining = buildTimeBase;
                if (NetworkServer.active)
                    CmdSpawnWave(waveNumber, setDifficulty); //Spawn our enemies
                waveOngoing = true;
            }
        }else
        {
            //The wave is currently ongoing.

            enemiesAlive = GameObject.FindGameObjectsWithTag("Enemy");

            if (enemiesAlive.Length <= 0)
            {
                //All of the enemies are dead.
                treesSpawned = false;
                waveOngoing = false;

                //Time to build!
                waveNumber++; //Time to increment our wave number.

                if (!NetworkServer.active)
                    return;
                GameObject[] deadBricks = GameObject.FindGameObjectsWithTag("DeadBrick");
                foreach (GameObject go in deadBricks)
                {
                    NetworkServer.Destroy(go);
                }
            }

            if(!gameOver)
            {
                playersInGame = GameObject.FindGameObjectsWithTag("Player");
                playersAlive.Clear();
                foreach (GameObject go in playersInGame)
                {
                    if (!go.GetComponent<PlayerHealth_New>().isDead)
                        playersAlive.Add(go);
                }
            }

            if (playersAlive.Count <= 0 && !gameOver)
            {
                //All of our players have died!
                Debug.LogError("Game Over!");
                gameOver = true;
                FindObjectOfType<PopupManager>().SpawnHintBox("All players have died. Restarting game...", 3f);
                StartCoroutine(RestartLevel(3f));
            }
        }
	}

    [Command]
    void CmdSpawnWave(int waveNum, int difficulty)
    {
        int zombiesToSpawn = (zombiesPerWaveBase + (difficulty * 2)) * waveNum;
        for (int i = 0; i < zombiesToSpawn; i++)
        {
            Transform spawnPoint = enemySpawns[Random.Range(0, enemySpawns.Length)].transform;
            GameObject e = (GameObject)Instantiate(enemy, spawnPoint.position, spawnPoint.rotation);
            NetworkServer.Spawn(e);
        }
    }

    [Command]
    void CmdRestartLevel()
    {
        RpcRestartLevel();
    }

    [ClientRpc]
    void RpcRestartLevel()
    {
        NetworkManager.singleton.ServerChangeScene(NetworkManager.singleton.onlineScene);
    }

    IEnumerator RestartLevel(float delay)
    {
        if (!NetworkServer.active)
            yield return null;

        yield return new WaitForSeconds(delay);
        CmdRestartLevel();
    }
}
