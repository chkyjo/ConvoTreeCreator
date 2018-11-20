using UnityEngine;
using System.IO;
using UnityEngine.UI;
using System;
using System.Collections;

public class SaveManager : MonoBehaviour {

    public GameObject treePanel;
    public InputField saveTextFile;
    public InputField textFileField;

    public GameObject playerStart;
    public GameObject NPCStart;

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

        if(treePanel.transform.childCount < 2) {
            Debug.Log("Root must have at least one child");
            return;
        }

        StreamWriter file = new StreamWriter(Application.persistentDataPath + "/SurvivalGame/" + saveTextFile.GetComponent<InputField>().text, true);

        for(int i = 0; i < treePanel.transform.childCount; i++) {
            if(i == 0) {
                //output root response
                if(treePanel.transform.GetChild(0).transform.childCount == 3) {
                    file.WriteLine("(" + treePanel.transform.GetChild(0).GetChild(0).GetComponent<InputField>().text);

                    //if root has children print them
                    if (treePanel.transform.GetChild(0).GetComponent<NodeUpdater>().childrenGroup != null) {
                        for (int n = 1; n < treePanel.transform.GetChild(0).GetComponent<NodeUpdater>().childrenGroup.transform.childCount; n++) {
                            file.WriteLine(":" + treePanel.transform.GetChild(0).GetComponent<NodeUpdater>().childrenGroup.transform.GetChild(n).GetChild(3).GetComponent<InputField>().text);
                        }
                    }
                }
                else {
                    GameObject node = treePanel.transform.GetChild(0).gameObject;
                    //description of player response
                    file.WriteLine("/" + node.transform.GetChild(0).GetComponent<InputField>().text);
                    //player response
                    file.WriteLine(")" + node.transform.GetChild(1).GetComponent<InputField>().text);
                    //character response
                    file.WriteLine("(" + node.transform.GetChild(2).GetComponent<InputField>().text);

                    if (node.transform.GetChild(6).GetComponent<InputField>().text != "0") {
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
        StreamReader file = new StreamReader(Application.persistentDataPath + "/SurvivalGame/" + textFileField.GetComponent<InputField>().text, true);

        int nodeIndex = 0;

        int numChildren = 0;
        int[] childIndexes = new int[5];
        string nextLine;

        //get root data
        nextLine = file.ReadLine();

        //if the first line is a character response
        if (nextLine.StartsWith("(")) {
            convoNodes[0].nodeID = 0;
            convoNodes[0].characterResponse = nextLine.TrimStart('(');
            convoNodes[0].childrenGroupID = Convert.ToInt16(file.ReadLine().TrimStart('}'));
            nextLine = file.ReadLine();
            while (nextLine.StartsWith(":")) {
                AddPlayerOption(nodeIndex, Convert.ToInt16(nextLine.TrimStart(':')));
                nextLine = file.ReadLine();
            }
            nextLine = file.ReadLine();
            nodeIndex++;
        }


        while (!file.EndOfStream) {
            convoNodes[nodeIndex].responseSummary = nextLine.TrimStart('/');
            convoNodes[nodeIndex].playerResponse = file.ReadLine().TrimStart(')');
            convoNodes[nodeIndex].characterResponse = file.ReadLine().TrimStart('(');
            string[] splitEffect = file.ReadLine().TrimStart('>').Split('-');
            if (splitEffect.Length == 2) {
                convoNodes[nodeIndex].effect = Convert.ToInt16(splitEffect[0]);
                convoNodes[nodeIndex].parameter1 = Convert.ToInt16(splitEffect[1]);
            }
            else {
                convoNodes[nodeIndex].effect = Convert.ToInt16(splitEffect[0]);
            }
            nextLine = file.ReadLine();
            if (nextLine.StartsWith("}")) {
                convoNodes[nodeIndex].childrenGroupID = Convert.ToInt16(nextLine.TrimStart('}'));
                nextLine = file.ReadLine();
                while (nextLine.StartsWith(":")) {
                    AddPlayerOption(nodeIndex, Convert.ToInt16(nextLine.TrimStart(':')));
                    nextLine = file.ReadLine();
                }
            }
            convoNodes[nodeIndex].nodeID = Convert.ToInt16(nextLine.TrimStart('-'));
            nextLine = file.ReadLine();
            nodeIndex++;
        }
    }

    public void DisplayTree() {
        StartCoroutine(_DisplayTree());
    }

    IEnumerator _DisplayTree() {
        //clear the tree panel
        for (int i = 0; i < treePanel.transform.childCount; i++) {
            Destroy(treePanel.transform.GetChild(i).gameObject);
        }

        GameObject root;
        //if the root node is a character response
        if (convoNodes[0].playerResponse == "") {
            root = Instantiate(NPCStart);
            root.transform.SetParent(treePanel.transform, false);
            root.GetComponent<NodeUpdater>().level = 0;
            root.transform.GetChild(0).GetComponent<InputField>().text = convoNodes[0].characterResponse;
            root.transform.GetChild(1).GetComponent<Text>().text = "0";
            yield return new WaitForEndOfFrame();
            root.transform.GetChild(2).GetComponent<Slider>().value = convoNodes[0].playerOptions.Length;

        }
        else {
            root = Instantiate(playerStart);
            root.transform.SetParent(treePanel.transform, false);
            root.GetComponent<NodeUpdater>().level = 0;
            root.transform.GetChild(0).GetComponent<InputField>().text = convoNodes[0].responseSummary;
            root.transform.GetChild(1).GetComponent<InputField>().text = convoNodes[0].playerResponse;
            root.transform.GetChild(2).GetComponent<InputField>().text = convoNodes[0].characterResponse;
            root.transform.GetChild(3).GetComponent<InputField>().text = convoNodes[0].nodeID.ToString();
            yield return new WaitForEndOfFrame();
            root.transform.GetChild(4).GetComponent<Slider>().value = convoNodes[0].playerOptions.Length;
            root.transform.GetChild(5).GetComponent<Dropdown>().value = convoNodes[0].effect;
            root.transform.GetChild(6).GetComponent<InputField>().text = convoNodes[0].parameter1.ToString();
        }

        bool hasChildren;

        //for each child in the newly created group of children fill the values
        for (int i = 1; i <= convoNodes[0].playerOptions.Length; i++) {
            GameObject node = treePanel.transform.GetChild(1).GetChild(0).GetChild(i).gameObject;
            ConvoNode convoNode = convoNodes[convoNodes[0].playerOptions[i - 1]];
            Debug.Log(treePanel.transform.GetChild(1).name);
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

        int nodeIndex = 1;
        int levelIndex = 1;

        if(treePanel.transform.childCount == 3) {
            hasChildren = true;
        }
        else {
            hasChildren = false;
        }

        //while the previous level created a new level
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

            if (treePanel.transform.childCount == levelIndex + 1) {
                hasChildren = false;
            }
            levelIndex++;
        }
    }

    public bool LinkNodeToGroup(GameObject node, int groupID) {


        //Debug.Log("Attempting to link group " + group.GetComponent<HorizontalTracker>().groupID);

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

    void AddPlayerOption(int nodeIndex, int childID) {
        
        if(convoNodes[nodeIndex].playerOptions != null) {
            int oldLength = convoNodes[nodeIndex].playerOptions.Length;
            int[] oldList = convoNodes[nodeIndex].playerOptions;
            convoNodes[nodeIndex].playerOptions = new int[oldLength + 1];
            for(int i = 0; i < oldLength; i++) {
                convoNodes[nodeIndex].playerOptions[i] = oldList[i];
            }

            convoNodes[nodeIndex].playerOptions[oldLength] = childID;
        }
        else {
            convoNodes[nodeIndex].playerOptions = new int[1] { childID };
        }
        
    }
}
