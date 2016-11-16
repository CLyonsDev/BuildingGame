using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class ResourceManager : NetworkBehaviour {

    public GameObject[] trees;
    GameObject[] treeSpawnLoc;

    int treesPerSpawnLoc = 5;
    float radius = 4;

    float pi2 = 2 * Mathf.PI;

	// Use this for initialization
	void Start () {

        treeSpawnLoc = GameObject.FindGameObjectsWithTag("Tree Spawn");
        //if (NetworkServer.active)
        //{
        //    Debug.Log("SpawnTrees");
         //   SpawnTrees();
        //}

        //PoolManager.instance.CreatePool()
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
    }

    [Command]
    void CmdSpawnTrees()
    {
        StartCoroutine(SpawnTreesEnum());
    }

    IEnumerator SpawnTreesEnum()
    {
        if (!NetworkServer.active)
            yield return null;

        Debug.Log("SpawnTreesEnum");
        for (int i = 0; i < treeSpawnLoc.Length; i++)
        {
            if (treeSpawnLoc[i].transform.childCount == 0)
            {
                for (int j = 0; j < treesPerSpawnLoc; j++)
                {
                    yield return new WaitForSeconds(0.0000001f);
                
                    Vector3 pos = treeSpawnLoc[i].transform.position + new Vector3(Mathf.Cos(j * pi2 / treesPerSpawnLoc) * radius, 0, Mathf.Sin(j * pi2 / treesPerSpawnLoc) * radius);
                    GameObject tree = (GameObject)Instantiate(trees[Random.Range(0, trees.Length)], pos, Quaternion.identity);

                    if(NetworkServer.active)
                    {
                        NetworkServer.Spawn(tree);
                    }

                    tree.transform.Rotate(Vector3.up * Random.Range(0, 360));
                    tree.transform.SetParent(treeSpawnLoc[i].transform, true);
                }
            }  
        }
    }
}
