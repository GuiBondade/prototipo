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

        // Descobre o bloco que já está dentro deste slot (se houver)
        BlockUI existingBlock = null;
        if (transform.childCount > 1)
        {
            // Itera de baixo para cima, ignorando RaycastTarget ou outros elementos
            for (int i = transform.childCount - 1; i >= 0; i--)
            {
                var candidate = transform.GetChild(i).GetComponent<BlockUI>();
                if (candidate != null && candidate != draggedBlock)
                {
                    existingBlock = candidate;
                    break;
                }
            }
        }

        // Se houver um bloco já neste slot, movê-lo para o tail da linha arrastada
        if (existingBlock != null)
        {
            RectTransform nextSlotTransform = tail.slotNext.GetComponent<RectTransform>();
            existingBlock.transform.SetParent(nextSlotTransform, false);
            existingBlock.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
            existingBlock.transform.localPosition = new Vector3(0, -nextSlotTransform.rect.height, 0); // força z = 0
            existingBlock.GetComponentInParent<RectTransform>().pivot = new Vector2(0.5f, 1.5f);
        }

        // Parentar a linha arrastada neste slot
        // se o slot for body, ajustar o pivot para alinhar melhor
        if (slotType == SlotType.Body) 
            rect.pivot = new Vector2(0.4f, 1.5f);
        else 
            rect.pivot = new Vector2(0.5f, 1.5f);
        draggedObj.transform.SetParent(transform, false);
        rect.anchoredPosition = Vector2.zero;
        draggedObj.transform.localPosition = new Vector3(rect.localPosition.x, rect.localPosition.y, 0f); // força z = 0
    
        // Determina o contexto do parent (herdar ancestors do parentBlock)
        BlockUI parentBlock = GetComponentInParent<BlockUI>();

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
            Debug.Log($"Ajustando altura de: {draggedBlock.bodyAncestors.Count} ancestrais do bloco arrastado em: {dragHeight}");
            draggedBlock.AdjustBodySpacers(dragHeight);
        }

        // limpa cache do draggable
        if (draggable != null)
            draggable.dragHeightCache = 0f;

        // atualiza referência local
        currentBlock = draggedBlock;
    }
}
