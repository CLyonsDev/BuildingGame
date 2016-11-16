using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class HarvestResources : NetworkBehaviour {

    RaycastHit harvestHitInfo;

    public float harvestAmount = 10f;

    public float timer = 0f;
    public float timeToHarvest = 0.5f;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if (GetComponent<Build>().isBuilding == true)
        {
            return;
        }

        if (Input.GetMouseButton(1))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out harvestHitInfo, Mathf.Infinity))
            {
                //Debug.Log("Ray hit: " + harvestHitInfo.transform.name + " at " + harvestHitInfo.point);
                //Debug.Log(harvestHitInfo.transform.parent.tag);
                if (harvestHitInfo.transform.parent == null)
                    return;
                if(harvestHitInfo.transform.parent.tag == "Tree")
                {
                    timer += Time.deltaTime;
                    if(timer >= timeToHarvest)
                    {
                        CmdHarvestResource("Tree", harvestHitInfo.transform.parent.gameObject.GetComponent<NetworkIdentity>().netId, GetComponent<NetworkIdentity>().netId, harvestAmount);
                        timer = 0;
                    }
                }
                else
                {
                    timer = 0;
                }
            }
        }
	}

    [Command]
    void CmdHarvestResource(string resourceType, NetworkInstanceId resourceGO, NetworkInstanceId harvestingPlayer, float harvestDamage)
    {
        RpcHarvestResource(resourceType, resourceGO, harvestingPlayer, harvestDamage);
    }

    [ClientRpc]
    void RpcHarvestResource(string resourceType, NetworkInstanceId resourceGO, NetworkInstanceId harvestingPlayer, float harvestDamage)
    {
        if(resourceType == "Tree")
        {
            GameObject resource = ClientScene.FindLocalObject(resourceGO);
            resource.GetComponent<Tree>().GetHarvested(harvestDamage);

            GameObject player = ClientScene.FindLocalObject(harvestingPlayer);
            player.GetComponent<PlayerInventory>().ModifyWood((int)(harvestDamage * resource.GetComponent<Tree>().resourcePerHealthPoint));

            Debug.Log("Collected some " + resourceType + "!");
        }
    }
}
