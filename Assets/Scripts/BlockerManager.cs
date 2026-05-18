using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlockerManager : MonoBehaviour
{
    public static BlockerManager instancia { get; private set; }

    public RectTransform workspace;

    [HideInInspector] public GameObject UIEmFoco;

    void Awake() 
    {
        instancia = this;
    }

    void Start() 
    {
        GetComponent<Button>().onClick.AddListener(() => Reset());
        gameObject.GetComponent<Image>().raycastTarget = false;
    }

    public void Reset() 
    {
        if (UIEmFoco != null) UIEmFoco.SetActive(false);
        UIEmFoco = null;
        gameObject.GetComponent<Image>().raycastTarget = false;
    }

    public void SetUIOnBlocker(GameObject UI) 
    {
        Reset();
        UIEmFoco = UI;
        UIEmFoco.SetActive(true);
        gameObject.GetComponent<Image>().raycastTarget = true;
        
        /* // Redimensiona para o tamanho do avô uma única vez ao abrir, to vendo de so ativar o reset quando acessar qualquer UI
        // se mudar de ideia so reativar isso e os componentes do blocker
        RectTransform blockerRect = GetComponent<RectTransform>();
        if (transform.parent != null && transform.parent.parent != null)
        {
            RectTransform avoRect = transform.parent.parent.GetComponent<RectTransform>();
            if (avoRect != null)
            {
                blockerRect.anchorMin = new Vector2(0.5f, 0.5f);
                blockerRect.anchorMax = new Vector2(0.5f, 0.5f);
                blockerRect.pivot = new Vector2(0.5f, 0.5f);

                Vector3 posicaoCentroAvo = transform.parent.InverseTransformPoint(avoRect.position);
                blockerRect.localPosition = new Vector3(posicaoCentroAvo.x, posicaoCentroAvo.y, 0f);

                blockerRect.sizeDelta = avoRect.rect.size;
            }
        } */

        transform.SetAsLastSibling();
        
        var blockUI = UIEmFoco.GetComponentInParent<BlockUI>();
        var blockParent = blockUI.transform.parent;
        while (blockParent != workspace && blockParent != null) {
            blockUI = blockParent.GetComponentInParent<BlockUI>();
            blockParent = blockParent.parent;
        }
        blockUI.transform.SetAsLastSibling();
    }
}
