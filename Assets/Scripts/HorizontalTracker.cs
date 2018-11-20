using UnityEngine;
using System.Collections.Generic;

public class HorizontalTracker : MonoBehaviour {

    public List<NodeUpdater> parents;
    public GameObject level;
    public int groupID;

    void Awake() {
        parents = new List<NodeUpdater>();
    }

    void Start() {
        
    }

    public void Minimize() {
        int numChildren = transform.childCount;
        for(int i = 1; i < numChildren; i++) {
            transform.GetChild(i).gameObject.SetActive(false);
            GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 30);
            GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 65);
        }
    }

    public void Maximize() {
        int numChildren = transform.childCount;
        for (int i = 1; i < numChildren; i++) {
            transform.GetChild(i).gameObject.SetActive(true);
            GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, ((numChildren - 1) * 110) + 30);
            GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 85);
        }
    }

    public void AddParent(NodeUpdater node) {
        Debug.Log("Adding parent");
        parents.Add(node);
    }

    public void RemoveParent(NodeUpdater node) {
        Debug.Log("Removing parent");
        parents.Remove(node);
    }

    public void SetAsGroupToLink() {
        ChildrenManager cM = GameObject.Find("ChildrenManager").GetComponent<ChildrenManager>();
        cM.EnableAllLinkButtons();
        cM.SetGroup(gameObject);
        cM.DisableGroupLinkButtons();
    }
}
