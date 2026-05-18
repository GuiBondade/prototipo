using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;

public class AdjustWidthByText : MonoBehaviour 
{
    //[SerializeField] public TMP_Text tmpText;
    [SerializeField] public GameObject fontWidthGO; // content e layout
    [SerializeField] public RectTransform targetRect;

    /* void Start() 
    {
        AdjustWidth();
    } */

    public void AdjustWidth() {
        if (fontWidthGO == null || targetRect == null) { //|| tmpText == null) {
            return;
        }
        float biggestChildWidth = 0;
        for (int j = 0; j < fontWidthGO.transform.childCount; j++ ) {
            var child = fontWidthGO.transform.GetChild(j);
            float childWidth = 0;
            for (int k = 0; k < child.transform.childCount; k++ ) {
                var childElement = child.transform.GetChild(k); // checkmark e tmp text
                var childText = childElement.GetComponent<TMP_Text>();
                var childElementRect = childElement.GetComponent<RectTransform>();
                if  (childText != null) {
                    childText.ForceMeshUpdate(); //precisa?
                    childWidth += childText.preferredWidth;    
                } else {
                    childWidth += childElementRect.rect.width;
                }
                LayoutRebuilder.ForceRebuildLayoutImmediate(childElementRect);
            }
            var childPadding = child.GetComponent<HorizontalLayoutGroup>().padding;
            childWidth += childPadding.left + childPadding.right;
            if (biggestChildWidth < childWidth) biggestChildWidth = childWidth;
        }
        for (int i = 0; i < fontWidthGO.transform.childCount; i++) {
            var childRect = fontWidthGO.transform.GetChild(i).GetComponent<RectTransform>();
            childRect.GetComponent<LayoutElement>().preferredWidth = biggestChildWidth;
            LayoutRebuilder.ForceRebuildLayoutImmediate(childRect);
        }
        var fontWidthRect = fontWidthGO.GetComponent<RectTransform>();
        fontWidthRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal,biggestChildWidth);
        LayoutRebuilder.ForceRebuildLayoutImmediate(fontWidthRect);
        targetRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal,biggestChildWidth);
        // targetRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, targetRect);
    }
}