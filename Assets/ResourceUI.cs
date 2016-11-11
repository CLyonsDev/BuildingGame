using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ResourceUI : MonoBehaviour {

    Text woodText;
    PlayerInventory pi;

    float woodNum = 0f;
    float modifier = 0.5f;

	// Use this for initialization
	void Start () {
        woodText = transform.FindChild("WoodText").gameObject.GetComponent<Text>();
        pi = transform.parent.parent.parent.parent.gameObject.GetComponent<PlayerInventory>();
	}
	
	// Update is called once per frame
	void Update () {
        woodNum = Mathf.Round(Mathf.Lerp(woodNum, pi.wood, modifier));
        woodText.text = "W: " + woodNum;
	}
}
