using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;
using UnityEngine.Networking;
using System.Collections;

[RequireComponent(typeof(Player))]
public class NetworkScriptManager : NetworkBehaviour
{

    public Camera[] cameras;

    void Start()
    {
        if (isLocalPlayer)
        {
            GetComponent<CharacterController>().enabled = true;
            GetComponent<HarvestResources>().enabled = true;
            GetComponent<Build>().enabled = true;
            GetComponent<FirstPersonController>().enabled = true;
            GetComponent<AudioSource>().enabled = true;
            GetComponent<PlayerInventory>().enabled = true;
            GetComponentInChildren<AudioListener>().enabled = true;
            GetComponent<ItemManager>().enabled = true;
            GetComponent<BuildToolUI>().enabled = true;
            GetComponent<PlayerHealth_New>().enabled = true;

            cameras[0].transform.tag = "MainCamera";

            foreach (Camera c in cameras)
            {
                c.enabled = true;
            }
        }
    }
}
