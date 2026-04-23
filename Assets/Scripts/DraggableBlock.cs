using UnityEngine;
using UnityEngine.EventSystems;

// [RequireComponent(typeof(BlockUI))] (pra que serve?) 
public class DraggableBlock : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [HideInInspector] public BlockUI blockUI;
    //public BlockSlot blockSlot;
    private RectTransform rect; // ns se precisa
    private CanvasGroup canvasGroup;
    private RectTransform workspace;

    private Transform originalParent;
    private Vector3 originalLocalPos;
    public float dragHeightCache;

    private Vector2 offset; // precisa, a nao ser q ache alternativa

    void Awake()
    {
        blockUI = GetComponent<BlockUI>();
        rect = GetComponent<RectTransform>(); // ns se precisa
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null) canvasGroup = gameObject.AddComponent<CanvasGroup>();
        
        var go = GameObject.Find("Workspace");
        if (go) workspace = go.GetComponent<RectTransform>();
    }

    public void OnBeginDrag(PointerEventData eventData) {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(workspace, eventData.position, eventData.pressEventCamera, out Vector2 localPoint);
      
        // Converte posição do bloco (em world space) para o espaço da workspace
        Vector2 blockPosInWorkspace = workspace.InverseTransformPoint(rect.position);
        offset = blockPosInWorkspace - localPoint;

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
            Debug.Log($"Subtraindo altura de: {blockUI.bodyAncestors.Count} ancestrais do bloco arrastado em: {dragHeightCache}");
            blockUI.AdjustBodySpacers(-dragHeightCache);
            if (originalParent != workspace && originalParent.GetComponent<BlockSlot>().slotType == BlockSlot.SlotType.Body) blockUI.AdjustBodySpacers(30);

        // Parenta na workspace (ou root) para arrastar livremente
        if (workspace != null) {
            transform.SetParent(workspace, true);
        }

        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
    }

    public void OnDrag(PointerEventData eventData) {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(workspace, eventData.position, eventData.pressEventCamera, out Vector2 localPoint);
        rect.localPosition = localPoint + offset;
    }

    public void OnEndDrag(PointerEventData eventData) {
        canvasGroup.blocksRaycasts = true;

        // Se soltar fora da workspace, destruir
        if (!RectTransformUtility.RectangleContainsScreenPoint(workspace, eventData.position, eventData.pressEventCamera)) {
            Destroy(gameObject);
        }
        // verificar se foi solto em um slot de bloco, caso não, resetar os ancestrais
        else
        {
            var parentBlock = transform.parent.GetComponentInParent<BlockUI>();
            if (parentBlock == null)
            {
                // resetar ancestrais
                blockUI.AssignBodyAncestorsRecursive(null, null);
                // resetar currentblock do slot que tava alocado
                var slotParent = originalParent.GetComponent<BlockSlot>();
                if (slotParent != null) slotParent.currentBlock = null; // atual = blocoDrag.slotPai.currentBlock; talvez arrumar pra ...BlockSlot.currentBlock (nao parece ta dando problema, n se mexe em time que n ta perdendo)
            }
        }
    }
}
