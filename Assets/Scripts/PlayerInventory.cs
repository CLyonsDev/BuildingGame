using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class PlayerInventory : NetworkBehaviour {

    [SyncVar] public int wood = 0;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void ModifyWood(int amount)
    {
        wood += amount;
    }
}
