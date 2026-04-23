using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class PaletteItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Tooltip("Prefab UI que será instanciado na Workspace")]
    public GameObject blockPrefab;

    [Tooltip("Prefab Parâmetros que será instanciado no Bloco Prefab")]
    public GameObject parameterPrefab;

    [Tooltip("RectTransform do painel workspace (onde instanciar)")]
    public RectTransform workspace;

    [Tooltip("BlockData associado a este item da paleta")]
    public BlockData blockData;

    private GameObject draggingInstance;

    private RectTransform rect;
    
    private Vector2 offset;

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
        RectTransform TopSlot = null;
        if (draggingInstance.GetComponent<BlockUI>().slotBody == null) { // bloco next
            TopSlot = draggingInstance.GetComponent<RectTransform>();
        } else {
            TopSlot = draggingInstance.transform.Find("TopBody").GetComponent<RectTransform>();
        }
        
        foreach (var parametro in blockData.parametros) {
            var parametroInstanciado = Instantiate(parameterPrefab, TopSlot);
            parametroInstanciado.GetComponent<TMP_Dropdown>().captionText.text = parametro.name;
            parametroInstanciado.GetComponent<TMP_Dropdown>().options.Add(new TMP_Dropdown.OptionData("exemplo...",null)); // por em foreach, e adicionar optiondata pra cada opção dependendo do tipo do parametro
        }
        
        rect = draggingInstance.GetComponent<RectTransform>();

        // Configurar o UI do bloco com os dados
        var ui = draggingInstance.GetComponent<BlockUI>();
        if (ui != null && blockData != null) {
            ui.Setup(blockData);
        }

        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);

        RectTransformUtility.ScreenPointToLocalPointInRectangle(workspace, eventData.position, eventData.pressEventCamera, out Vector2 localPoint);
        // Converte posição do bloco (em world space) para o espaço da workspace
        Vector2 blockPosInWorkspace = workspace.InverseTransformPoint(rect.position);
        if (draggingInstance.GetComponent<BlockUI>().slotBody != null) { // se for bloco body
            offset = new Vector2((rect.sizeDelta.x)/2, -(rect.sizeDelta.y)/2);
        } else { // se for bloco next
            offset = new Vector2((ui.slotNext.GetComponent<RectTransform>().sizeDelta.x)/2, -(ui.slotNext.GetComponent<RectTransform>().sizeDelta.y)/2); 
        }
        rect.anchoredPosition = localPoint - offset;

        // eu real nem sei se faz sentido esse canvas group (acho que funciona enquanto to fazendo drag do bloco, dai é util mesmo)
        var cg = draggingInstance.GetComponent<CanvasGroup>();
        if (cg == null) cg = draggingInstance.AddComponent<CanvasGroup>();
        cg.blocksRaycasts = false; // Permitir que o drag atravesse raycasts
    } 

    public void OnDrag (PointerEventData eventData) {
        if (draggingInstance == null) return;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(workspace, eventData.position, eventData.pressEventCamera, out Vector2 localPoint);
        rect.anchoredPosition = localPoint - offset;
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
