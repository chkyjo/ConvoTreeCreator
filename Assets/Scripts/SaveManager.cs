using UnityEngine;
using System.IO;
using UnityEngine.UI;
using System;

public class SaveManager : MonoBehaviour {

    public GameObject treePanel;
    public InputField saveTextFile;
    public InputField textFileField;
    public GameObject root;
    public GameObject group;
    public GameObject level;
    public GameObject leafNode;

    public struct ConvoNode {
        public int nodeID;
        public string playerResponse;
        public string responseSummary;
        public string characterResponse;
        public int childrenGroupID;
        public int[] playerOptions;
        public int effect;
        public int parameter1;
    }

    ConvoNode[] convoNodes = new ConvoNode[100];

    public void SaveConversation() {
        StreamWriter file = new StreamWriter(Application.persistentDataPath + "/" + saveTextFile.GetComponent<InputField>().text, true);

        for(int i = 0; i < treePanel.transform.childCount; i++) {
            if(i == 0) {
                //output root response
                file.WriteLine(treePanel.transform.GetChild(0).GetChild(0).GetComponent<InputField>().text);
                //if root has children print them
                if(treePanel.transform.GetChild(0).GetComponent<NodeUpdater>().childrenGroup != null) {
                    for (int n = 1; n < treePanel.transform.GetChild(0).GetComponent<NodeUpdater>().childrenGroup.transform.childCount; n++) {
                        file.WriteLine(":" + treePanel.transform.GetChild(0).GetComponent<NodeUpdater>().childrenGroup.transform.GetChild(n).GetChild(3).GetComponent<InputField>().text);
                    }
                }
            }
            else {
                //for every group in the current level
                for(int j = 0; j < treePanel.transform.GetChild(i).childCount; j++) {
                    //for every leaf in the current group
                    for(int k = 1; k < treePanel.transform.GetChild(i).GetChild(j).childCount; k++) {
                        GameObject node = treePanel.transform.GetChild(i).GetChild(j).GetChild(k).gameObject;
                        //description of player response
                        file.WriteLine("/" + node.transform.GetChild(0).GetComponent<InputField>().text);
                        //player response
                        file.WriteLine(")" + node.transform.GetChild(1).GetComponent<InputField>().text);
                        //character response
                        file.WriteLine("(" + node.transform.GetChild(2).GetComponent<InputField>().text);

                        if(node.transform.GetChild(6).GetComponent<InputField>().text != "0") {
                            //effect of speech + parameter if there is one
                            file.WriteLine(">" + node.transform.GetChild(5).GetComponent<Dropdown>().value + "-" + node.transform.GetChild(6).GetComponent<InputField>().text);
                        }
                        else {
                            //effect of speech
                            file.WriteLine(">" + node.transform.GetChild(5).GetComponent<Dropdown>().value);
                        }
                        
                        //display child indexes if there are any
                        if (node.GetComponent<NodeUpdater>().childrenGroup != null) {
                            file.WriteLine("}" + node.GetComponent<NodeUpdater>().childrenGroup.GetComponent<HorizontalTracker>().groupID);
                            for (int n = 1; n < node.GetComponent<NodeUpdater>().childrenGroup.transform.childCount; n++) {
                                file.WriteLine(":" + node.GetComponent<NodeUpdater>().childrenGroup.transform.GetChild(n).GetChild(3).GetComponent<InputField>().text);
                            }
                        }
                        //response IDs
                        file.WriteLine("-" + node.transform.GetChild(3).GetComponent<InputField>().text);
                    }
                }
            }
        }

        file.Close();
    }

    public void LoadConversation() {
        StreamReader file = new StreamReader(Application.persistentDataPath + "/" + textFileField.GetComponent<InputField>().text, true);

        int nodeIndex = 0;

        int numChildren = 0;
        int[] childIndexes = new int[5];
        string nextLine;

        //get root data
        convoNodes[0].nodeID = 0;
        convoNodes[0].characterResponse = file.ReadLine();

        while (!file.EndOfStream) {
            nextLine = file.ReadLine();
            if (nextLine.StartsWith(":")) {//if child index add to child indexes
                childIndexes[numChildren] = Convert.ToInt32(nextLine.TrimStart(':'));
                numChildren++;
            }
            else if (numChildren != 0) {//if not a child index and numchildren is not 0 add children to character response
                convoNodes[nodeIndex].playerOptions = new int[numChildren];
                for(int i = 0; i < numChildren; i++) {
                    convoNodes[nodeIndex].playerOptions[i] = childIndexes[i];
                }
                if (nodeIndex == 0) {//if root also add to the index
                    nodeIndex++;
                }
                numChildren = 0;
            }

            if (nextLine.StartsWith("/")) {//if summary add string
                convoNodes[nodeIndex].responseSummary = nextLine.TrimStart('/');
            }

            if (nextLine.StartsWith(")")) {//if player response add the string
                convoNodes[nodeIndex].playerResponse = nextLine.TrimStart(')');
            }

            if (nextLine.StartsWith("(")) {//if character response add the string
                convoNodes[nodeIndex].characterResponse = nextLine.TrimStart('(');
            }

            if (nextLine.StartsWith(">")) {//if effect index
                string[] splitEffect;
                splitEffect = nextLine.TrimStart('>').Split('-');
                if(splitEffect.Length == 2) {
                    convoNodes[nodeIndex].effect = Convert.ToInt16(splitEffect[0]);
                    convoNodes[nodeIndex].parameter1 = Convert.ToInt16(splitEffect[1]);
                }
                else {
                    convoNodes[nodeIndex].effect = Convert.ToInt16(splitEffect[0]);
                    convoNodes[nodeIndex].parameter1 = 0;
                }
            }

            if (nextLine.StartsWith("}")) {//if children group ID
                convoNodes[nodeIndex].childrenGroupID = Convert.ToInt16(nextLine.TrimStart('}'));
            }

            if (nextLine.StartsWith("-")) {
                convoNodes[nodeIndex].nodeID = Convert.ToInt16(nextLine.TrimStart('-'));

                nodeIndex++;
            }
        }
    }

    public void DisplayTree() {

        root.transform.GetChild(0).GetComponent<InputField>().text = convoNodes[0].characterResponse;
        int numRootChildren = convoNodes[0].playerOptions.Length;
        root.transform.GetChild(2).GetComponent<Slider>().value = numRootChildren;
        bool hasChildren;

        if (numRootChildren > 0) {
            hasChildren = true;
            for (int i = 1; i <= numRootChildren; i++) {
                GameObject node = treePanel.transform.GetChild(1).GetChild(0).GetChild(i).gameObject;
                ConvoNode convoNode = convoNodes[convoNodes[0].playerOptions[i-1]];
                node.transform.GetChild(0).GetComponent<InputField>().text = convoNode.responseSummary;
                node.transform.GetChild(1).GetComponent<InputField>().text = convoNode.playerResponse;
                node.transform.GetChild(2).GetComponent<InputField>().text = convoNode.characterResponse;
                node.transform.GetChild(3).GetComponent<InputField>().text = convoNode.nodeID.ToString();
                
                if (convoNode.playerOptions != null) {
                    //if we don't link to an already existing node
                    if (!LinkNodeToGroup(node, convoNode.childrenGroupID)) {
                        //create new group with empty children
                        node.transform.GetChild(4).GetComponent<Slider>().value = convoNode.playerOptions.Length;
                        //assign groupID
                        node.GetComponent<NodeUpdater>().childrenGroup.GetComponent<HorizontalTracker>().groupID = convoNode.childrenGroupID;

                    }
                }
                
                node.transform.GetChild(5).GetComponent<Dropdown>().value = convoNode.effect;
                node.transform.GetChild(6).GetComponent<InputField>().text = convoNode.parameter1.ToString();
            }
        }
        else {
            hasChildren = true;
        }

        int nodeIndex = 1;
        int levelIndex = 1;

        while (hasChildren == true) {
            //for every group in current level
            for (int group = 0; group < treePanel.transform.GetChild(levelIndex).childCount; group++) {
                //for every leaf in group
                for (int leaf = 1; leaf < treePanel.transform.GetChild(levelIndex).GetChild(group).childCount; leaf++) {
                    //instantiate children if it has any
                    if (treePanel.transform.GetChild(levelIndex).GetChild(group).GetChild(leaf).GetComponent<NodeUpdater>().childrenGroup != null) {
                        GameObject childrenGroup = treePanel.transform.GetChild(levelIndex).GetChild(group).GetChild(leaf).GetComponent<NodeUpdater>().childrenGroup;
                        GameObject currentChild;
                        //for every child
                        for (int child = 0; child < convoNodes[nodeIndex].playerOptions.Length; child++) {
                            currentChild = childrenGroup.transform.GetChild(child + 1).gameObject;
                            ConvoNode convoNode = convoNodes[convoNodes[nodeIndex].playerOptions[child]];
                            
                            currentChild.transform.GetChild(0).GetComponent<InputField>().text = convoNode.responseSummary;
                            currentChild.transform.GetChild(1).GetComponent<InputField>().text = convoNode.playerResponse;
                            currentChild.transform.GetChild(2).GetComponent<InputField>().text = convoNode.characterResponse;
                            currentChild.transform.GetChild(3).GetComponent<InputField>().text = convoNode.nodeID.ToString();
                            if (convoNode.playerOptions != null) {
                                //if we don't link to an already existing node
                                if (!LinkNodeToGroup(currentChild, convoNode.childrenGroupID)) {
                                    //create empty children
                                    currentChild.transform.GetChild(4).GetComponent<Slider>().value = convoNode.playerOptions.Length;
                                    //assign groupID
                                    currentChild.GetComponent<NodeUpdater>().childrenGroup.GetComponent<HorizontalTracker>().groupID = convoNode.childrenGroupID;
                                }
                            }
                            currentChild.transform.GetChild(5).GetComponent<Dropdown>().value = convoNode.effect;
                            currentChild.transform.GetChild(6).GetComponent<InputField>().text = convoNode.parameter1.ToString();
                        }
                    }
                    nodeIndex++;
                }
            }
            
            if(treePanel.transform.childCount == levelIndex + 1) {
                hasChildren = false;
            }
            levelIndex++;
        }

    }

    public bool LinkNodeToGroup(GameObject node, int groupID) {


        Debug.Log("Attempting to link group " + group.GetComponent<HorizontalTracker>().groupID);

        for(int i = 1; i < treePanel.transform.childCount; i++) {
            for(int j = 0; j < treePanel.transform.GetChild(i).childCount; j++) {
                if(treePanel.transform.GetChild(i).GetChild(j).GetComponent<HorizontalTracker>().groupID == groupID) {
                    GameObject group = treePanel.transform.GetChild(i).GetChild(j).gameObject;
                    node.GetComponent<NodeUpdater>().childrenGroup = group;
                    node.transform.GetChild(4).GetComponent<Slider>().value = group.transform.childCount - 1;
                    node.GetComponent<Image>().color = group.GetComponent<Image>().color;
                    node.GetComponent<NodeUpdater>().groupID = group.GetComponent<HorizontalTracker>().groupID;
                    Debug.Log("Linked");
                    return true;
                }
            }
        }

        return false;
    }
}
