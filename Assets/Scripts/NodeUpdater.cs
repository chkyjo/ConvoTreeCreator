using System;
using UnityEngine;
using UnityEngine.UI;

public class NodeUpdater : MonoBehaviour {

    public int level;
    public Color childrenColor;
    public Slider childrenSlider;
    public GameObject childrenGroup;

    public void SendValueToManager() {
        int numChildren = (int)childrenSlider.value;
        if(gameObject.name != "TreeRoot") {
            if (gameObject.transform.GetChild(3).GetComponent<InputField>().text == "") {
                Debug.Log("Error, did not set CharacterResponseID");
                childrenSlider.value = 0;
                return;
            }
        }

        GameObject.Find("ChildrenManager").GetComponent<ChildrenManager>().UpdateChildren(level, numChildren, gameObject);
    }


}
