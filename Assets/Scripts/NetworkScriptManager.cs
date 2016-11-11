using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;
using UnityEngine.Networking;
using System.Collections;

public class NetworkScriptManager : NetworkBehaviour {

    public Camera[] cameras;

	void Start() {
        if(isLocalPlayer)
        {
            Debug.LogWarning("Client");
            GetComponent<CharacterController>().enabled = true;
            GetComponent<HarvestResources>().enabled = true;
            GetComponent<Build>().enabled = true;
            GetComponent<FirstPersonController>().enabled = true;
            GetComponent<AudioSource>().enabled = true;
            GetComponent<PlayerInventory>().enabled = true;
            GetComponentInChildren<AudioListener>().enabled = true;
            GetComponent<ItemManager>().enabled = true;
            GetComponent<MachineGun>().enabled = true;

            cameras[0].transform.tag = "MainCamera";

            foreach(Camera c in cameras)
            {
                c.enabled = true;
            }
        }
            
	}
}
