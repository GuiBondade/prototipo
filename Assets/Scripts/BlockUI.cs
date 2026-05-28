using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BlockUI : MonoBehaviour
{
    public Image background;
    public Image backgroundSecondary;
    public TMP_Text label;

    [HideInInspector] public BlockData data;
    public RectTransform TopSlot;
    public List<ReferenceHolder> parameterInitialList;
    public BlockSlot slotNext;
    public BlockSlot slotBody;
    public RectTransform bodySpacer;

    [Header("Ancestrais de body")]
    public List<BlockUI> bodyAncestors = new List<BlockUI>();
    public BlockUI parentBodyOwner;

    [Header("Configurações")]
    public float defaultSpacerHeight = 30f;

    public void SetupUI(BlockData newData) {
        data = newData;

        if (label != null) {
            label.text = newData.blockName;
            label.ForceMeshUpdate();
            //LayoutRebuilder.ForceRebuildLayoutImmediate(label.GetComponent<RectTransform>());
        }
        if (background != null) background.color = newData.color;
        if (backgroundSecondary != null){
            backgroundSecondary.color = newData.color;
            bodySpacer.GetComponent<Image>().color = newData.color;
        }
        /* Debug.Log(TopSlot.sizeDelta);
        LayoutRebuilder.ForceRebuildLayoutImmediate(TopSlot);
        Debug.Log(TopSlot.sizeDelta); */
    }

// ------------ Acessores ------------

    // Retorna referência ao próximo bloco no slotNext ou slotBody
    public BlockUI GetNext() { // SLOT NEXT VAI TER MAIS DE UM FILHO? [na teoria nao vai ter, e daria pra tirar o loop]
        if (slotNext == null) return null;

        // iterar de baixo pra cima dentre os filhos de slotNext
        for (int i = slotNext.transform.childCount - 1; i >= 0; i--) {
            BlockUI block = slotNext.transform.GetChild(i).GetComponent<BlockUI>();
            if (block != null) return block;
        }

        return null;
    }

    public BlockUI GetBody() { // SLOT BODY VAI TER MAIS DE UM FILHO? [na teoria nao vai ter, e daria pra tirar o loop]
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

    // Retorna a altura total ocupada por este bloco e toda a cadeia abaixo (next/body),
    // incluindo os bodySpacers dos blocos que possuem body.
    public float GetTailHeight()
    {
        float total = 0f;

        var myRect = GetComponent<RectTransform>();
        if (myRect != null) total += myRect.rect.height;

        // Se tiver bodySpacer (filhos do body), incluir sua altura também:
        if (bodySpacer != null){
            total += bodySpacer.sizeDelta.y; // aumenta o tamanho adicional dos filhos do slot body
            total += slotNext.GetComponent<RectTransform>().rect.height; // espaço do slot next
        }
        // soma recursiva do next
        var next = GetNext();
        if (next != null)
            total += next.GetTailHeight();
        //Debug.Log($"Calculando GetTailHeight para {this}: altura total = {total}");
        return total;
    }


// ------------ Ancestralidade ------------

    public void AssignBodyAncestorsRecursive(List<BlockUI> parentAncestors, BlockUI parentBodyOwner)
    {
        // Atualiza lista local
        bodyAncestors.Clear();
        if (parentAncestors != null && parentAncestors.Count > 0)
            bodyAncestors.AddRange(parentAncestors);

        if (parentBodyOwner != null)
            bodyAncestors.Add(parentBodyOwner);

        this.parentBodyOwner = parentBodyOwner;

        // Propaga para next (mesmos ancestrais)
        var next = GetNext();
        if (next != null)
            next.AssignBodyAncestorsRecursive(this.bodyAncestors, null);

        // Propaga para body (agora o current block vira parentBodyOwner para os filhos do body)
        var bodyChild = GetBody();
        if (bodyChild != null)
        {
            //var newAnc = new List<BlockUI>(this.bodyAncestors);
            //Debug.Log("Assigning body ancestors: " + newAnc.Count + " with new parentBodyOwner: " + this);
            // se este bloco tem slotBody, ele passa a ser ancestor para filhos do body
            //if (this.slotBody != null)
            //    newAnc.Add(this);
            bodyChild.AssignBodyAncestorsRecursive(this.bodyAncestors, this);
        }
    }

// ------------ Helpers para spacer ------------
    public void ResetSpacer()
    {
        if (bodySpacer == null) return;
        bodySpacer.sizeDelta = new Vector2(bodySpacer.sizeDelta.x, defaultSpacerHeight);
    }

    // Ajusta todos os bodySpacers dos ancestors adicionando delta (pode ser positivo ou negativo)
    public void AdjustBodySpacers(float delta)
    {
        if (bodyAncestors == null || bodyAncestors.Count == 0) return;
        //print("BodySpacer ajustado");

        foreach (var ancestor in bodyAncestors) // vai de dentro pra fora ou o contrario?
        {
            if (ancestor == null || ancestor.bodySpacer == null) continue;

            // Ajusta apenas o eixo Y do sizeDelta
            ancestor.bodySpacer.sizeDelta += new Vector2(0f, delta);

            /* if (ancestor.slotBody.transform.childCount == 0) { //(ancestor.bodySpacer.sizeDelta.y % 100 == defaultSpacerHeight && ancestor.bodySpacer.sizeDelta.y != defaultSpacerHeight) {
                ancestor.bodySpacer.sizeDelta += new Vector2(0, defaultSpacerHeight); 
            } else {
                ancestor.bodySpacer.sizeDelta -= new Vector2(0, defaultSpacerHeight); 
            }  */
        }

        // Agora força o rebuild do layout apenas nos ancestors afetados
        foreach (var ancestor in bodyAncestors)
        {
            
            var rt = ancestor.GetComponent<RectTransform>();
            if (rt != null) LayoutRebuilder.ForceRebuildLayoutImmediate(rt);
        }
    }
}
