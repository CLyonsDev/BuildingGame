using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections;

public class UIManager :  NetworkBehaviour{

    Text timerCountdownText;
    Text currentWaveNumberText;
    Text playersAliveNumber;
    Text enemiesAliveNumber;

    WaveManager wm;

	// Use this for initialization
	void Start () {
        currentWaveNumberText = GameObject.Find("WaveNumber").GetComponent<Text>();
        timerCountdownText = GameObject.Find("WaveTimerText").GetComponent<Text>();
        playersAliveNumber = GameObject.Find("PlayersAliveCounter").GetComponent<Text>();
        enemiesAliveNumber = GameObject.Find("EnemiesAliveCounter").GetComponent<Text>();
        wm = GetComponent<WaveManager>();
    }
	
	// Update is called once per frame
	void Update () {
        float minutesRemaining = Mathf.Floor(wm.buildTimeRemaining / 60);
        float seconds = Mathf.RoundToInt(wm.buildTimeRemaining % 60);

        playersAliveNumber.text = wm.playersAlive.Count.ToString();
        enemiesAliveNumber.text = wm.enemiesAlive.Length.ToString();

        if (wm.waveOngoing)
        {
            currentWaveNumberText.text = "Wave " + wm.waveNumber;
            timerCountdownText.text = "-∞-";
        }
        else
        {
            currentWaveNumberText.text = "Time to build!";
            timerCountdownText.text = (minutesRemaining + ":" + seconds.ToString("00"));
        }
    }
}
