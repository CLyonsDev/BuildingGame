using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PopupManager : MonoBehaviour {

    public GameObject popupGO;
    Transform playerUI;

	// Use this for initialization
	void Start () {
        playerUI = GameObject.Find("PlayerUI").transform;
	}

    public void SpawnHintBox(string hintText, float duration)
    {
        GameObject newPopup = (GameObject)Instantiate(popupGO, playerUI, false);
        newPopup.transform.GetChild(0).GetChild(0).gameObject.GetComponent<Text>().text = hintText;
        StartCoroutine(DestroyHintBoxAfterTime(newPopup, 5f));
    }

    IEnumerator DestroyHintBoxAfterTime(GameObject thingToDestroy, float time)
    {
        yield return new WaitForSeconds(time);
        Destroy(thingToDestroy);
    }
}
