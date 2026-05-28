using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ParameterConfig : MonoBehaviour
{
    public ReferenceHolder refs;

    [HideInInspector] public GameObject ParameterPrefab;
    
    [HideInInspector] public ParameterSections sectionSelected;

    private BlockerManager blocker;

    public void Start() {
        ParameterPrefab = Resources.Load<GameObject>("GenericParameter");
        blocker = BlockerManager.instancia;
    }

    public void OnTriggerButtonPress() {
        blocker.SetUIOnBlocker(refs.selectionArea);
    }

    public void SectionPressionado(GameObject sectionPressed) { 
        // da pra subtituir sectionPressed por this?
        if (sectionPressed != refs.selectedSection) {
            var sectionPressedInfo = sectionPressed.GetComponent<SectionInfo>();
            UpdateValues(sectionPressedInfo);
            UpdateSelectedOption(sectionPressed);
        }
    }

    public void ValuePressionado(GameObject valuePressed) {
        if (valuePressed != refs.selectedValue) { 
            var valuePressedInfo = valuePressed.GetComponent<ValueInfo>(); //value info
            // refs.placeholderLabel vai ser o valor final(por enquanto), depois vai ter somente valor equivalente ao atribuido ao valor final
            // atribuir valor no setup/config para leitura pratica no save
            var placeholderTMPText = refs.placeholderLabel.GetComponent<TMP_Text>();
            placeholderTMPText.text = valuePressedInfo.label.text;
            placeholderTMPText.ForceMeshUpdate();

            if (valuePressedInfo.sectionCurrent != sectionSelected) {
                ResetParameterState();

                switch (valuePressedInfo.sectionCurrent) {
                    case ParameterSections.BoolValues:
                    case ParameterSections.DirectionValues:
                    case ParameterSections.MaterialValues:
                    case ParameterSections.IntValues:
                        break; 
                    case ParameterSections.BoolLogicOperations:
                        refs.parentesesInstance.SetActive(true);
                        InstantiateParameter(ref refs.leftOperand, ParameterType.BooleanParameter, true, "Boolean");
                        InstantiateParameter(ref refs.rightOperand, ParameterType.BooleanParameter, false, "Boolean");
                        refs.parentesesInstance.GetComponent<RectTransform>().SetAsFirstSibling();
                        LayoutRebuilder.ForceRebuildLayoutImmediate(refs.rootParameter.GetComponent<RectTransform>());
                        break; // adicionar dois prefab boolean (como first e last) (irmaos de genericParameter)
                    case ParameterSections.BoolAllComparisonOperations:
                        refs.parentesesInstance.SetActive(true);
                        InstantiateParameter(ref refs.leftOperand, ParameterType.AllTypeParameter, true, "All Types");
                        InstantiateParameter(ref refs.rightOperand, ParameterType.AllTypeParameter, false, "All Types");
                        refs.parentesesInstance.GetComponent<RectTransform>().SetAsFirstSibling();
                        LayoutRebuilder.ForceRebuildLayoutImmediate(refs.rootParameter.GetComponent<RectTransform>());
                        break; // adicionar dois prefab com todas as seções (como first e last) (irmaos de genericParameter)
                    case ParameterSections.IntOperations: // adicionar dois prefabs param int (como first e last) (irmaos de genericParameter)
                    case ParameterSections.BoolIntComparisonOperations:
                        refs.parentesesInstance.SetActive(true);
                        InstantiateParameter(ref refs.leftOperand, ParameterType.IntParameter, true, "Int");
                        InstantiateParameter(ref refs.rightOperand, ParameterType.IntParameter, false, "Int");
                        refs.parentesesInstance.transform.SetAsFirstSibling();
                        LayoutRebuilder.ForceRebuildLayoutImmediate(refs.rootParameter.GetComponent<RectTransform>());
                        break; // adicionar dois prefabs param int, um como first e outro como last (irmaos de genericParameter)
                    default:
                        Debug.Log("Deu erro ao pressionar valor, parece que tem opção de seção a mais no Setup(ParameterSections) do que no Config(ValuePressionado");
                        break;
                } 
            } 
            if (valuePressedInfo.sectionCurrent == ParameterSections.IntValues) {
                if (valuePressedInfo.value == GetComponent<ParameterSetup>().GetIntValueAt(0)) refs.inputText.SetActive(true);
            }
            Transform atual = transform.parent; // Começa do pai direto
            while (atual != null) {
                LayoutRebuilder.ForceRebuildLayoutImmediate(atual.GetComponent<RectTransform>());
                var componente = atual.GetComponent<ParameterConfig>();
                if (componente == null) break;
                atual = atual.parent; // Sobe um nível
            }

            GetComponent<AdjustWidthByText>().AdjustWidth();

            sectionSelected = valuePressedInfo.GetComponent<ValueInfo>().sectionCurrent;
            UpdateSelectedOption(valuePressed);
        }
        blocker.Reset();
    }

    public void InstantiateParameter(ref GameObject instanceReference, ParameterType type, bool isFirst, string label) where T : ParameterSetup
    {
        // vai ser substituido pelo blocksFatory né? nao sei nao, usa o instanceReference no workspace builder (da pra tentar melhorar)
        instanceReference = blocksFactory.InitializeParameter(label, type, transform).gameObject;
        if (isFirst) instanceReference.transform.SetAsFirstSibling();
        else instanceReference.transform.SetAsLastSibling();

        LayoutRebuilder.ForceRebuildLayoutImmediate(instanceReference.GetComponent<RectTransform>()); //precisa?
    }


    public void UpdateSelectedOption(GameObject optionPressed) { // tem que ALTERAR porque ta so funfando pra section(?, nao sei mais)
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
            var value = refs.valueContent.transform.GetChild(i);
            blocksFactory.CleanUpValue(value);
        }
        
        foreach (var valor in sectionInfo.valueList) {
            var item = blocksFactory.InitializeValue(valor, refs.valueContent.transform);
        }
        refs.valueContent.GetComponent<AdjustWidthByText>().AdjustWidth();
    }

     public void ResetParameterState() { // chamar cleanup no blocksFactory
        blocksFactory.CleanUpParameter();
    }
}
