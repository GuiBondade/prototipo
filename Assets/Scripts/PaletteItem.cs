using UnityEngine;
using UnityEngine.EventSystems;

public class PaletteItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Tooltip("Prefab UI que será instanciado na Workspace")]
    public GameObject blockPrefab;

    [Tooltip("RectTransform do painel workspace (onde instanciar)")]
    public RectTransform workspace;

    [Tooltip("BlockData associado a este item da paleta")]
    public BlockData blockData;

    private GameObject draggingInstance;

    public Vector2 offset;
    
    void Start()
    {
        if (workspace == null) {
            var go = GameObject.Find("Workspace");
            if (go) workspace = go.GetComponent<RectTransform>();
        }
    }

    public void OnBeginDrag(PointerEventData eventData) {
        if (blockPrefab == null || workspace == null) return;

        draggingInstance = Instantiate(blockPrefab, workspace);

        // Configurar o UI do bloco com os dados
        var ui = draggingInstance.GetComponent<BlockUI>();
        if (ui != null && blockData != null) {
            ui.Setup(blockData);
        }

        var rect = draggingInstance.GetComponent<RectTransform>();
        RectTransformUtility.ScreenPointToLocalPointInRectangle(workspace, eventData.position, eventData.pressEventCamera, out Vector2 localPoint);
        rect.anchoredPosition = localPoint;

        // eu real nem sei se faz sentido esse canvas group (acho que funciona enquanto to fazendo drag do bloco, dai é util mesmo)
        var cg = draggingInstance.GetComponent<CanvasGroup>();
        if (cg == null) cg = draggingInstance.AddComponent<CanvasGroup>();
        cg.blocksRaycasts = false; // Permitir que o drag atravesse raycasts
    } 

    public void OnDrag (PointerEventData eventData) {
        if (draggingInstance == null) return;
        var rect = draggingInstance.GetComponent<RectTransform>();
        RectTransformUtility.ScreenPointToLocalPointInRectangle(workspace, eventData.position, eventData.pressEventCamera, out Vector2 localPoint);
        rect.anchoredPosition = localPoint;
    }

    public void OnEndDrag(PointerEventData eventData) {
        if (draggingInstance == null) return;

        // Finaliza drag: permitir que o objeto receba raycasts e possa ser movido depois
        var cg = draggingInstance.GetComponent<CanvasGroup>();
        if (cg) cg.blocksRaycasts = true;

        // Se liberou fora da workspace, descarta; caso contrário mantém
        if (!RectTransformUtility.RectangleContainsScreenPoint(workspace, eventData.position, eventData.pressEventCamera)) {
            Destroy(draggingInstance);
        }
        else {
            // Por padrão, coloca como filho do workspace
            draggingInstance.transform.SetParent(workspace, true);

            // Se estiver sobre um BlockSlot, coloca como filho dele
            if (eventData.pointerEnter != null)
            {
                BlockSlot slot = eventData.pointerEnter.GetComponentInParent<BlockSlot>();
                if (slot != null)
                {
                    // Seta o drag height cache no DraggableBlock
                    var draggable = draggingInstance.GetComponent<DraggableBlock>();
                    if (draggable != null)
                    {
                        var myRect = GetComponent<RectTransform>();
                        draggable.dragHeightCache = draggable.blockUI.GetTailHeight();
                    }
                    // Simula que o bloco instanciado está sendo arrastado sobre o slot
                    PointerEventData fakeEvent = new PointerEventData(EventSystem.current);
                    fakeEvent.pointerDrag = draggingInstance;
                    slot.OnDrop(fakeEvent);
                }
            }
        }

        draggingInstance = null;
    }

}
