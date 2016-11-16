using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class BrickHealth : NetworkBehaviour {

    [SyncVar]
    public float brickHealth;

    public float maxHealth = 100;

	// Use this for initialization
	void Start () {
        brickHealth = maxHealth;
	}
	
	// Update is called once per frame
	void Update () {
	}

    public void TakeDamage(float dmg)
    {
        brickHealth -= dmg;

        if(brickHealth <= 0)
        {
            GetComponent<Rigidbody>().isKinematic = false;
            GetComponent<Rigidbody>().AddForce(Vector3.up * 3, ForceMode.Impulse);
            gameObject.layer = LayerMask.NameToLayer("DeadBrick");
            gameObject.tag = "DeadBrick";
        }
    }

    public void HealBrick(float amt)
    {
        brickHealth += amt;

        if (brickHealth > maxHealth)
            brickHealth = maxHealth;
    }
}
