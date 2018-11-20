using System;
using UnityEngine;
using UnityEngine.UI;

public class NodeUpdater : MonoBehaviour {

    public int level;
    public Color childrenColor;
    public Slider childrenSlider;
    public GameObject childrenGroup;
    public int groupID;
    public Dropdown actionDropDown;

    int actionValue;

    public void SendValueToManager() {
        GameObject.Find("ChildrenManager").GetComponent<ChildrenManager>().UpdateChildren(level, gameObject);
    }

    public void SaveActionValue() {
        actionValue = actionDropDown.GetComponent<Dropdown>().value;
    }

    public int GetActionValue() {
        return actionValue;
    }

    public void SetSelfAsParent() {
        childrenGroup.GetComponent<HorizontalTracker>().AddParent(this);
    }

    //Called when the user clicks the link button. Sets the node as the current node to 
    //link to the next node the user hits the link button on
    public void SetAsNodeToLink() {
        ChildrenManager cM = GameObject.Find("ChildrenManager").GetComponent<ChildrenManager>();
        if (cM.nodeToLink != null) {
            if(cM.nodeToLink == gameObject) {
                cM.nodeToLink = null;
                cM.EnableAllLinkButtons();
                cM.DisableGroupLinkButtons();
                return;
            }
        }
        cM.nodeToLink = gameObject;
        //set all other link buttons to not be interactable
        cM.DisableAllLinkButtons();
        //set this button to be interactable to cancel the action
        transform.GetChild(7).GetComponent<Button>().interactable = true;
        cM.EnableGroupLinkButtons();
    }

}
