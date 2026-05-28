using UnityEngine;
using UnityEngine.UI;

public class ColoridorSequencial : MonoBehaviour
{
    // VARIÁVEL ESTÁTICA CENTRAL: Compartilhada por absolutamente todas as instâncias na memória
    private static int indexCorGlobal = 0;

    // Paleta estática com as 5 cores possíveis
    [SerializeField]
    public Color[] PALETA_DE_CORES = new Color[]
    {
        Color.black,                                   // 0: Preto de nascença
        new Color(0.89f, 0.66f, 0.34f, 1f),           // 1: Dourado
        new Color(0.2f, 0.6f, 0.86f, 1f),             // 2: Azul
        new Color(0.15f, 0.68f, 0.37f, 1f),           // 3: Verde
        new Color(0.75f, 0.22f, 0.17f, 1f)            // 4: Vermelho
    };

    private Image imagemParentese;

    void Start()
    {
        imagemParentese = GetComponent<Image>();
        
        if (imagemParentese != null)
        {
            // 1. Aplica a cor atual apontada pelo índice estático global
            imagemParentese.color = PALETA_DE_CORES[indexCorGlobal];

            // 2. Avança o índice para o próximo parêntese que nascer na tela
            indexCorGlobal++;

            // 3. RESET AUTOMÁTICO: Se o índice estático chegar a 5, ele volta para 0
            if (indexCorGlobal >= PALETA_DE_CORES.Length)
            {
                indexCorGlobal = 0;
            }
        }
    }
}
