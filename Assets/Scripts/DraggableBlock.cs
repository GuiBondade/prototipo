using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableBlock : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private RectTransform rect;
    private CanvasGroup canvasGroup;
    private RectTransform workspace;

    private Vector2 offset;

    void Awake()
    {
        rect = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null) canvasGroup = gameObject.AddComponent<CanvasGroup>();
        
        var go = GameObject.Find("Workspace");
        if (go) workspace = go.GetComponent<RectTransform>();
    }

    public void OnBeginDrag(PointerEventData eventData) {
        transform.SetParent(workspace, true);

        RectTransformUtility.ScreenPointToLocalPointInRectangle(workspace, eventData.position, eventData.pressEventCamera, out Vector2 localPoint);
        offset = rect.anchoredPosition - localPoint;

        canvasGroup.blocksRaycasts = false;
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
    }
}
