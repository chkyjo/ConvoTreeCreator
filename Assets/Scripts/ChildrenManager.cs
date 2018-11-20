using UnityEngine;
using UnityEngine.UI;

public class ChildrenManager : MonoBehaviour {

    public GameObject treePanel;
    public GameObject horizontalPanel;
    public GameObject treeNode;
    public GameObject NPCRoot;
    public GameObject levelPanel;
    public GameObject nodeToLink;

    Vector2 offset = new Vector2(1, 0);


    public void UpdateChildren(int parentLevel, GameObject parent) {
        int numChildren = (int)parent.GetComponent<NodeUpdater>().childrenSlider.value;
        if(parent.GetComponent<NodeUpdater>().childrenGroup != null) {
            if (numChildren == parent.GetComponent<NodeUpdater>().childrenGroup.transform.childCount - 1) {
                return;
            }
        }
        
        if (numChildren == 0) {
            Debug.Log("numChildren is 0");
            //if the slider was updated automatically because it was another parent of a childrenGroup then the childrenGroup is already destroyed
            if (parent.GetComponent<NodeUpdater>().childrenGroup == null) {
                Debug.Log("childrenGroup is null");
                return;
            }

            //update the other parents of the childrenGroup
            for (int i = 0; i < parent.GetComponent<NodeUpdater>().childrenGroup.GetComponent<HorizontalTracker>().parents.Count; i++) {
                parent.GetComponent<NodeUpdater>().childrenGroup.GetComponent<HorizontalTracker>().parents[i].gameObject.transform.GetChild(4).GetComponent<Slider>().value = numChildren;
            }

            //if there are still child nodes in the group
            if (parent.GetComponent<NodeUpdater>().childrenGroup.transform.childCount > 1) {
                for (int i = 1; i < parent.GetComponent<NodeUpdater>().childrenGroup.transform.childCount; i++) {
                    //if it has a childrenGroup
                    if (parent.GetComponent<NodeUpdater>().childrenGroup.transform.GetChild(i).GetComponent<NodeUpdater>().childrenGroup != null) {
                        //remove itself as a parent from that childrenGroup
                        parent.GetComponent<NodeUpdater>().childrenGroup.transform.GetChild(i).GetComponent<NodeUpdater>().childrenGroup.GetComponent<HorizontalTracker>().RemoveParent(parent.GetComponent<NodeUpdater>().childrenGroup.transform.GetChild(i).GetComponent<NodeUpdater>());
                    }
                }
            }

            //if it was the only group in the level
            if (parent.GetComponent<NodeUpdater>().childrenGroup.GetComponent<HorizontalTracker>().level.transform.childCount <= 1) {
                Debug.Log("Destroying level");
                Destroy(parent.GetComponent<NodeUpdater>().childrenGroup.GetComponent<HorizontalTracker>().level);
            }
            else {//only destroy the group
                Destroy(parent.GetComponent<NodeUpdater>().childrenGroup);
            }
            
        }
        else {//The number of desired children is not 0
            //if the node calling the function is on the last level then create a new level
            if (treePanel.transform.childCount == parentLevel + 1) {
                var newLevel = Instantiate(levelPanel);
                newLevel.transform.SetParent(treePanel.transform, false);
            }
            //if there is no group create one
            if (parent.GetComponent<NodeUpdater>().childrenGroup == null) {
                var group = Instantiate(horizontalPanel);
                group.GetComponent<Image>().color = parent.GetComponent<NodeUpdater>().childrenColor;
                
                group.transform.SetParent(treePanel.transform.GetChild(parentLevel + 1).transform, false);
                group.GetComponent<HorizontalTracker>().level = treePanel.transform.GetChild(parentLevel + 1).gameObject;
                parent.GetComponent<NodeUpdater>().childrenGroup = group;
                //Add the parent to the groups parents
                parent.GetComponent<NodeUpdater>().SetSelfAsParent();
                
            }

            int difference;
            //if the desired number of children is greater than the number of children
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
                    //if the node to remove has a children group, remove it as a parent from the childrenGroup
                    if(parent.GetComponent<NodeUpdater>().childrenGroup.transform.GetChild(childCount - 1).GetComponent<NodeUpdater>().childrenGroup != null) {
                        parent.GetComponent<NodeUpdater>().childrenGroup.transform.GetChild(childCount - 1).GetComponent<NodeUpdater>().childrenGroup.GetComponent<HorizontalTracker>().RemoveParent(parent.GetComponent<NodeUpdater>().childrenGroup.transform.GetChild(childCount - 1).GetComponent<NodeUpdater>());
                    }
                    Destroy(parent.GetComponent<NodeUpdater>().childrenGroup.transform.GetChild(--childCount).gameObject);
                    parent.GetComponent<NodeUpdater>().childrenGroup.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, (numChildren * 110) + 30);
                }
            }

            //update the other parents of the childrenGroup
            for(int i = 0; i < parent.GetComponent<NodeUpdater>().childrenGroup.GetComponent<HorizontalTracker>().parents.Count; i++) {
                parent.GetComponent<NodeUpdater>().childrenGroup.GetComponent<HorizontalTracker>().parents[i].gameObject.transform.GetChild(4).GetComponent<Slider>().value = numChildren;
            }

            AssignIDs();

        }
    }

    public void AssignIDs() {

        int nodeID = 1;
        int groupID = 0;

        for (int i = 1; i < treePanel.transform.childCount; i++) {
            //for every group in the current level
            for (int j = 0; j < treePanel.transform.GetChild(i).childCount; j++) {
                treePanel.transform.GetChild(i).GetChild(j).GetComponent<HorizontalTracker>().groupID = groupID;
                treePanel.transform.GetChild(i).GetChild(j).GetChild(0).GetChild(2).GetComponent<Text>().text = groupID.ToString();
                groupID++;
                //for every leaf in the current group
                for (int k = 1; k < treePanel.transform.GetChild(i).GetChild(j).childCount; k++) {
                    treePanel.transform.GetChild(i).GetChild(j).GetChild(k).GetChild(3).GetComponent<InputField>().text = nodeID.ToString();
                    nodeID++;
                }
            }
        }
    }

    //Links the group and the node that were selected for the link
    public void SetGroup(GameObject groupPanel) {
        nodeToLink.GetComponent<NodeUpdater>().childrenGroup = groupPanel;
        //updates the value of the slider
        nodeToLink.transform.GetChild(4).GetComponent<Slider>().value = groupPanel.transform.childCount - 1;
        nodeToLink.GetComponent<Image>().color = groupPanel.GetComponent<Image>().color;
        nodeToLink.GetComponent<NodeUpdater>().groupID = groupPanel.GetComponent<HorizontalTracker>().groupID;
        groupPanel.GetComponent<HorizontalTracker>().AddParent(nodeToLink.GetComponent<NodeUpdater>());

        nodeToLink = null;
    }

    public void UpdateConversationStart(int dropDownValue) {
        for(int i = 0; i < treePanel.transform.childCount; i++) {
            Destroy(treePanel.transform.GetChild(i).gameObject);
        }

        if(dropDownValue == 0) {
            GameObject root = Instantiate(treeNode);
            root.transform.SetParent(treePanel.transform, false);
            root.GetComponent<NodeUpdater>().level = 0;
            root.transform.GetChild(3).GetComponent<InputField>().text = "0";
        }
        else {
            GameObject root = Instantiate(NPCRoot);
            root.transform.SetParent(treePanel.transform, false);
            root.GetComponent<NodeUpdater>().level = 0;
            root.transform.GetChild(1).GetComponent<Text>().text = "0";
        }

    }

    public void DisableAllLinkButtons() {
        treePanel.transform.GetChild(0).GetChild(7).GetComponent<Button>().interactable = false;
        for (int level = 1; level < treePanel.transform.childCount; level++) {
            for (int group = 0; group < treePanel.transform.GetChild(level).childCount; group++) {
                for (int child = 1; child < treePanel.transform.GetChild(level).GetChild(group).childCount; child++) {
                    treePanel.transform.GetChild(level).GetChild(group).GetChild(child).GetChild(7).GetComponent<Button>().interactable = false;
                }
            }
        }
    }

    public void DisableGroupLinkButtons() {
        for (int level = 1; level < treePanel.transform.childCount; level++) {
            for (int group = 0; group < treePanel.transform.GetChild(level).childCount; group++) {
                treePanel.transform.GetChild(level).GetChild(group).GetChild(0).GetChild(4).GetComponent<Button>().interactable = false;
            }
        }
    }

    public void EnableAllLinkButtons() {
        treePanel.transform.GetChild(0).GetChild(7).GetComponent<Button>().interactable = true;
        for (int level = 1; level < treePanel.transform.childCount; level++) {
            for (int group = 0; group < treePanel.transform.GetChild(level).childCount; group++) {
                for (int child = 1; child < treePanel.transform.GetChild(level).GetChild(group).childCount; child++) {
                    treePanel.transform.GetChild(level).GetChild(group).GetChild(child).GetChild(7).GetComponent<Button>().interactable = true;
                }
            }
        }
    }

    public void EnableGroupLinkButtons() {
        for (int level = 1; level < treePanel.transform.childCount; level++) {
            for (int group = 0; group < treePanel.transform.GetChild(level).childCount; group++) {
                treePanel.transform.GetChild(level).GetChild(group).GetChild(0).GetChild(4).GetComponent<Button>().interactable = true;
            }
        }
    }

}
