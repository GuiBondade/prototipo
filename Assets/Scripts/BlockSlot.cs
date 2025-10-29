using UnityEngine;
using UnityEngine.EventSystems;

public class BlockSlot : MonoBehaviour, IDropHandler
{
    public enum SlotType { Next, Body }
    public SlotType slotType;

    // So roda quando solta em cima de um bloco (GameObject que nao o workspace, que esta sendo desconsiderado)
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
            existingBlock.transform.localPosition = new Vector3(0, 0, 0); // força z = 0
        }

        // Parentar a linha arrastada neste slot
        rect.pivot = new Vector2(0.5f, 1.5f);
        draggedObj.transform.SetParent(transform, false);
        rect.anchoredPosition = Vector2.zero;
        draggedObj.transform.localPosition = new Vector3(rect.localPosition.x, rect.localPosition.y, 0f); // força z = 0
    
        // Atualiza layout do body após drop
        draggedBlock.UpdateBodyLayout();
    }
}
