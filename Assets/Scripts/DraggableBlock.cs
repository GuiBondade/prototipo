using UnityEngine;
using UnityEngine.EventSystems;

// [RequireComponent(typeof(BlockUI))] (pra que serve?) 
public class DraggableBlock : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [HideInInspector] public BlockUI blockUI;
    private RectTransform rect; // ns se precisa
    private CanvasGroup canvasGroup;
    private RectTransform workspace;

    private Transform originalParent;
    private Vector3 originalLocalPos;
    public float dragHeightCache;

    private Vector2 offset; // ns se precisa

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


        originalParent = transform.parent;
        originalLocalPos = transform.localPosition;
        canvasGroup.blocksRaycasts = false;

        // Calcular altura total da linha arrastada e guardar no cache
        dragHeightCache = blockUI.GetTailHeight();

        // Subtrai essa altura dos blockSpacers dos ancestrais do bloco sendo arrastado
        if (blockUI.bodyAncestors != null && blockUI.bodyAncestors.Count > 0)
            Debug.Log($"Subtraindo altura de: {blockUI.bodyAncestors.Count} ancestrais do bloco arrastado em: {dragHeightCache}");
            blockUI.AdjustBodySpacers(-dragHeightCache);

        // Parenta na workspace (ou root) para arrastar livremente
        if (workspace != null) {
            transform.SetParent(workspace, true);
        }
    }

    public void OnDrag(PointerEventData eventData) {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(workspace, eventData.position, eventData.pressEventCamera, out Vector2 localPoint);
        rect.anchoredPosition = localPoint + offset;
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
                blockUI.bodyAncestors.Clear();
                blockUI.parentBodyOwner = null;
            }
        }
    }
}
