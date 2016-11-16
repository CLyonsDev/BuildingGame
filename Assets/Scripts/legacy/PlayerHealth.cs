using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityStandardAssets.Characters.FirstPerson;
using System.Collections;

public class PlayerHealth : NetworkBehaviour {

    public float maxHealth = 100;
    [SyncVar] public float health = 100;

    [SyncVar] public bool isDead = false;

    float shakeAmplitude = 0.1f;
    float shakeDuration = 0.05f;
    bool camShaking = false;

    Vector3 startPos;

    Image healthBar;

    bool spawnedWarning = false;

    // Use this for initialization
    void Start() {
        health = maxHealth;
        if (!isLocalPlayer)
            return;
        healthBar = GameObject.Find("HealthBar").GetComponent<Image>();
    }

    // Update is called once per frame
    void Update() {

        if (!isLocalPlayer)
            return;

        if (Input.GetKeyDown(KeyCode.V))
        {
            CamShake();
        }
        if (camShaking)
        {
            transform.localPosition = transform.localPosition + Random.insideUnitSphere * Random.Range(shakeAmplitude, 0.2f);
        }

        
        healthBar.fillAmount = Mathf.Lerp(healthBar.fillAmount, (health / maxHealth), 3 * Time.deltaTime);

        if (Input.GetKeyDown(KeyCode.L))
        {
            CmdRespawn(GetComponent<NetworkIdentity>().netId);
        }
    }

    public void TakeDamage(int dmg, float delay)
    {
        StartCoroutine(DelayedDamage(dmg, delay));
    }

    IEnumerator DelayedDamage(int dmg, float delay)
    {
        yield return new WaitForSeconds(delay);
        CamShake();
        GetComponent<FirstPersonController>().m_WalkSpeed = GetComponent<FirstPersonController>().m_attackedSpeed;
        health -= dmg;
        if (health <= 0)
        {
            Die();
        }
        StartCoroutine(ReturnSpeedAfterTime(0.5f));
    }

    void Die()
    {
        isDead = true;

        Renderer[] rends = GetComponentsInChildren<Renderer>();

        foreach (Renderer r in rends)
        {
            r.enabled = false;
        }

        GetComponentInChildren<CapsuleCollider>().enabled = false;
        GetComponent<FirstPersonController>().enabled = false;
        GetComponent<CharacterController>().enabled = false;
        FindObjectOfType<PopupManager>().SpawnHintBox("You are dead. You will respawn after the wave ends.", 3f);
    }

    public void Respawn(Vector3 spawnPos)
    {
        transform.position = spawnPos;
        health = maxHealth;
        isDead = false;

        Renderer[] rends = GetComponentsInChildren<Renderer>();

        foreach (Renderer r in rends)
        {
            r.enabled = true;
        }

        GetComponentInChildren<CapsuleCollider>().enabled = true;
        GetComponent<FirstPersonController>().enabled = true;
        GetComponent<CharacterController>().enabled = true;
    }

    [Command]
    void CmdRespawn(NetworkInstanceId netID)
    {
        Transform spawn = NetworkManager.singleton.GetStartPosition();
        RpcRespawn(netId, spawn.position);
    }

    [ClientRpc]
    void RpcRespawn(NetworkInstanceId netID, Vector3 spawnPos)
    {
        GameObject player = ClientScene.FindLocalObject(netId);
        player.GetComponent<PlayerHealth>().Respawn(spawnPos);
    }

    IEnumerator ReturnSpeedAfterTime(float time)
    {
        yield return new WaitForSeconds(time);
        GetComponent<FirstPersonController>().m_WalkSpeed = GetComponent<FirstPersonController>().m_normalSpeed;
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

    void OnTriggerEnter(Collider col)
    {
        if (col.transform.tag == "Border")
        {
            spawnedWarning = true;
            GameObject.Find("GameManager").GetComponent<PopupManager>().SpawnHintBox("Warning, you have entered the zombie spawn zone!", 2f);
        }
    }
}
