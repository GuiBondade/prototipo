using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BlockUI : MonoBehaviour
{
    public Image background;
    public TMP_Text label;

    [HideInInspector] public BlockData data;
    public BlockSlot slotNext;
    public BlockSlot slotBody;

    public void Setup(BlockData newData) {
        data = newData;

        if (label != null) label.text = newData.blockName;
        if (background != null) background.color = newData.color;
    }

    // Retorna referência ao próximo bloco no slotNext ou slotBody
    public BlockUI GetNext() {
        if (slotNext == null) return null;

        // iterar de baixo pra cima dentre os filhos de slotNext
        for (int i = slotNext.transform.childCount - 1; i >= 0; i--) {
            BlockUI block = slotNext.transform.GetChild(i).GetComponent<BlockUI>();
            if (block != null) return block;
        }

        return null;
    }

    public BlockUI GetBody() {
        if (slotBody == null) return null;

        // iterar de baixo pra cima dentre os filhos de slotBody
        for (int i = slotBody.transform.childCount - 1; i >= 0; i--) {
            BlockUI block = slotBody.transform.GetChild(i).GetComponent<BlockUI>();
            if (block != null) return block;
        }

        return null;
    }

    // pega o ultimo bloco da linha
    public BlockUI GetTail() {
        BlockUI tail = this;
        while (tail.GetNext() != null) {
            tail = tail.GetNext();
        }
        return tail;
    }

    // Atualiza o layout do corpo (slotBody) e propaga para blocos pais
    public void UpdateBodyLayout()
    {
        if (slotBody != null && slotBody.transform.childCount > 0)
        {
            RectTransform container = slotBody.transform as RectTransform;
            LayoutRebuilder.ForceRebuildLayoutImmediate(container);
        }

        if (transform.parent != null)
        {
            BlockUI parentBlock = transform.parent.GetComponentInParent<BlockUI>();
            if (parentBlock != null)
            {
                parentBlock.UpdateBodyLayout();
            }
        }
    }
}
