using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;


public abstract class OptionInfo : MonoBehaviour
{
    public TMP_Text label;
    public Image checkmark;    

    public Button btn;
    protected ParameterConfig parentParamConfig;
    public ParameterSections sectionCurrent;// referencia pra saber oq fazer apos a seleção de valor(clique)

    public void Setup() { // atribuição do botao da opção
        btn = GetComponent<Button>();
        parentParamConfig = GetComponentInParent<ParameterConfig>();

        ConfigurarBotao();
    }

    protected abstract void ConfigurarBotao();
}
