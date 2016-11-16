using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class ZombieMovement : NetworkBehaviour {

    public Transform target;
    public List<Transform> allPlayers = new List<Transform>();

    float speed = 4;
    float lookSpeed = 5f;
    public float stopDistance = 1.5f;

    float distTraveled = 0;

    Vector3 lastPos;

    float distToGround;

    [SyncVar]
    public bool isWalking;

	// Use this for initialization
	void Start () {
        if (!NetworkServer.active)
        {
            return;
        }

        lastPos = transform.position;

        StartCoroutine("Think");
        //Debug.Log("Started Think.");

        foreach (GameObject g in GameObject.FindGameObjectsWithTag("Player"))
        {
            allPlayers.Add(g.transform);
        }

        if (allPlayers.Count == 0)
            return;

        distToGround = GetComponentInChildren<MeshCollider>().bounds.extents.y;

        target = allPlayers[Random.Range(0, allPlayers.Count)];
    }
	
	// Update is called once per frame
	void Update () {

        if(isWalking)
            GetComponent<Animator>().SetBool("Walking", true);

        if (!NetworkServer.active)
            return;

        if (target.GetComponent<PlayerHealth_New>().isDead)
            target = null;

            //if (!GetComponent<NavMeshAgent>().enabled && IsGrounded())
            //    GetComponent<NavMeshAgent>().enabled = true;

        if (target == null)
        {
            foreach (GameObject g in GameObject.FindGameObjectsWithTag("Player"))
            {
                if(!g.GetComponent<PlayerHealth_New>().isDead)
                    allPlayers.Add(g.transform);
            }

            target = allPlayers[Random.Range(0, allPlayers.Count)];
            return;
        }

        //if(GetComponent<NavMeshAgent>().enabled)
        //   GetComponent<NavMeshAgent>().SetDestination(target.position);

        //GetComponent<NavMeshAgent>().destination = target.position;

        Quaternion targetRot = Quaternion.LookRotation(new Vector3(target.position.x, transform.position.y, target.position.z) - transform.position);

        //transform.LookAt(new Vector3(target.position.x, transform.position.y, target.position.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, lookSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, target.position) >= stopDistance)
        {
            transform.position += transform.forward * speed * Time.deltaTime;
            //GetComponent<Rigidbody>().MovePosition(transform.position + transform.forward * speed * Time.fixedDeltaTime);
            isWalking = true;
        }

        //transform.LookAt(new Vector3(target.position.x, transform.position.y, target.position.z));

    }

    bool IsGrounded()
    {
        return Physics.Raycast(transform.position, -Vector3.up, distToGround + 0.1f);
    }

    IEnumerator Think()
    {
        while(true)
        {
            yield return new WaitForSeconds(Random.Range(0.5f, 1.5f));

            distTraveled = Vector3.Distance(transform.position, lastPos);
            //Debug.Log(distTraveled);
            if (distTraveled <= 0.9f && !GetComponent<ZombieAttack>().playerInAttackRange)
            {
                Vector3 moveDir = new Vector3(0, 600, 0);
                GetComponent<Rigidbody>().AddForce(moveDir);
            }
            lastPos = transform.position;

        }
    }
}
