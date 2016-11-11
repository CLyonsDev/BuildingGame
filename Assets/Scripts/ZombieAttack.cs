using UnityEngine;
using System.Collections;

public class ZombieAttack : MonoBehaviour {

    Transform target;

    float timer = 0;
    float attackRate = 1.4f;

    int damage = 5;

    float stopDist;



	// Use this for initialization
	void Start () {
        stopDist = GetComponent<ZombieMovement>().stopDistance;

    }
	
	// Update is called once per frame
	void Update () {
        if (target == null)
            target = GetComponent<ZombieMovement>().target;

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
                Attack();
                timer = 0;
            }
        }
	}

    void Attack()
    {
        Invoke("DelayedDamage", 1f);
        GetComponent<Animator>().SetTrigger("Attack");
        GetComponent<Animator>().SetBool("Walking", false);
    }
    
    void DelayedDamage()
    {
        if(Vector3.Distance(transform.position, target.position) <= stopDist)
            target.GetComponent<PlayerHealth>().TakeDamage(damage);
    }
}
