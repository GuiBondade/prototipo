using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ParameterConfig : MonoBehaviour
{
    public ReferenceHolder refs;

    [HideInInspector] public GameObject ParameterPrefab;

    [HideInInspector] public GameObject inputText; // settado na instanciação (quando int) 
    
    [HideInInspector] public ParameterSections sectionSelected;

    private BlockerManager blocker;

    public void Start() {
        ParameterPrefab = Resources.Load<GameObject>("GenericParameter");
        blocker = BlockerManager.instancia;
    }

    public void SetInputText(GameObject inputTextInstance) {
        inputText = inputTextInstance;
    }

    public void OnTriggerButtonPress() {
        blocker.SetUIOnBlocker(refs.selectionArea);
    }

    public void SectionPressionado(GameObject sectionPressed) { 
        // da pra subtituir sectionPressed por this?
        if (sectionPressed != refs.selectedSection) {
            var sectionPressedInfo = sectionPressed.GetComponent<SectionInfo>();
            UpdateValues(sectionPressedInfo);
            refs.valueContent.GetComponent<AdjustWidthByText>().AdjustWidth();
            UpdateSelectedOption(sectionPressed);
        }
    }

    public void ValuePressionado(GameObject valuePressed) {
        if (valuePressed != refs.selectedValue) { 
            var valuePressedInfo = valuePressed.GetComponent<ValueInfo>(); //value info
            // refs.placeholderLabel vai ser o valor final(por enquanto), depois vai ter somente valor equivalente ao atribuido ao valor final
            // atribuir valor no setup/config para leitura pratica no save
            refs.placeholderLabel.GetComponent<TMP_Text>().text = valuePressedInfo.label.text;
            GetComponent<AdjustWidthByText>().AdjustWidth();

            if (valuePressedInfo.sectionCurrent != sectionSelected) {
                ResetParameterState();

                switch (valuePressedInfo.sectionCurrent) {
                    case ParameterSections.BoolValues:
                    case ParameterSections.DirectionValues:
                    case ParameterSections.MaterialValues:
                        break; // nada
                    case ParameterSections.IntValues:
                        if (valuePressedInfo.value == GetComponent<ParameterSetup>().GetIntValueAt(0)) inputText.SetActive(true);
                        break; // ativar InputInt GO caso seja o valor pressionado(index 0)
                    case ParameterSections.BoolLogicOperations:
                        InstantiateParameter<BooleanParameter>(ref refs.leftOperand, true, "Boolean");
                        InstantiateParameter<BooleanParameter>(ref refs.rightOperand, false, "Boolean");
                        LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
                        break; // adicionar dois prefab boolean (como first e last) (irmaos de genericParameter)
                    case ParameterSections.BoolAllComparisonOperations:
                        InstantiateParameter<AllTypeParameter>(ref refs.leftOperand, true, "All Types");
                        InstantiateParameter<AllTypeParameter>(ref refs.rightOperand, false, "All Types");
                        LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
                        break; // adicionar dois prefab com todas as seções (como first e last) (irmaos de genericParameter)
                    case ParameterSections.IntOperations: // adicionar dois prefabs param int (como first e last) (irmaos de genericParameter)
                    case ParameterSections.BoolIntComparisonOperations:
                        InstantiateParameter<IntParameter>(ref refs.leftOperand, true, "Int");
                        InstantiateParameter<IntParameter>(ref refs.rightOperand, false, "Int");
                        LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
                        break; // adicionar dois prefabs param int, um como first e outro como last (irmaos de genericParameter)
                    default:
                        Debug.Log("Deu erro ao pressionar valor, parece que tem opção de seção a mais no Setup(ParameterSections) do que no Config(ValuePressionado");
                        break;
                } 
            } else if (valuePressedInfo.sectionCurrent == ParameterSections.IntValues) {
                if (valuePressedInfo.value == GetComponent<ParameterSetup>().GetIntValueAt(0)) inputText.SetActive(true);
            }
            Transform atual = transform.parent; // Começa do pai direto
            while (atual != null) {
                LayoutRebuilder.ForceRebuildLayoutImmediate(atual.GetComponent<RectTransform>());
                var componente = atual.GetComponent<ParameterConfig>();
                if (componente == null) break;
                atual = atual.parent; // Sobe um nível
            }

            sectionSelected = valuePressedInfo.GetComponent<ValueInfo>().sectionCurrent;
            UpdateSelectedOption(valuePressed);
        }
        blocker.Reset();
    }

    public void InstantiateParameter<T>(ref GameObject instanceReference, bool isFirst, string label) where T : ParameterSetup
    {
        instanceReference = Instantiate(ParameterPrefab, transform);
        if (isFirst) instanceReference.transform.SetAsFirstSibling();
        else instanceReference.transform.SetAsLastSibling();

        T parameterComponent = instanceReference.AddComponent<T>(); // nao ta dando certo
        parameterComponent.Initialize(instanceReference.GetComponent<ReferenceHolder>());
        parameterComponent.Setup(label);
        instanceReference.GetComponent<AdjustWidthByText>().AdjustWidth();
        LayoutRebuilder.ForceRebuildLayoutImmediate(instanceReference.GetComponent<RectTransform>());
    }

    public void UpdateSelectedOption(GameObject optionPressed) { // tem que ALTERAR porque ta so funfando pra section
        var pressedOptionInfo = optionPressed.GetComponent<OptionInfo>();
        GameObject preSelectedOption = null;
        if (pressedOptionInfo is SectionInfo) {
            preSelectedOption = refs.selectedSection;
            refs.selectedSection = optionPressed;
        }
        else if (pressedOptionInfo is ValueInfo) {
            preSelectedOption = refs.selectedValue;
            refs.selectedValue = optionPressed;
        }

        if (preSelectedOption != null) preSelectedOption.GetComponent<OptionInfo>().checkmark.GetComponent<Image>().enabled = false;
        optionPressed.GetComponent<OptionInfo>().checkmark.GetComponent<Image>().enabled = true;
    }

    public void UpdateValues(SectionInfo sectionInfo) { //operations / values
        for (int i = refs.valueContent.transform.childCount - 1; i >= 0; i--)
        {
            Destroy(refs.valueContent.transform.GetChild(i).gameObject);
        }
        foreach (var valor in sectionInfo.valueList) { // o values pode ser uma referencia pra seção do tipo de dado do parametro, do scriptable object inherit, ve com gemini como fazer isso funfar
            var item = Instantiate(refs.valuePrefab, refs.valueContent.transform);
            var valueInfo = item.GetComponent<ValueInfo>(); 
            valueInfo.label.text = valor;
            valueInfo.label.ForceMeshUpdate(); // precisa?
            valueInfo.sectionCurrent = sectionInfo.sectionCurrent;
            valueInfo.value = valor;
        }
    }

     public void ResetParameterState() { //sepa que é melhor por reset state
        if (inputText != null) inputText.SetActive(false);
        if (refs.leftOperand != null){
             Destroy(refs.leftOperand);
             refs.leftOperand = null;
        }
        if (refs.rightOperand != null){
             Destroy(refs.rightOperand);
             Debug.Log("rightOperand apos destruição" + refs.rightOperand);
             refs.rightOperand = null;
        }
    }
}
