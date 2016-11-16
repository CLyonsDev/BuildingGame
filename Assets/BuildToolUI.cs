using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BuildToolUI : MonoBehaviour {

    public Text currentText;
    public Text prevText;
    public Text nextText;

    public Image currentImg, prevImg, nextImg;

    public GameObject buildPanel;

    Build buildScript;

	// Use this for initialization
	void Start () {
        buildScript = GetComponent<Build>();
    }
	
	// Update is called once per frame
	void Update () {

        currentText.text = "Hello";
        prevText.text = "Prev";
        nextText.text = "Next";

        currentText.text = "Selected: " + buildScript.bricks[buildScript.brickIndex].GetComponent<Brick>().brickName;
        currentImg.sprite = buildScript.bricks[buildScript.brickIndex].GetComponent<Brick>().icon;

        //prevText.text = "Meme";

        if(buildScript.brickIndex < buildScript.bricks.Length - 1)
        {
            nextText.text = "Next: " + buildScript.bricks[buildScript.brickIndex + 1].GetComponent<Brick>().brickName;
            nextImg.sprite = buildScript.bricks[buildScript.brickIndex + 1].GetComponent<Brick>().icon;
            //nextText.text = ("Meme");
        }
        else
        {
            nextText.text = "Next: " + buildScript.bricks[0].GetComponent<Brick>().brickName;
            nextImg.sprite = buildScript.bricks[0].GetComponent<Brick>().icon;
            //nextText.text = ("Machine");
        }

        if (buildScript.brickIndex > 0)
        {
            prevText.text = "Prev: " + buildScript.bricks[buildScript.brickIndex - 1].GetComponent<Brick>().brickName;
            prevImg.sprite = buildScript.bricks[buildScript.brickIndex - 1].GetComponent<Brick>().icon;
            //nextText.text = ("Dream");
        }
        else
        {
            prevText.text = "Prev: " + buildScript.bricks[buildScript.bricks.Length - 1].GetComponent<Brick>().brickName;
            prevImg.sprite = buildScript.bricks[buildScript.bricks.Length - 1].GetComponent<Brick>().icon;
            //nextText.text = ("Latrine");
        }

    }
}
