using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;

public class MachineGun : NetworkBehaviour {

    float fireRate = 0.095f;

    public bool canFire = true;
    public bool reloading = false;

    RaycastHit hit;

    public LayerMask lm;

    float damage = 15;

    [SyncVar] public float currentAmmo, maxAmmo;

    public bool isEnabled = true;

    public GameObject[] bulletHoles;

    public GameObject audioGO;
    public AudioClip[] weaponSounds;

    Text ammoText;

	// Use this for initialization
	void Start () {
        currentAmmo = maxAmmo;
        ammoText = GameObject.Find("PrimaryPanel").transform.FindChild("Ammo").GetComponent<Text>();
	}
	
	// Update is called once per frame
	void Update () {
        if (!isEnabled)
            return;

        ammoText.text = currentAmmo.ToString() + "/" + maxAmmo.ToString();

        if (Input.GetMouseButton(0) && canFire && currentAmmo > 0 && !reloading)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if(Physics.Raycast(ray, out hit, Mathf.Infinity, lm))
            {
                CmdShoot(hit.transform.gameObject.GetComponent<NetworkIdentity>().netId);
            }
            else
            {
                CmdSpawnSoundGO(transform.position, GetComponent<NetworkIdentity>().netId, 0);
            }
            currentAmmo--;
            canFire = false;
            Invoke("FireDelay", fireRate);
        }else if(Input.GetMouseButtonDown(0) && currentAmmo <= 0 && !reloading)
        {
            reloading = true;
            StartCoroutine(Reload(3.5f));
        }

        if(Input.GetKeyDown(KeyCode.R) && currentAmmo != maxAmmo && !reloading)
        {
            reloading = true;
            StartCoroutine(Reload(3.5f));
        }
	}

    void FireDelay()
    {
        canFire = true;
    }

    IEnumerator Reload(float delay)
    {
        CmdSpawnSoundGO(transform.position, GetComponent<NetworkIdentity>().netId, 1);
        yield return new WaitForSeconds(delay);
        currentAmmo = maxAmmo;
        reloading = false;
    }

    [Command]
    void CmdSpawnSoundGO(Vector3 pos, NetworkInstanceId weaponID, int index)
    {
        RpcSpawnSoundGO(pos, weaponID, index);
    }

    [Command]
    void CmdShoot(NetworkInstanceId netid)
    {
        GameObject target = NetworkServer.FindLocalObject(netid);
        target.GetComponent<EnemyHealth>().TakeDamage(damage);

        RpcSpawnSoundGO(transform.position, GetComponent<NetworkIdentity>().netId, 0);

        //var hitRotation = Quaternion.FromToRotation(Vector3.back, hit.normal);
        //GameObject bh = (GameObject)Instantiate(bulletHoles[0], hit.point, hitRotation);
        //bh.transform.position = bh.transform.position + (-bh.transform.forward / 5);
        //bh.transform.SetParent(target.transform);

       // NetworkServer.Spawn(bh);
    }
    [ClientRpc]
    void RpcSpawnSoundGO(Vector3 pos, NetworkInstanceId weaponID, int index)
    {
        Transform weapon = ClientScene.FindLocalObject(weaponID).transform;
        GameObject soundGO = (GameObject)Instantiate(audioGO, weapon, false);
        soundGO.GetComponent<AudioSource>().clip = weaponSounds[index];
        soundGO.GetComponent<AudioSource>().Play();
    }
}
