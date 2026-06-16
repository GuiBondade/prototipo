using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableBlock : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerDownHandler, IPointerUpHandler
{
    [HideInInspector] public BlockUI blockUI;
    private RectTransform rect;
    private CanvasGroup canvasGroup;
    private CanvasAreasManager canvasAreasManager;
    private ZoomScrollWorkspace scrollWorkspace;

    private BlocksFactory blocksFactory;

    private Transform originalParent;
    //private Vector3 originalLocalPos;

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
    }
    
    void Start() 
    {
        canvasAreasManager = CanvasAreasManager.instancia;
        scrollWorkspace = canvasAreasManager.visibleWorkspace.GetComponent<ZoomScrollWorkspace>();
        blocksFactory = BlocksFactory.instance;
    }

    public void OnBeginDrag(PointerEventData eventData) {
        if (isRightClickDragging || eventData.button == PointerEventData.InputButton.Right)
        {
            eventData.pointerDrag = null;
            return;
        }

        BlockerManager.instancia.Reset();

        originalParent = transform.parent;
        //originalLocalPos = transform.localPosition;
        canvasGroup.blocksRaycasts = false;

        // Subtrai essa altura dos blockSpacers dos ancestrais do bloco sendo arrastado
        if (blockUI.bodyAncestors != null && blockUI.bodyAncestors.Count > 0) { // originalParent = slotBody
            blockUI.AdjustBodySpacers(-(blockUI.GetTailHeight()));
        }
        if (originalParent != canvasAreasManager.contentWorkspace && originalParent.TryGetComponent<BlockSlot>(out BlockSlot slot)) {
            if (slot.slotType == SlotType.Body) { //significa que ta tirando o filho direto do body, junto a seus filhos(deixando o slot vazio de blocos)
                blockUI.AdjustBodySpacers(blockUI.defaultSpacerHeight);
            }
        } 
        blockUI.AssignBodyAncestorsRecursive(null, null);

        // Parenta na workspace (ou root) para arrastar livremente
        transform.SetParent(canvasAreasManager.canvasProgramming, true);

        // resetar currentblock do slot que tava alocado
        var slotParent = originalParent.GetComponent<BlockSlot>();
        if (slotParent != null) slotParent.currentBlock = null;

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
        
        scrollWorkspace.blockInDrag = null;

        // Se soltar fora da workspace, destruir
        if (!RectTransformUtility.RectangleContainsScreenPoint(canvasAreasManager.visibleWorkspace, eventData.position, eventData.pressEventCamera)) {
            blocksFactory.CleanUpBlock(blockUI);
        }
        // verificar se foi solto em um slot de bloco, caso não, resetar os ancestrais
        else
        {
            // Se estiver sobre um BlockSlot, coloca como filho dele
            var placeDropped = eventData.pointerEnter?.transform;
            if (placeDropped != null && placeDropped != canvasAreasManager.contentWorkspace)
            {
                Debug.Log("dropou em bloco");
                BlockSlot slot = BlockUIUtils.GetBlockSlotByEventData(eventData); 
                if (slot != null) blocksFactory.ConnectToSlot(blockUI, false, slot.transform);
            } else {
                Debug.Log("dropou em workspace");
                blocksFactory.ConnectToSlot(blockUI, true, canvasAreasManager.contentWorkspace);
            }

        }

        canvasGroup.blocksRaycasts = true;
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
