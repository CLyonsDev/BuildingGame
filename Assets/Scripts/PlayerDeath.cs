using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections;

public class PlayerDeath : NetworkBehaviour {

    private PlayerHealth_New healthScript;

	// Use this for initialization
	void Start () {
        healthScript = GetComponent<PlayerHealth_New>();
        healthScript.EventDie += DisablePlayer;
	}

    void OnDisable()
    {
        healthScript.EventDie -= DisablePlayer;
    }

    void DisablePlayer()
    {
        Debug.Log("Player dead!");
        GetComponent<CharacterController>().enabled = false;
        GetComponent<PlayerInventory>().enabled = false;
        GetComponent<UnityStandardAssets.Characters.FirstPerson.FirstPersonController>().enabled = false;

        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        foreach(Renderer rend in renderers)
        {
            rend.enabled = false;
        }

        GetComponentInChildren<CapsuleCollider>().enabled = false;

        healthScript.isDead = true;

        if(isLocalPlayer)
        {
            FindObjectOfType<PopupManager>().SpawnHintBox("You are dead. You will respawn after the wave ends.", 3f);
            //Respawn Stuff.
        }
    }
}
