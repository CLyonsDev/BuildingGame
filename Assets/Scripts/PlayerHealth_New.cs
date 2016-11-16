using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections;

public class PlayerHealth_New : NetworkBehaviour {

    [SyncVar(hook = "OnHealthChanged")]
    public int currentHealth;
    private int maxHealth = 100;

    private bool shouldDie = false;
    [SyncVar] public bool isDead = false;

    public delegate void DieDelegate();
    public event DieDelegate EventDie;

    public delegate void RespawnDelegate();
    public event RespawnDelegate EventRespawn;

    Image healthImage;

	// Use this for initialization
	void Start () {
        currentHealth = maxHealth;

        healthImage = GameObject.Find("HealthBar").GetComponent<Image>();
        SetHealthBarFill();

        if(GameObject.Find("GameManager").GetComponent<WaveManager>().waveOngoing)
        {
            DeductHealth(maxHealth);
            GameObject.Find("GameManager").GetComponent<PopupManager>().SpawnHintBox("You have joined a game in progress! You will spwan when the wave is over.", 5f);
        }
	}
	
	// Update is called once per frame
	void Update () {
        CheckCondition();
        if(healthImage == null)
            healthImage = GameObject.Find("HealthBar").GetComponent<Image>();
    }

    void CheckCondition()
    {
        if(currentHealth <= 0 && !shouldDie && !isDead)
        {
            shouldDie = true;
        }

        if(currentHealth <= 0 && shouldDie)
        {
            if(EventDie != null)
            {
                EventDie();
            }

            shouldDie = false;
        }

        if(currentHealth > 0 && isDead)
        {
            if(EventRespawn != null)
            {
                EventRespawn();
            }
            isDead = false;
        }
    }

    public void ResetHealth()
    {
        currentHealth = maxHealth;
    }

    void SetHealthBarFill()
    {
        if(isLocalPlayer)
        {
            if(healthImage != null)
                healthImage.fillAmount = (float)currentHealth / maxHealth;
        }
    }

    public void DeductHealth(int damage)
    {
        Debug.Log("Took " + damage + " damage! (" + currentHealth + " / " + maxHealth + ")");
        currentHealth -= damage;
    }

    void OnHealthChanged(int newHealth)
    {
        currentHealth = newHealth;
        SetHealthBarFill();
    }
}
