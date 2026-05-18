using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BlockSlot : MonoBehaviour, IDropHandler
{
    public enum SlotType { Next, Body }
    public SlotType slotType;

    public BlockUI currentBlock;

    // So roda quando solta em cima de um bloco (GameObject que nao o workspace, que esta sendo desconsiderado)
    // roda antes de OnEndDrag do DraggableBlock
    public void OnDrop(PointerEventData eventData) {
        var draggedObj = eventData.pointerDrag;
        if (draggedObj == null) return;

        var draggedBlock = draggedObj.GetComponent<BlockUI>();
        if (draggedBlock == null) return;
        
        var rect = draggedObj.GetComponent<RectTransform>();

        // Descobre último da linha dropada
        BlockUI tail = draggedBlock.GetTail();

        // Se houver bloco nesse slot, move-o para o tail da linha arrastada
        BlockUI existingBlock = null;
        if (transform.childCount == 1) // se tem 1 filho (nunca vai ter mais que 1, é pra saber se tem ou não)
        {
            //print(transform); // SlotNext; print(transform.GetChild(0)); // blockPrefabNext;
            existingBlock = transform.GetChild(0).GetComponent<BlockUI>();
        // if (existingBlock != null) {}, [so serviria se slot pudesse ter filho sem blockUI, mas nao tem como]
            RectTransform nextSlotTransform = tail.slotNext.GetComponent<RectTransform>();
            existingBlock.transform.SetParent(nextSlotTransform, false);
            existingBlock.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
            //existingBlock.transform.localPosition = new Vector3(0, 0, 0); // força z = 0 (da descomentar, so noa settar y e x, so setar z = 0)
            //existingBlock.GetComponentInParent<RectTransform>().pivot = new Vector2(0.5f, 1.5f);
        }

        // Parentar a linha arrastada neste slot
        draggedObj.transform.SetParent(transform, false);
        rect.anchoredPosition = Vector2.zero;
        draggedObj.transform.localPosition = new Vector3(rect.localPosition.x, rect.localPosition.y, 0f); // força z = 0

        rect.anchorMin = new Vector2(0, 0);
        rect.anchorMax = new Vector2(0, 0);

        rect.pivot = new Vector2(0f,1f);

        rect.localScale = transform.localScale;
    
        // Determina o contexto do parent (herdar ancestors do parentBlock)
        BlockUI parentBlock = GetComponentInParent<BlockUI>();
        //print("parentBlock: " + parentBlock);

        if (slotType == SlotType.Body) { 
            //Debug.Log("offset de bloco");
            rect.anchoredPosition = new Vector2(parentBlock.defaultSpacerHeight, 0);
            //if (existingBlock == null) parentBlock.bodySpacer.sizeDelta -= new Vector2(0, 30);
        }

        List<BlockUI> parentAncestorList = parentBlock != null ? new List<BlockUI>(parentBlock.bodyAncestors) : null;
        BlockUI parentBodyOwner = (parentBlock != null && slotType == SlotType.Body) ? parentBlock : null;

        // Aplica as ancestrais recursivamente na cadeia arrastada
        draggedBlock.AssignBodyAncestorsRecursive(parentAncestorList, parentBodyOwner);

        // Recupera dragHeight cache (espera-se que DraggableBlock tenha setado)
        var draggable = draggedObj.GetComponent<DraggableBlock>();
        float dragHeight = 0f;
        if (draggable != null)
            dragHeight = draggable.dragHeightCache;

        // Aplica ajuste de altura em todos os ancestors do bloco arrastado (se houver ancestrais)
        if (parentBlock != null && draggedBlock.bodyAncestors != null && draggedBlock.bodyAncestors.Count > 0)
        {
            // soma (pode ser positivo)
            //Debug.Log($"Ajustando altura de: {draggedBlock.bodyAncestors.Count} ancestrais do bloco arrastado em: {dragHeight}");
            draggedBlock.AdjustBodySpacers(dragHeight);
            if (slotType == SlotType.Body && existingBlock == null) draggedBlock.AdjustBodySpacers(-(draggedBlock.defaultSpacerHeight));
        }

        // limpa cache do draggable
        if (draggable != null)
            draggable.dragHeightCache = 0f;

        // atualiza referência local
        currentBlock = draggedBlock;
    }
}
