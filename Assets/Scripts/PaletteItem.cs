using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class PaletteItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private CanvasAreasManager canvasAreasManager;

    public BlockData blockData;

    public TMP_Text label;

    [HideInInspector]public GameObject ParameterPrefab;

    private GameObject draggingInstance;

    private RectTransform rect;
    
    private ZoomScrollWorkspace scrollWorkspace;

    private static Vector3 GetMouseScreenPosition()
    {
        Vector3 mouseWorldPos = Input.mousePosition;
        mouseWorldPos.z = 0f;
        return mouseWorldPos;
    }

    void Start()
    {
        ParameterPrefab = Resources.Load<GameObject>("GenericParameter");

        canvasAreasManager = CanvasAreasManager.instancia;

        label = GetComponentInChildren<TMP_Text>(); // precisa? como é prefab acho que nao
        label.text = blockData.blockName;

        scrollWorkspace = canvasAreasManager.visibleWorkspace.GetComponent<ZoomScrollWorkspace>();
    }

    public void OnBeginDrag(PointerEventData eventData) {
        if (blockData.blockPrefab == null) return;
        BlockerManager.instancia.Reset();

        draggingInstance = Instantiate(blockData.blockPrefab, canvasAreasManager.canvasProgramming); // por canvas Programming aqui (pega por tag sei la)
        var ui = draggingInstance.GetComponent<BlockUI>();
        rect = draggingInstance.GetComponent<RectTransform>();

        // parametros são setados antes da propria UI, ta certo?
        /* RectTransform TopSlot = null;
        if (draggingInstance.GetComponent<BlockUI>().slotBody == null) { // bloco next
            TopSlot = draggingInstance.GetComponent<RectTransform>();
        } else { // bloco body
            TopSlot = draggingInstance.transform.Find("TopBody").GetComponent<RectTransform>();
        } */
        foreach (var parametro in blockData.listaParametros) {
            var parametroInstanciado = Instantiate(ParameterPrefab, ui.TopSlot);
            var paramComponent = parametroInstanciado.AddComponent(parametro.ScriptTypeParameter.GetClass()) as ParameterSetup;
            var paramReference = parametroInstanciado.GetComponent<ReferenceHolder>();
            paramComponent.Initialize(paramReference);
            ui.parameterInitialList.Add(paramReference);
            paramComponent.Setup(parametro.name);
            parametroInstanciado.GetComponent<AdjustWidthByText>().AdjustWidth();
            // add component parameterSetup<T> a partir de referencia do BlockData
        }
        
        // Configurar o UI do bloco com os dados
        if (ui != null && blockData != null) {
            ui.SetupUI(blockData);
        }

        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);

        rect.pivot = new Vector2(0.5f, 0.5f);

        LayoutRebuilder.ForceRebuildLayoutImmediate(rect);

        rect.position = GetMouseScreenPosition();
        rect.localScale = canvasAreasManager.contentWorkspace.localScale;
        scrollWorkspace.blockInDrag = rect; // altera bloco a ter scale alterado com scroll

        // eu real nem sei se faz sentido esse canvas group (acho que funciona enquanto to fazendo drag do bloco, dai é util mesmo)
        var cg = draggingInstance.GetComponent<CanvasGroup>();
        if (cg == null) cg = draggingInstance.AddComponent<CanvasGroup>();
        cg.blocksRaycasts = false; // Permitir que o drag atravesse raycasts
    } 

    public void OnDrag (PointerEventData eventData) {
        if (draggingInstance == null) return;
        rect.position = GetMouseScreenPosition();
    }

    public void OnEndDrag(PointerEventData eventData) {
        if (draggingInstance == null) return;

        // Finaliza drag: permitir que o objeto receba raycasts e possa ser movido depois
        var cg = draggingInstance.GetComponent<CanvasGroup>();
        if (cg) cg.blocksRaycasts = true;

        scrollWorkspace.blockInDrag = null;

        // Se liberou fora da workspace, descarta; caso contrário mantém
        if (!RectTransformUtility.RectangleContainsScreenPoint(canvasAreasManager.visibleWorkspace, eventData.position, eventData.pressEventCamera)) {
            Destroy(draggingInstance);
        }
        else {
            // Por padrão, coloca como filho do workspace
            draggingInstance.transform.SetParent(canvasAreasManager.contentWorkspace, true);

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
