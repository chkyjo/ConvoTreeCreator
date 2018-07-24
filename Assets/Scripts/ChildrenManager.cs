using UnityEngine;
using UnityEngine.UI;

public class ChildrenManager : MonoBehaviour {

    public GameObject treePanel;
    public GameObject horizontalPanel;
    public GameObject treeNode;
    public GameObject levelPanel;

    Vector2 offset = new Vector2(1, 0);

    public void UpdateChildren(int parentLevel, int numChildren, GameObject parent) {
        
        if (numChildren == 0) {
            //destroy group
            Destroy(parent.GetComponent<NodeUpdater>().childrenGroup);
        }
        else {
            //if child count of the treePanel is equal to the level of the node calling the function
            //then it is on the last level of the tree and a new level must be created
            if (treePanel.transform.childCount == parentLevel + 1) {
                var newLevel = Instantiate(levelPanel);
                newLevel.transform.SetParent(treePanel.transform, false);
            }
            //if there is no group create one
            if (parent.GetComponent<NodeUpdater>().childrenGroup == null) {
                var group = Instantiate(horizontalPanel);
                group.GetComponent<Image>().color = parent.GetComponent<NodeUpdater>().childrenColor;
                group.transform.SetParent(treePanel.transform.GetChild(parentLevel + 1).transform, false);
                parent.GetComponent<NodeUpdater>().childrenGroup = group;
            }

            int difference;
            if (parent.GetComponent<NodeUpdater>().childrenGroup.transform.childCount - 1 < numChildren) {
                difference = numChildren - (parent.GetComponent<NodeUpdater>().childrenGroup.transform.childCount - 1);
                for(int i = 0; i < difference; i++) {
                    treeNode.GetComponent<NodeUpdater>().level = parentLevel + 1;
                    float randRed = Random.Range(0, 256);
                    float randBlue = Random.Range(0, 256);
                    float randGreen = Random.Range(0, 256);
                    treeNode.GetComponent<Image>().color = new Color(randRed / 255f, randBlue / 255f, randGreen / 255f);
                    treeNode.GetComponent<NodeUpdater>().childrenColor = new Color(randRed / 255f, randBlue / 255f, randGreen / 255f);
                    treeNode.transform.GetChild(6).GetComponent<InputField>().text = "0";
                    var leafnode = Instantiate(treeNode);
                    leafnode.transform.SetParent(parent.GetComponent<NodeUpdater>().childrenGroup.transform, false);
                    parent.GetComponent<NodeUpdater>().childrenGroup.transform.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, (numChildren * 110) + 30);
                }
            }
            else {
                int childCount = parent.GetComponent<NodeUpdater>().childrenGroup.transform.childCount;
                difference = (childCount - 1) - numChildren;
                for(int i = 0; i < difference; i++) {
                    Destroy(parent.GetComponent<NodeUpdater>().childrenGroup.transform.GetChild(--childCount).gameObject);
                    parent.GetComponent<NodeUpdater>().childrenGroup.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, (numChildren * 110) + 30);
                }
            }
        }
    }

    public void AssignIDs() {

        int ID = 1;

        for (int i = 1; i < treePanel.transform.childCount; i++) {
            
            //for every group in the current level
            for (int j = 0; j < treePanel.transform.GetChild(i).childCount; j++) {
                //for every leaf in the current group
                for (int k = 1; k < treePanel.transform.GetChild(i).GetChild(j).childCount; k++) {
                    treePanel.transform.GetChild(i).GetChild(j).GetChild(k).GetChild(3).GetComponent<InputField>().text = ID.ToString();
                    ID++;
                }
            }
            
        }
    }

}
