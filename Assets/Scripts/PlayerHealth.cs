using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class PlayerHealth : NetworkBehaviour {

    [SyncVar] public int health = 100;

    float shakeAmplitude = 0.1f;
    float shakeDuration = 0.05f;
    bool camShaking = false;

    Vector3 startPos;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
        if(Input.GetKeyDown(KeyCode.V))
        {
            CamShake();
        }
	    if(camShaking)
        {
            transform.localPosition = transform.localPosition + Random.insideUnitSphere * Random.Range(shakeAmplitude, 0.2f);
        }
	}

    public void TakeDamage(int dmg)
    {
        CamShake();

        health -= dmg;
        if (health <= 0)
        {
            Debug.LogError("DEAD");
            Destroy(this.gameObject);
        }
    }

    void CamShake()
    {
        startPos = transform.localPosition;
        CancelInvoke();
        camShaking = true;
        Invoke("StopShake", shakeDuration);
    }

    void StopShake()
    {
        camShaking = false;
        transform.localPosition = startPos;
    }
}
