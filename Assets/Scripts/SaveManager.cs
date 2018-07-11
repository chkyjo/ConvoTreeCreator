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

    public struct CharacterResponse {
        public int responseID;
        public string response;
        public int[] playerOptions;
    }
    public struct PlayerResponse {
        public int responseID;
        public string response;
        public int characterResponseID;
    }

    CharacterResponse[] characterResponses = new CharacterResponse[100];
    PlayerResponse[] playerResponses = new PlayerResponse[100];

    public void SaveConversation() {
        StreamWriter file = new StreamWriter(Application.persistentDataPath + "/" + saveTextFile.GetComponent<InputField>().text, true);

        for(int i = 0; i < treePanel.transform.childCount; i++) {
            if(i == 0) {
                //output root response
                file.WriteLine(treePanel.transform.GetChild(0).GetChild(0).GetComponent<InputField>().text);
                //if root has children print them
                if(treePanel.transform.GetChild(0).GetComponent<NodeUpdater>().childrenGroup != null) {
                    for (int n = 0; n < treePanel.transform.GetChild(0).GetComponent<NodeUpdater>().childrenGroup.transform.childCount; n++) {
                        file.WriteLine(":" + treePanel.transform.GetChild(0).GetComponent<NodeUpdater>().childrenGroup.transform.GetChild(n).GetChild(2).GetComponent<InputField>().text);
                    }
                }
            }
            else {
                //for every group in the current level
                for(int j = 0; j < treePanel.transform.GetChild(i).childCount; j++) {
                    //for every leaf in the current group
                    for(int k = 0; k < treePanel.transform.GetChild(i).GetChild(j).childCount; k++) {
                        GameObject node = treePanel.transform.GetChild(i).GetChild(j).GetChild(k).gameObject;
                        //player response
                        file.WriteLine(")" + node.transform.GetChild(0).GetComponent<InputField>().text);
                        //character response
                        file.WriteLine("(" + node.transform.GetChild(1).GetComponent<InputField>().text);
                        //display child indexes if there are any
                        if (node.GetComponent<NodeUpdater>().childrenGroup != null) {
                            Debug.Log("Children group for child " + k + " in group " + j + " and level " + i + " is not null");
                            for (int n = 0; n < node.GetComponent<NodeUpdater>().childrenGroup.transform.childCount; n++) {
                                file.WriteLine(":" + node.GetComponent<NodeUpdater>().childrenGroup.transform.GetChild(n).GetChild(2).GetComponent<InputField>().text);
                            }
                        }
                        //response IDs
                        file.WriteLine("-" + node.transform.GetChild(2).GetComponent<InputField>().text + "," + node.transform.GetChild(3).GetComponent<InputField>().text);
                    }
                }
            }
        }

        file.Close();
    }

    public void LoadConversation() {
        StreamReader file = new StreamReader(Application.persistentDataPath + "/" + textFileField.GetComponent<InputField>().text, true);

        int characterResponseIndex = 0;
        int playerResponseIndex = 0;

        int numChildren = 0;
        int[] childIndexes = new int[5];
        string nextLine;

        //get root data
        string rootString = file.ReadLine();
        characterResponses[0].responseID = 0;
        characterResponses[0].response = rootString.TrimStart('(');

        while (!file.EndOfStream) {
            nextLine = file.ReadLine();
            if (nextLine.StartsWith(":")) {//if child index add to child indexes
                childIndexes[numChildren] = Convert.ToInt16(nextLine.TrimStart(':'));
                numChildren++;
            }
            else if (numChildren != 0) {//if not a child index and numchildren is not 0 add children to character response
                characterResponses[characterResponseIndex].playerOptions = new int[numChildren];
                for(int i = 0; i < numChildren; i++) {
                    characterResponses[characterResponseIndex].playerOptions[i] = childIndexes[i];
                }
                if (characterResponseIndex == 0) {//if root also up the index
                    characterResponseIndex++;
                }
                numChildren = 0;
            }

            if (nextLine.StartsWith(")")) {//if player response add the string
                playerResponses[playerResponseIndex].response = nextLine.TrimStart(')');
            }

            if (nextLine.StartsWith("(")) {//if character response add the string
                characterResponses[characterResponseIndex].response = nextLine.TrimStart('(');
            }

            if (nextLine.StartsWith("-")) {//if IDs add Ids and up index for both
                string[] splitByComma = nextLine.TrimStart('-').Split(',');
                playerResponses[playerResponseIndex].responseID = Convert.ToInt16(splitByComma[0]);
                playerResponses[playerResponseIndex].characterResponseID = playerResponses[playerResponseIndex].responseID + 1;
                characterResponses[characterResponseIndex].responseID = Convert.ToInt16(splitByComma[1]);

                playerResponseIndex++;
                characterResponseIndex++;
            }
        }
    }

    public void DisplayTree() {

        root.transform.GetChild(0).GetComponent<InputField>().text = characterResponses[0].response;
        int numRootChildren = characterResponses[0].playerOptions.Length;
        root.transform.GetChild(2).GetComponent<Slider>().value = numRootChildren;
        bool hasChildren;

        if (numRootChildren > 0) {
            hasChildren = true;
            for (int i = 0; i < numRootChildren; i++) {
                GameObject node = treePanel.transform.GetChild(1).GetChild(0).GetChild(i).gameObject;
                node.transform.GetChild(0).GetComponent<InputField>().text = playerResponses[characterResponses[0].playerOptions[i]].response;
                node.transform.GetChild(2).GetComponent<InputField>().text = playerResponses[characterResponses[0].playerOptions[i]].responseID.ToString();
            }
        }
        else {
            hasChildren = true;
        }

        int characterResponseIndex = 1;
        int levelIndex = 1;

        while (hasChildren == true) {
            //for every group in current level
            for (int i = 0; i < treePanel.transform.GetChild(levelIndex).childCount; i++) {
                //for ever leaf in group
                for (int j = 0; j < treePanel.transform.GetChild(levelIndex).GetChild(i).childCount; j++) {
                    GameObject currentNode = treePanel.transform.GetChild(levelIndex).GetChild(i).GetChild(j).gameObject;
                    currentNode.transform.GetChild(1).GetComponent<InputField>().text = characterResponses[characterResponseIndex].response;
                    currentNode.transform.GetChild(3).GetComponent<InputField>().text = characterResponses[characterResponseIndex].responseID.ToString();
                    if(characterResponses[characterResponseIndex].playerOptions != null) {
                        currentNode.transform.GetChild(4).GetComponent<Slider>().value = characterResponses[characterResponseIndex].playerOptions.Length;
                    }        

                    if (currentNode.transform.GetChild(4).GetComponent<Slider>().value != 0) {
                        for (int n = 0; n < currentNode.transform.GetChild(4).GetComponent<Slider>().value; n++) {
                            currentNode.GetComponent<NodeUpdater>().childrenGroup.transform.GetChild(n).GetChild(0).GetComponent<InputField>().text = playerResponses[characterResponses[characterResponseIndex].playerOptions[n]].response;
                            currentNode.GetComponent<NodeUpdater>().childrenGroup.transform.GetChild(n).GetChild(2).GetComponent<InputField>().text = playerResponses[characterResponses[characterResponseIndex].playerOptions[n]].responseID.ToString();
                        }
                    }
                    characterResponseIndex++;
                }
            }
            
            if(treePanel.transform.childCount == levelIndex + 1) {
                hasChildren = false;
            }
            levelIndex++;
        }

    }
}
