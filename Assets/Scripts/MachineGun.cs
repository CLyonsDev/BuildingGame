using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class MachineGun : NetworkBehaviour {

    float fireRate = 0.15f;

    public bool canFire = true;

    RaycastHit hit;

    public LayerMask lm;

    float damage = 5;

    public bool isEnabled = true;

    public GameObject[] bulletHoles;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if (!isEnabled)
            return;

	    if(Input.GetMouseButton(0) && canFire)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if(Physics.Raycast(ray, out hit, Mathf.Infinity, lm))
            {
                Debug.Log("Hit");
                CmdShoot(hit.transform.gameObject.GetComponent<NetworkIdentity>().netId);
            }
            canFire = false;
            Invoke("FireDelay", fireRate);
        }
	}

    void FireDelay()
    {
        canFire = true;
    }

    [Command]
    void CmdShoot(NetworkInstanceId netid)
    {
        GameObject target = NetworkServer.FindLocalObject(netid);
        target.GetComponent<EnemyHealth>().TakeDamage(damage);

        //var hitRotation = Quaternion.FromToRotation(Vector3.back, hit.normal);
        //GameObject bh = (GameObject)Instantiate(bulletHoles[0], hit.point, hitRotation);
        //bh.transform.position = bh.transform.position + (-bh.transform.forward / 5);
        //bh.transform.SetParent(target.transform);

       // NetworkServer.Spawn(bh);
    }
}
