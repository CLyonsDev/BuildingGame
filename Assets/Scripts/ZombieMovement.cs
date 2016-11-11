using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;

public class ZombieMovement : NetworkBehaviour {

    public Transform target;
    public List<Transform> allPlayers = new List<Transform>();

    float speed = 2;
    public float stopDistance = 1.5f;

	// Use this for initialization
	void Start () {
        if (!NetworkServer.active)
        {
            GetComponent<NavMeshAgent>().enabled = false;
            return;
        }

        foreach (GameObject g in GameObject.FindGameObjectsWithTag("Player"))
        {
            allPlayers.Add(g.transform);
        }

        if (allPlayers.Count == 0)
            return;

        target = allPlayers[Random.Range(0, allPlayers.Count)];
    }
	
	// Update is called once per frame
	void FixedUpdate () {
        if (!NetworkServer.active)
            return;

        if(target == null)
        {
            foreach (GameObject g in GameObject.FindGameObjectsWithTag("Player"))
            {
                allPlayers.Add(g.transform);
            }

            target = allPlayers[Random.Range(0, allPlayers.Count)];
            return;
        }

        //GetComponent<NavMeshAgent>().destination = target.position;

        if(Vector3.Distance(transform.position, target.position) > stopDistance)
        {
            GetComponent<Rigidbody>().MovePosition(transform.position + transform.forward * speed * Time.fixedDeltaTime);
            GetComponent<Animator>().SetBool("Walking", true);
        }

        transform.LookAt(new Vector3(target.position.x, transform.position.y, target.position.z));

    }
}
