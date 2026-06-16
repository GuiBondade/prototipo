using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ParameterConfig : MonoBehaviour
{
    public ReferenceHolder refs;

    private BlockerManager blocker;

    public void Start() {
        blocker = BlockerManager.instancia;
    }

    public void OnTriggerButtonPress() {
        blocker.SetUIOnBlocker(refs.selectionArea);
    }

    public void SectionPressionado(SectionInfo sectionPressed) { 
        // da pra subtituir sectionPressed por this?
        if (sectionPressed != refs.selectedSection) { // se mudou o section selecionado
            UpdateValues(sectionPressed);
            UpdateSelectedSection(sectionPressed);
            if (refs.selectedValueVisual != null) {
                if (sectionPressed.sectionCurrent == refs.selectedValueSection) {
                    refs.selectedValueVisual.checkmark.enabled = true;
                } else {
                    refs.selectedValueVisual.checkmark.enabled = false;
                }
            }
        }
    }

    public void ValuePressionado(ValueInfo valuePressed) {
        if (refs.selectedValueKey == null || valuePressed.value != refs.selectedValueKey) { 
            // refs.placeholderLabel vai ser o valor final(por enquanto), depois vai ter somente valor equivalente ao atribuido ao valor final
            // atribuir valor no setup/config para leitura pratica no save
            string valueText = valuePressed.label.text;
            refs.placeholderLabel.text = valueText; 
            refs.placeholderLabel.ForceMeshUpdate();
            if (valuePressed.sectionCurrent != refs.selectedValueSection) {
                // trocou de seção
                ResetParameterState();
                switch (valuePressed.sectionCurrent) {
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
            if (refs.variableManager.variables.ContainsKey(valueText)) {
                refs.selectedVariableName = valueText;
            } else if (valuePressed.sectionCurrent == ParameterSections.IntValues) {
                if (valuePressed.value == GetComponent<ParameterSetup>().GetValueAt(ParameterSections.IntValues, 0)) refs.inputTextInstance.SetActive(true);
            }
            Transform atual = transform.parent; // Começa do pai direto
            while (atual != null) {
                LayoutRebuilder.ForceRebuildLayoutImmediate(atual.GetComponent<RectTransform>());
                var componente = atual.GetComponent<ParameterConfig>();
                if (componente == null) break;
                atual = atual.parent; // Sobe um nível
            }

            GetComponent<AdjustWidthByText>().AdjustWidth();

            UpdateSelectedValue(valuePressed);
        }
        blocker.Reset();
    }

    public void InstantiateParameter(ref ReferenceHolder instanceReference, ParameterType type, bool isFirst, string label)
    {
        var parameterRefs = refs.blocksFactory.InitializeParameter(label, type, transform);
        instanceReference = parameterRefs;
        if (isFirst) parameterRefs.transform.SetAsFirstSibling();
        else parameterRefs.transform.SetAsLastSibling();

        LayoutRebuilder.ForceRebuildLayoutImmediate(parameterRefs.GetComponent<RectTransform>()); //precisa?
    }


    public void UpdateSelectedSection(SectionInfo sectionPressed) { // tem que ALTERAR porque ta so funfando pra section(?, nao sei mais)
        if (refs.selectedSection != null) refs.selectedSection.checkmark.enabled = false;
        refs.selectedSection = sectionPressed;
        sectionPressed.checkmark.enabled = true;
    }

    public void UpdateSelectedValue(ValueInfo valuePressed) { 
        if (refs.selectedValueVisual != null) refs.selectedValueVisual.checkmark.enabled = false;
        refs.selectedValueVisual = valuePressed;
        refs.selectedValueKey = valuePressed.value;
        refs.selectedValueSection = valuePressed.sectionCurrent;
        valuePressed.checkmark.enabled = true;
    }

    public void UpdateValues(SectionInfo sectionInfo) { //operations / values
        for (int i = refs.valueContent.transform.childCount - 1; i >= 0; i--)
        {
            var value = refs.valueContent.transform.GetChild(i);
            var valueInfo = value.GetComponent<ValueInfo>();
            refs.blocksFactory.CleanUpValue(valueInfo);
        }
        var section = sectionInfo.sectionCurrent;
        foreach (string valor in sectionInfo.valueList) {
            refs.blocksFactory.InitializeValue(valor, section, refs.valueContent.transform);
        }
        var variablesNames = refs.variableManager.GetVarNamesBySection(section);
        /* Debug.Log(refs.createdVar); // NÃO PRECISA MAIS EU ACHO, já que não tem mais problema setar VarX como VarX (já que o valor inicial vai ser setado na criação, a partir do section database somente, sem variáveis)
        variablesNames.Remove(refs.createdVar);  */
        // setar no escolher index 0 no varNameDropdown, passando pros operandos
        // impede uma variavel setar o valor como si mesma na criação

        // se setou index > 0 (ta setando valor de var) deve...


        foreach (string variable in variablesNames) {
            refs.blocksFactory.InitializeValue(variable, section, refs.valueContent.transform);
        }
        refs.valueContent.GetComponent<AdjustWidthByText>().AdjustWidth();
    }

    public void ResetValues() {
        Debug.Log(refs.selectedSection);
        if (refs.selectedSection is SectionInfo section) UpdateValues(section);
    }

    public void ResetParameterState() { // chamar cleanup no refs.blocksFactory
        if (refs.inputTextInstance != null) refs.inputTextInstance.SetActive(false);
        refs.parentesesInstance.SetActive(false);

        refs.selectedSection.checkmark.enabled = false;
        if (refs.selectedValueVisual != null) refs.selectedValueVisual.checkmark.enabled = false;

        refs.selectedSection = null;
        refs.selectedValueVisual = null;
        refs.selectedValueKey = null;

        if (refs.leftOperand != null) {
            refs.blocksFactory.CleanUpParameter(refs.leftOperand);
            refs.leftOperand = null;
        }
        if (refs.rightOperand != null) {
            refs.blocksFactory.CleanUpParameter(refs.rightOperand);
            refs.rightOperand = null;
        }
    }
}
