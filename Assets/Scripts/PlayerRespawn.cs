using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class PlayerRespawn : NetworkBehaviour {

    private PlayerHealth_New healthScript;
    private WaveManager wm;

	// Use this for initialization
	void Start () {
        wm = GameObject.Find("GameManager").GetComponent<WaveManager>();

        healthScript = GetComponent<PlayerHealth_New>();
        healthScript.EventRespawn += EnablePlayer;
	}

    void OnDisable()
    {
        healthScript.EventRespawn -= EnablePlayer;
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.P))
        {
            StartRespawn();
        }

        if(!wm.waveOngoing && healthScript.isDead)
        {
            StartRespawn();
        }
    }

    void EnablePlayer()
    {
        Debug.Log("Player alive!");
        GetComponent<CharacterController>().enabled = true;
        GetComponent<PlayerInventory>().enabled = true;

        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        foreach (Renderer rend in renderers)
        {
            rend.enabled = true;
        }

        GetComponentInChildren<CapsuleCollider>().enabled = true;

        if (isLocalPlayer)
        {
            GetComponent<UnityStandardAssets.Characters.FirstPerson.FirstPersonController>().enabled = true;
            //Respawn Client Stuff.
        }
    }

    public void StartRespawn()
    {
        CmdRespawnOnServer(GetComponent<NetworkIdentity>().netId);
    }

    [Command]
    void CmdRespawnOnServer(NetworkInstanceId netID)
    {
        GameObject player = NetworkServer.FindLocalObject(netID);
        Transform spawn = NetworkManager.singleton.GetStartPosition();
        player.transform.position = spawn.position;
        healthScript.ResetHealth();
    }
}
