using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class Build : NetworkBehaviour {

    public GameObject[] bricks;
    public GameObject ghostBrick;

    [SyncVar]
    public int brickIndex = 0;
    [SyncVar]
    public bool snapping = false;

    RaycastHit passiveRayHitInfo;
    public LayerMask lm;
    public LayerMask destroyLM;

    Quaternion brickRot;

    public bool isBuilding = true;

    public Material ghostBrickMaterial;

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        
        if(Input.GetKeyDown(KeyCode.B) && GetComponent<ItemManager>().itemIndex == 2)
        {
            ToggleIfBuilding();
        }

        if (!isBuilding)
        {
            if(ghostBrick != null)
                Destroy(ghostBrick);
            return;
        }

        //Create a ghost brick if we don't already have one.
        if (ghostBrick == null)
        {
            //CmdCreateGhostBrick();
            ghostBrick = (GameObject)Instantiate(bricks[brickIndex]);
            ghostBrick.GetComponentInChildren<Collider>().enabled = false;
            ghostBrick.GetComponentInChildren<Rigidbody>().isKinematic = true;
            ghostBrick.layer = LayerMask.NameToLayer("Ignore Raycast");
            ghostBrick.transform.GetChild(0).gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
            ghostBrick.GetComponentInChildren<Renderer>().material = ghostBrickMaterial;
        }

        //Snapping
        if(Input.GetKeyDown(KeyCode.R))
        {
            snapping = !snapping;
        }else if(Input.GetKeyDown(KeyCode.E)) //Rotation
        {
            ghostBrick.transform.Rotate(Vector3.up * 90, Space.World);
            brickRot = ghostBrick.transform.rotation;
        }

        //Moving ghost brick
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if(Physics.Raycast(ray, out passiveRayHitInfo, lm))
        {
            if (ghostBrick == null)
            {
                Debug.LogError("We don't have a ghost brick to move!");
                return;
            }
            else
            {
                if (passiveRayHitInfo.transform.gameObject.layer != LayerMask.NameToLayer("Buildable") && passiveRayHitInfo.transform.gameObject.layer != LayerMask.NameToLayer("Brick"))
                    return;

                Vector3 posToMoveTo = Vector3.zero;
                if (!snapping)
                {
                    posToMoveTo = new Vector3(Mathf.Round(passiveRayHitInfo.point.x / 0.25f) * 0.25f,
                                              Mathf.Round(passiveRayHitInfo.point.y / 0.5f) * 0.5f,
                                              Mathf.Round(passiveRayHitInfo.point.z / 0.25f) * 0.25f);
                    //posToMoveTo = new Vector3(RoundToHalf(passiveRayHitInfo.point.x), RoundToHalf(passiveRayHitInfo.point.y), RoundToHalf(passiveRayHitInfo.point.z));
                }
                else
                {
                    if (passiveRayHitInfo.transform.gameObject.layer == LayerMask.NameToLayer("Buildable"))
                        return;

                    posToMoveTo = new Vector3(passiveRayHitInfo.transform.position.x, 
                                              passiveRayHitInfo.transform.position.y
                                                + passiveRayHitInfo.transform.GetComponentInChildren<Collider>().bounds.size.y
                                                + ghostBrick.transform.GetChild(0).GetComponent<Collider>().bounds.size.y , 
                                              passiveRayHitInfo.transform.position.z);
                    //posToMoveTo = new Vector3(passiveRayHitInfo.transform.position.x, passiveRayHitInfo.transform.position.y + ghostBrick.GetComponentInChildren<Renderer>().bounds.size.y, passiveRayHitInfo.transform.position.z); //+ ghostBrick.GetComponent<Renderer>().bounds.size.y


                    //Debug.Log(posToMoveTo);
                }


                ghostBrick.transform.position = posToMoveTo;
            }
        }

        //Creating brick
        if(Input.GetMouseButtonDown(0))
        {
            //Debug.Log(ghostBrick.transform.position);
            Vector3 pos = ghostBrick.transform.position;
            CmdCreateBrick(pos, brickIndex, brickRot);
            //CmdTestCmd();
        }else if(Input.GetMouseButtonDown(1))
        {
            Ray brickDestroyRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if(Physics.Raycast(brickDestroyRay, out hit, destroyLM))
            {
                CmdDestroyBrick(hit.transform.gameObject.GetComponentInChildren<NetworkIdentity>().netId);
            }
        }

        if(Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            if (brickIndex + 1 < bricks.Length)
            {
                brickIndex++;

            }
            else
            {
                brickIndex = 0;
            }

            CmdChangeGhost(ghostBrick.GetComponent<NetworkIdentity>().netId);
            Destroy(ghostBrick);
        }else if(Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            if (brickIndex - 1 >= 0)
                brickIndex--;
            else
                brickIndex = bricks.Length - 1;

            CmdChangeGhost(ghostBrick.GetComponent<NetworkIdentity>().netId);
            Destroy(ghostBrick);
        }
	}

    public static float RoundToHalf(float f)
    {
        return f = Mathf.Round(f * 4f) * 0.25f;
    }

    public void ToggleIfBuilding()
    {
        isBuilding = !isBuilding;
        ItemManager im = GetComponent<ItemManager>();
        if(isBuilding == false)
        {
            //Debug.Log("Hey can u pls destroy " + ghostBrick.transform.name + "(" + ghostBrick.GetComponent<NetworkIdentity>().netId + ")");
            //CmdDestroyBrick(ghostBrick.GetComponent<NetworkIdentity>().netId);
            Destroy(ghostBrick);
        }
        im.SwapBuildMode(isBuilding);
        Debug.LogWarning("Toggled building. " + isBuilding);
    }

    [Command]
    void CmdTestCmd()
    {
        GameObject test = (GameObject)Instantiate(bricks[0]);
        NetworkServer.Spawn(test);
    }

    [Command]
    void CmdDestroyBrick(NetworkInstanceId brickID)
    {
        GameObject brick = NetworkServer.FindLocalObject(brickID);
        Debug.Log(brick.transform.name + " ok i will destroy ("+brickID+")");
        NetworkServer.Destroy(brick);
    }

    [Command]
    void CmdChangeGhost(NetworkInstanceId ghostID)
    {
        GameObject ghost = NetworkServer.FindLocalObject(ghostID);
        NetworkServer.Destroy(ghost);
    }


    [Command]
    void CmdCreateBrick(Vector3 pos, int brickGOIndex, Quaternion rot)
    {
        //Debug.Log("spawn rpc");
        GameObject newBrick = (GameObject)Instantiate(bricks[brickGOIndex], pos, rot);
        newBrick.transform.tag = "Brick";
        newBrick.layer = LayerMask.NameToLayer("Brick");
        NetworkServer.Spawn(newBrick);

        //newBrick.transform.position = pos;
        //newBrick.transform.rotation = rot;
        //RpcCreateBrick(pos, brickToSpawn, rot);
    }

    [ClientRpc]
    void RpcCreateBrick(Vector3 pos, GameObject brickToSpawn, Quaternion rot)
    {
        
    }
}
