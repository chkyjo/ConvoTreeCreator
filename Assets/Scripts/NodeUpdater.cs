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
        int numChildren = (int)childrenSlider.value;

        GameObject.Find("ChildrenManager").GetComponent<ChildrenManager>().UpdateChildren(level, numChildren, gameObject);
    }

    public void SaveActionValue() {
        actionValue = actionDropDown.GetComponent<Dropdown>().value;
    }

    public int GetActionValue() {
        return actionValue;
    }

    public void SetAsNodeToLink() {
        GameObject.Find("ChildrenManager").GetComponent<ChildrenManager>().nodeToLink = gameObject;
    }

}
