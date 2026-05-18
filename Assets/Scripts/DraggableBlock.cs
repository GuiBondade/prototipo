using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(BlockUI))]
public class DraggableBlock : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerDownHandler, IPointerUpHandler
{
    [HideInInspector] public BlockUI blockUI;
    //public BlockSlot blockSlot;
    private RectTransform rect; // ns se precisa
    private CanvasGroup canvasGroup;
    private CanvasAreasManager canvasAreasManager;
    private ZoomScrollWorkspace scrollWorkspace;

    private Transform originalParent;
    private Vector3 originalLocalPos;
    public float dragHeightCache;

    private bool isRightClickDragging = false;

    private static Vector3 GetMouseScreenPosition()
    {
        Vector3 mouseWorldPos = Input.mousePosition;
        mouseWorldPos.z = 0f;
        return mouseWorldPos;
    }

    void Awake()
    {
        blockUI = GetComponent<BlockUI>();
        rect = GetComponent<RectTransform>(); // ns se precisa
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null) canvasGroup = gameObject.AddComponent<CanvasGroup>();
        
        canvasAreasManager = CanvasAreasManager.instancia;
        scrollWorkspace = canvasAreasManager.visibleWorkspace.GetComponent<ZoomScrollWorkspace>();
    }

    public void OnBeginDrag(PointerEventData eventData) {
        if (isRightClickDragging || eventData.button == PointerEventData.InputButton.Right)
        {
            eventData.pointerDrag = null;
            return;
        }

        BlockerManager.instancia.Reset();

        transform.SetAsLastSibling(); // fica na frente de tudo

        originalParent = transform.parent;
        originalLocalPos = transform.localPosition;
        canvasGroup.blocksRaycasts = false;

        // Calcular altura total da linha arrastada e guardar no cache
        dragHeightCache = blockUI.GetTailHeight();

        // Subtrai essa altura dos blockSpacers dos ancestrais do bloco sendo arrastado
        if (blockUI.bodyAncestors != null && blockUI.bodyAncestors.Count > 0) // originalParent = slotBody
            /* if (originalParent.GetComponent<BlockSlot>().slotType == BlockSlot.SlotType.Body) {
                originalParent.GetComponentInParent<BlockUI>().bodySpacer.sizeDelta += new Vector2(0, 30);
            } */
            //Debug.Log($"Subtraindo altura de: {blockUI.bodyAncestors.Count} ancestrais do bloco arrastado em: {dragHeightCache}");
            blockUI.AdjustBodySpacers(-dragHeightCache);
            if (originalParent != canvasAreasManager.contentWorkspace && originalParent.TryGetComponent<BlockSlot>(out BlockSlot slot)) {
                if (slot.slotType == BlockSlot.SlotType.Body) {
                    blockUI.AdjustBodySpacers(30);
                }
            } 

        // Parenta na workspace (ou root) para arrastar livremente
        transform.SetParent(canvasAreasManager.canvasProgramming, true);

        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);

        rect.pivot = new Vector2(0.5f, 0.5f);

        scrollWorkspace.blockInDrag = rect; // altera bloco a ter scale alterado com scroll
    }

    public void OnDrag(PointerEventData eventData) {
        if (isRightClickDragging || eventData.button == PointerEventData.InputButton.Right) return;
        rect.position = GetMouseScreenPosition();
    }

    public void OnEndDrag(PointerEventData eventData) {
        if (isRightClickDragging || eventData.button == PointerEventData.InputButton.Right) return;
        canvasGroup.blocksRaycasts = true;
        
        scrollWorkspace.blockInDrag = null;

        // Se soltar fora da workspace, destruir
        if (!RectTransformUtility.RectangleContainsScreenPoint(canvasAreasManager.visibleWorkspace, eventData.position, eventData.pressEventCamera)) {
            Destroy(gameObject);
        }
        // verificar se foi solto em um slot de bloco, caso não, resetar os ancestrais
        else
        {
            var parentBlock = transform.parent.GetComponentInParent<BlockUI>();
            if (parentBlock == null)
            {
                transform.SetParent(canvasAreasManager.contentWorkspace, true);
                // resetar ancestrais
                blockUI.AssignBodyAncestorsRecursive(null, null);
                // resetar currentblock do slot que tava alocado
                var slotParent = originalParent.GetComponent<BlockSlot>();
                if (slotParent != null) slotParent.currentBlock = null;
            }
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            isRightClickDragging = true;
            eventData.pointerDrag = null; // Cancela o rastreamento de arrasto na UI da Unity
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            isRightClickDragging = false;
        }
    }
}
