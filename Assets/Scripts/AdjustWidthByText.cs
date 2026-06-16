using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;

public class AdjustWidthByText : MonoBehaviour 
{
    //[SerializeField] public TMP_Text tmpText;
    [SerializeField] public GameObject fontWidthGO; // content e layout
    [SerializeField] public RectTransform targetRect;

    private List<LayoutElement> listLabelLayout = new List<LayoutElement>();

    /* void Start() 
    {
        AdjustWidth();
    } */

    public void AdjustWidth() {
        if (fontWidthGO == null || targetRect == null) { //|| tmpText == null) {
            return;
        }
        listLabelLayout.Clear();
        float biggestChildWidth = 0;
        for (int j = 0; j < fontWidthGO.transform.childCount; j++ ) {
            var child = fontWidthGO.transform.GetChild(j);
            float childWidth = 0;
            for (int k = 0; k < child.transform.childCount; k++ ) {
                var childElement = child.transform.GetChild(k); // checkmark e tmp text
                var childText = childElement.GetComponent<TMP_Text>();
                var childElementRect = childElement.GetComponent<RectTransform>();
                if  (childText != null) {
                    LayoutElement layout;
                    if (childText.TryGetComponent<LayoutElement>(out layout)) listLabelLayout.Add(layout);                  
                    childText.ForceMeshUpdate(); //precisa?
                    childWidth += childText.preferredWidth;    
                }
                LayoutRebuilder.ForceRebuildLayoutImmediate(childElementRect);
            }
            if (biggestChildWidth < childWidth) biggestChildWidth = childWidth;
        }
        foreach (LayoutElement layout in listLabelLayout) {
            layout.preferredWidth = biggestChildWidth;
        }
        var childPadding = fontWidthGO.transform.GetChild(0).GetComponent<HorizontalLayoutGroup>().padding;
        biggestChildWidth += childPadding.left + childPadding.right;
        var fontWidthRect = fontWidthGO.GetComponent<RectTransform>();
        fontWidthRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal,biggestChildWidth);
        var targetLayoutElem = targetRect.GetComponent<LayoutElement>();
        if (targetLayoutElem != null) targetLayoutElem.preferredWidth = biggestChildWidth;
        targetRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal,biggestChildWidth);
        //LayoutRebuilder.ForceRebuildLayoutImmediate(ancestorRect.GetComponent<RectTransform>());
        LayoutRebuilder.ForceRebuildLayoutImmediate(targetRect);
        //Debug.Log("Adjusted width");
    }
}