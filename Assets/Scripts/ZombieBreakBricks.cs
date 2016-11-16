using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class ZombieBreakBricks : NetworkBehaviour {

    public float brickDamage = 5;

    float attackRate;
    float timer = 0;

	// Use this for initialization
	void Start () {
        if (!NetworkServer.active)
            this.enabled = false;

        attackRate = GetComponent<ZombieAttack>().attackRate;
        timer = attackRate;
	}
	
	// Update is called once per frame
	void Update () {
	    
	}

    void OnCollisionStay(Collision col)
    {
        if(col.transform.tag == "Brick")
        {
            timer += Time.deltaTime;
            if (timer >= attackRate)
            {
                AttackBrick(col.gameObject);
                //Debug.LogWarning("Attacking brick.");
                GetComponent<ZombieAttack>().attacking = true;
                timer = 0;
            }
        }   
    }

    void AttackBrick(GameObject brick)
    {
        brick.GetComponent<BrickHealth>().TakeDamage(brickDamage);
    }
}
