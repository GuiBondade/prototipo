using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;


public abstract class OptionInfo : MonoBehaviour
{
    public TMP_Text label;
    public GameObject checkmark;    

    protected Button btn;
    protected ParameterConfig parentParamConfig;
    /* public enum OptionType {value, section}
    public OptionType tipo; */

    // sepa que eu boto uma referencia pra seção que é ou faz parte aqui (e/ou então o valor dentro da seção)
    // JÁ SEI, ponho referencia pra enum da seção, junto com os valores, se for um tipo valor, tera so um valor na lista, senao tera todos os valores da seção
    
    [HideInInspector] public ParameterSections sectionCurrent;// referencia pra saber oq fazer apos a seleção de valor(clique)

    public void Start() { // atribuição do botao da opção
        btn = GetComponent<Button>();
        parentParamConfig = GetComponentInParent<ParameterConfig>();

        ConfigurarBotao();
    }

    protected abstract void ConfigurarBotao();
}
