using UnityEngine;
using UnityEngine.EventSystems;

public class ZoomScrollWorkspace : MonoBehaviour, IScrollHandler
{
    // FEITO TOTALMENTE POR IA
    [Header("Alvo do Zoom e Movimento")]
    [SerializeField] private RectTransform conteudoTransform;

    private Canvas canvas;
    private RectTransform paiTransform;
    private bool isDragging = false;
    private Vector2 lastMousePosition;

    [Header("Configurações de Zoom")]
    [SerializeField] private float zoomSpeed = 0.1f;
    [SerializeField] private float maxScale = 3f;
    // O minScale fixo foi removido pois agora ele é calculado dinamicamente

    [HideInInspector] public RectTransform blockInDrag;

    void Awake()
    {
        paiTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        
        if (conteudoTransform == null && transform.childCount > 0)
        {
            conteudoTransform = transform.GetChild(0).GetComponent<RectTransform>();
        }

        blockInDrag = null;
    }

    void Update()
    {
        HandleRightClickDrag();
    }

    private void HandleRightClickDrag()
    {
        if (Input.GetMouseButtonDown(1))
        {
            if (RectTransformUtility.RectangleContainsScreenPoint(paiTransform, Input.mousePosition, canvas.worldCamera))
            {
                isDragging = true;
                lastMousePosition = Input.mousePosition;
                //if (BlockerManager.instancia != null) BlockerManager.instancia.Reset();
            }
        }

        if (Input.GetMouseButton(1) && isDragging)
        {
            Vector2 currentMousePosition = Input.mousePosition;
            Vector2 mouseDelta = currentMousePosition - lastMousePosition;

            Vector3 novaPosicao = conteudoTransform.localPosition + (Vector3)(mouseDelta / canvas.scaleFactor);
            conteudoTransform.localPosition = ClampPosition(novaPosicao);
            
            lastMousePosition = currentMousePosition;
        }

        if (Input.GetMouseButtonUp(1))
        {
            isDragging = false;
        }
    }

    public void OnScroll(PointerEventData eventData)
    {
        if (conteudoTransform == null) return;

        //if (BlockerManager.instancia != null) BlockerManager.instancia.Reset();

        Vector3 currentScale = conteudoTransform.localScale;
        float zoomFactor = eventData.scrollDelta.y * zoomSpeed;
        Vector3 targetScale = currentScale + new Vector3(zoomFactor, zoomFactor, 0);

        // 1. Calcula o menor zoom permitido para que o filho NUNCA seja menor que o pai
        float menorZoomPermitido = CalcularMinScale();

        // 2. Limita o zoom entre o mínimo dinâmico e o máximo fixo
        targetScale.x = Mathf.Clamp(targetScale.x, menorZoomPermitido, maxScale);
        targetScale.y = Mathf.Clamp(targetScale.y, menorZoomPermitido, maxScale);

        RectTransformUtility.ScreenPointToLocalPointInRectangle(conteudoTransform, eventData.position, eventData.pressEventCamera, out Vector2 localMouseBefore);

        conteudoTransform.localScale = targetScale;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(conteudoTransform, eventData.position, eventData.pressEventCamera, out Vector2 localMouseAfter);

        Vector3 difference = localMouseAfter - localMouseBefore;
        Vector3 novaPosicao = conteudoTransform.localPosition + conteudoTransform.TransformDirection(difference);

        conteudoTransform.localPosition = ClampPosition(novaPosicao);

        if (blockInDrag) {
            AdjustBlockInDrag(blockInDrag, targetScale);
        }
    }

    // Calcula o zoom mínimo necessário para preencher todo o espaço do Pai
    private float CalcularMinScale()
    {
        Vector2 tamanhoPai = paiTransform.rect.size;
        Vector2 tamanhoFilhoOriginal = conteudoTransform.rect.size;

        // Descobre qual proporção (Largura ou Altura) precisa de mais zoom para cobrir o Pai
        float scaleNecessarioParaLargura = tamanhoPai.x / tamanhoFilhoOriginal.x;
        float scaleNecessarioParaAltura = tamanhoPai.y / tamanhoFilhoOriginal.y;

        // Retorna o maior valor entre os dois, garantindo cobertura total em ambos os eixos
        return Mathf.Max(scaleNecessarioParaLargura, scaleNecessarioParaAltura);
    }

    private Vector3 ClampPosition(Vector3 posicaoAlvo)
    {
        Vector2 tamanhoPai = paiTransform.rect.size;

        float larguraFilhoEscalado = conteudoTransform.rect.width * conteudoTransform.localScale.x;
        float alturaFilhoEscalado = conteudoTransform.rect.height * conteudoTransform.localScale.y;

        // Como o Filho agora é SEMPRE maior ou igual ao Pai, o limite impede que
        // as bordas do Filho entrem para dentro da área visível do Pai.
        float limiteX = (larguraFilhoEscalado - tamanhoPai.x) / 2f;
        float limiteY = (alturaFilhoEscalado - tamanhoPai.y) / 2f;

        // Trava a posição para que o fundo nunca fique visível
        float clampX = Mathf.Clamp(posicaoAlvo.x, -limiteX, limiteX);
        float clampY = Mathf.Clamp(posicaoAlvo.y, -limiteY, limiteY);

        return new Vector3(clampX, clampY, posicaoAlvo.z);
    }

    public void AdjustBlockInDrag(RectTransform block, Vector3 targetScale) {
        block.localScale = new Vector2(targetScale.x, targetScale.y);
        // 1. A posição do mouse na tela é a posição global no Canvas Overlay
        Vector3 mouseWorldPos = Input.mousePosition;

        // Z deve ser 0 para evitar problemas de profundidade em UI 2D
        mouseWorldPos.z = 0f; 

        // 2. Aplica diretamente na posição GLOBAL do bloco (isso anula qualquer efeito de zoom/escala dos pais)
        block.transform.position = mouseWorldPos;
    }
}