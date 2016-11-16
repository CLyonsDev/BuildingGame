using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class ZombieAttack : NetworkBehaviour {

    public Transform target;

    float timer = 0;
    public float attackRate = 1.4f;

    int damage = 5;

    float stopDist;

    [SyncVar]
    public bool attacking;
    [SyncVar]
    public bool playerInAttackRange = false;

	// Use this for initialization
	void Start () {
        stopDist = GetComponent<ZombieMovement>().stopDistance;

    }
	
	// Update is called once per frame
	void Update () {

        if (attacking)
        {
            GetComponent<Animator>().SetTrigger("Attack");
            GetComponent<Animator>().SetBool("Walking", false);
            attacking = false;
        }

        if (!NetworkServer.active)
            return;

        if (target == null)
        {
            if(GetComponent<ZombieMovement>().target)
            {
                target = GetComponent<ZombieMovement>().target;
                return;
            }
        }

        if (target.GetComponent<PlayerHealth_New>().isDead)
        {
            target = null;
            attacking = false;
            return;
        }

        playerInAttackRange = (Vector3.Distance(transform.position, target.position) <= stopDist);

        if(timer < attackRate)
        {
            timer += Time.deltaTime;
            if (timer > attackRate)
                timer = attackRate;
        }

        if (Vector3.Distance(transform.position, target.position) <= stopDist)
        {
            if (timer >= attackRate)
            {
                attacking = true;
                //NetworkInstanceId playerID = target.gameObject.GetComponent<NetworkIdentity>().netId;
                //Debug.LogWarning(target.transform.name + " - " + playerID + " - Client");
                Attack();
                timer = 0;
            }
        }
	}
    /*it was returning the zombie instead of the target idk why
    [Command]
    void CmdAttackPlayer(NetworkInstanceId netID)
    {
        //Debug.LogError("testCMD");
        //PlayerHealth player = PlayerList.GetPlayer(_playerID);
        //player.
        GameObject thePlayer = NetworkServer.FindLocalObject(netId);
        Debug.Log(thePlayer.transform.name + " - " + netId + " - Server");
        //player.GetComponent<PlayerHealth>().TakeDamage(damage, 1f);
    }*/

    void Attack()
    {
        Invoke("DelayedDamage", 1f);
    }
    
    void DelayedDamage()
    {
        if (target == null)
            return;

        if(Vector3.Distance(transform.position, target.position) <= stopDist)
            target.GetComponent<PlayerHealth_New>().DeductHealth(damage);
    }
}
