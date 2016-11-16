using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class ZombieAnimationControllerScript : NetworkBehaviour {

    void SetControllerBool(string name, bool value, GameObject target)
    {
        CmdSetControllerBool(name, value, target.GetComponent<NetworkIdentity>().netId);
    }

    [Command]
    void CmdSetControllerBool(string name, bool value, NetworkInstanceId netID)
    {
        RpcSetControllerBool(name, value, netID);
    }

    [ClientRpc]
    void RpcSetControllerBool(string name, bool value, NetworkInstanceId netID)
    {
        GameObject target = ClientScene.FindLocalObject(netID);
        target.GetComponent<Animator>().SetBool(name, value);
    }
}
