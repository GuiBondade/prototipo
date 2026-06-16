using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class PaletteCreateVar : PaletteItem
{
    private CreateVariableUI createVarUI;

    protected override void SetupBlockData() {
        createVarUI = blocksFactory.InitializeCreateVariable(base.canvasAreasManager.canvasProgramming);
        base.rect = createVarUI.GetComponent<RectTransform>();
    }

    protected override void HandleBlockDrop(PointerEventData eventData) {
        // Se liberou fora da workspace, descarta; caso contrário mantém
        if (!RectTransformUtility.RectangleContainsScreenPoint(base.canvasAreasManager.visibleWorkspace, eventData.position, eventData.pressEventCamera)) {
            blocksFactory.CleanUpCreateVariable(createVarUI);
        }
        else {
            Debug.Log("dropou em workspace");
            createVarUI.transform.SetParent(canvasAreasManager.contentWorkspace, true);
        }
        createVarUI = null;
    }
}
