using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class RepairBrick : NetworkBehaviour {

    float repairAmt = 15;
    float repairRate = 1f;

    float timer = 0;

    public LayerMask lm;

	// Use this for initialization
	void Start () {

    }
	
	// Update is called once per frame
	void Update () {
	    if(Input.GetMouseButton(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if(Physics.Raycast(ray, out hit, Mathf.Infinity, lm))
            {
                Debug.Log("Healing");
                timer += Time.deltaTime;
                if (timer >= repairRate)
                {

                    CmdHealBrick(hit.transform.gameObject.GetComponent<NetworkIdentity>().netId, repairAmt);
                }
            }  
        }
	}

    [Command]
    void CmdHealBrick(NetworkInstanceId brickID, float amount)
    {
        GameObject brick = NetworkServer.FindLocalObject(brickID);
        brick.GetComponent<BrickHealth>().HealBrick(amount);
    }
}
