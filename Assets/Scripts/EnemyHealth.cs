using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class EnemyHealth : NetworkBehaviour {

    [SyncVar] float health = 100;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if (health <= 0)
            Destroy(this.gameObject);
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
    }
}
