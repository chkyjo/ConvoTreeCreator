using UnityEngine;

public class HorizontalTracker : MonoBehaviour {

    public int parentID;

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
            GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 75);
        }
    }
}
