using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class ResourceManager : NetworkBehaviour {

    public GameObject[] trees;
    GameObject[] treeSpawnLoc;

	// Use this for initialization
	void Start () {
        treeSpawnLoc = GameObject.FindGameObjectsWithTag("Tree Spawn");
        if (NetworkServer.active)
        {
            Debug.Log("SpawnTrees");
            SpawnTrees();
        }
    }
	
	// Update is called once per frame
	void Update () {
	    if(Input.GetKeyDown(KeyCode.G))
        {
            SpawnTrees();
        }
	}

    public void SpawnTrees()
    {
        if (NetworkServer.active)
            CmdSpawnTrees();
        else
            Debug.LogError("wat not there");
    }

    [Command]
    void CmdSpawnTrees()
    {
        RpcSpawnTrees(GetComponent<NetworkIdentity>().netId);
    }
    [ClientRpc]
    void RpcSpawnTrees(NetworkInstanceId netID)
    {
        GameObject gm = ClientScene.FindLocalObject(netId);
        gm.GetComponent<ResourceManager>().StartCoroutine(SpawnTreesEnum());
    }

    IEnumerator SpawnTreesEnum()
    {
        for (int i = 0; i < treeSpawnLoc.Length; i++)
        {
            yield return new WaitForSeconds(Random.Range(0f, 0.05f));
            if(treeSpawnLoc[i].transform.childCount > 0)
            {
                Destroy(treeSpawnLoc[i].transform.GetChild(0).gameObject);
            }
            GameObject tree = (GameObject)Instantiate(trees[Random.Range(0, trees.Length)], treeSpawnLoc[i].transform, false);
            NetworkServer.Spawn(tree);
            tree.transform.position = tree.transform.parent.position;
            tree.transform.Rotate(Vector3.up * Random.Range(0, 360));
        }
    }
}
