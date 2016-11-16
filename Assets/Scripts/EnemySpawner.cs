using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;

public class EnemySpawner : NetworkBehaviour {

    public GameObject enemy;
    public GameObject[] enemySpawns;

    // Use this for initialization
    void Start () {
        enemySpawns = GameObject.FindGameObjectsWithTag("EnemySpawn");
	}
	
	// Update is called once per frame
	void Update () {
        if (!NetworkServer.active)
            return;

	    if(Input.GetKeyDown(KeyCode.N))
        {
            foreach(GameObject t in enemySpawns)
            {
                GameObject e = (GameObject)Instantiate(enemy, t.transform.position, t.transform.rotation);
                NetworkServer.Spawn(e);
            }
        }
	}
}
