using UnityEngine;
using System.Collections;

public class DestroyOnSoundComplete : MonoBehaviour {

	void Update () {
	    if(!GetComponent<AudioSource>().isPlaying)
        {
            Destroy(this.gameObject);
        }
	}
}
