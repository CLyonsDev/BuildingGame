using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class Tree : NetworkBehaviour {

    [SyncVar] public float health = 100;

    [SyncVar] public float resourcePerHealthPoint = 1; //A value of 1 would mean the tree gives off 100 pieces of wood.

    public void GetHarvested(float amount)
    {
        health -= amount;
        if(health <= 0)
        {
            Destroy(this.gameObject);
        }
    }
}
