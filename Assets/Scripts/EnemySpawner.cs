using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class EnemySpawner : NetworkBehaviour {

    public GameObject enemy;
    public Transform[] enemySpawns;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if (!NetworkServer.active)
            return;

	    if(Input.GetKeyDown(KeyCode.N))
        {
            foreach(Transform t in enemySpawns)
            {
                GameObject e = (GameObject)Instantiate(enemy, t.position, t.rotation);
                NetworkServer.Spawn(e);
            }
        }
	}
}
