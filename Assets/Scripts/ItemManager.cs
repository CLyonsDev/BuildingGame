using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ItemManager : MonoBehaviour {

    /*
        1- Primary Weapon
        2- Secondary Weapon
        3- Build/Harvest Tool
    */

    GameObject highlightGO;

    GameObject primaryPanel, secondaryPanel, toolPanel;

    public GameObject primary, secondary, tool;

    public int itemIndex = 2;

    public enum equipment
    {
        primary,
        secondary,
        tool
    };

    public equipment selectedItem;

    Text buildToolModeText;

	// Use this for initialization
	void Start () {
        highlightGO = GameObject.Find("PanelHighlight");
        buildToolModeText = GameObject.Find("ToolMode").GetComponent<Text>();

        primaryPanel = GameObject.Find("PrimaryPanel");
        secondaryPanel = GameObject.Find("SecondaryPanel");
        toolPanel = GameObject.Find("ToolPanel");
	}

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SwitchItem(0);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SwitchItem(1);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SwitchItem(2);
        }

        /*if(Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            if (itemIndex + 1 >= 3)
                itemIndex = 0;
            else
                itemIndex++;

            SwitchItem(itemIndex);
        } else if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            if (itemIndex - 1 < 0)
                itemIndex = 2;
            else
                itemIndex--;

            SwitchItem(itemIndex);
        }*/
    }

    public void SwitchItem(int index)
    {
        switch(index)
        {
            case 0:
                itemIndex = 0;
                selectedItem = equipment.primary;
                highlightGO.transform.position = primaryPanel.transform.position;
                primary.SetActive(true);
                secondary.SetActive(false);
                tool.SetActive(false);
                break;
            case 1:
                itemIndex = 1;
                selectedItem = equipment.secondary;
                highlightGO.transform.position = secondaryPanel.transform.position;
                primary.SetActive(false);
                secondary.SetActive(true);
                tool.SetActive(false);
                break;
            case 2:
                itemIndex = 2;
                selectedItem = equipment.tool;
                highlightGO.transform.position = toolPanel.transform.position;
                primary.SetActive(false);
                secondary.SetActive(false);
                tool.SetActive(true);
                break;
        }

        if(index != 2 && GetComponent<Build>().isBuilding)
        {
            GetComponent<Build>().ToggleIfBuilding();
        }

    }

    public void SwapBuildMode(bool build)
    {
        if (build)
            buildToolModeText.text = "BUILD";
        else
            buildToolModeText.text = "HARVEST";
    }
}
