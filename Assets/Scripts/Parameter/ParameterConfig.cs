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

    public void Start() {
        ParameterPrefab = Resources.Load<GameObject>("GenericParameter");
    }

    public void SetInputText(GameObject inputTextInstance) {
        inputText = inputTextInstance;
    }

    public void OnTriggerButtonPress() {
        refs.selectionArea.SetActive(true);
    }

    public void OnBlockerPress() {
        refs.selectionArea.SetActive(false);
    }

    public void SectionPressionado(GameObject sectionPressed) { 
        // da pra subtituir sectionPressed por this?
        if (sectionPressed != refs.selectedSection) {
            var sectionPressedInfo = sectionPressed.GetComponent<SectionInfo>();
            UpdateSelectedOption(sectionPressed);
            UpdateValues(sectionPressedInfo);
        }
    }

    public void ValuePressionado(GameObject valuePressed) {
        if (valuePressed != refs.selectedValue) { 
            ResetParameterState();

            var valuePressedInfo = valuePressed.GetComponent<ValueInfo>(); //value info
            switch (valuePressedInfo.sectionCurrent) {
                case ParameterSections.BoolValues:
                case ParameterSections.DirectionValues:
                case ParameterSections.MaterialValues:
                    break; // nada
                case ParameterSections.IntValues:
                    if (valuePressedInfo.value == GetComponent<ParameterSetup>().GetIntValueAt(0)) inputText.SetActive(true);
                    break; // ativar InputInt GO caso seja o valor pressionado(index 0)
                case ParameterSections.BoolAllComparisonOperations:
                    if (valuePressedInfo.sectionCurrent != sectionSelected){
                        InstantiateParameter<BooleanParameter>(ref refs.Value_1, true, "Boolean");
                        InstantiateParameter<BooleanParameter>(ref refs.Value_2, false, "Boolean");
                    }
                    break; // adicionar dois prefab param all sections (como first e last) (irmaos de genericParameter)
                case ParameterSections.IntOperations: // adicionar dois prefabs param int (como first e last) (irmaos de genericParameter)
                case ParameterSections.BoolIntComparisonOperations:
                    if (valuePressedInfo.sectionCurrent != sectionSelected){
                        InstantiateParameter<IntParameter>(ref refs.Value_1, true, "Int");
                        InstantiateParameter<IntParameter>(ref refs.Value_2, false, "Int");
                    }
                    break; // adicionar dois prefabs param int, um como first e outro como last (irmaos de genericParameter)
                case ParameterSections.BoolLogicOperations:
                    if (valuePressedInfo.sectionCurrent != sectionSelected){
                        InstantiateParameter<BooleanParameter>(ref refs.Value_1, true, "Boolean");
                    }
                    break; // adicionar um prefab param bool (como last) (irmao de genericParameter)
                default:
                    Debug.Log("Deu erro ao pressionar valor, parece que tem opção de seção a mais no Setup(ParameterSections) do que no Config(ValuePressionado");
                    break;
            }
            // refs.placeholderLabel vai ser o valor final(por enquanto), depois vai ter somente valor equivalente ao atribuido ao valor final
            refs.placeholderLabel.GetComponent<TMP_Text>().text = valuePressedInfo.label.text;
            // atribuir valor no setup/config para leitura pratica no save
            sectionSelected = valuePressedInfo.GetComponent<ValueInfo>().sectionCurrent;
            UpdateSelectedOption(valuePressed);
        }
        refs.selectionArea.SetActive(false);
    }

    public void InstantiateParameter<T>(ref GameObject instanceReference, bool isFirst, string label) where T : ParameterSetup
    {
        instanceReference = Instantiate(ParameterPrefab, transform);
        if (isFirst) instanceReference.transform.SetAsFirstSibling();
        else instanceReference.transform.SetAsLastSibling();

        T parameterComponent = instanceReference.AddComponent<T>(); // nao ta dando certo
        parameterComponent.Initialize(instanceReference.GetComponent<ReferenceHolder>());
        parameterComponent.Setup(label);
        LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
    }

    /* public void OperationValuePressionado(GameObject valuePressed) { // JUNTAR COM VALUE PRESSIONADO (PODE DELETAR)
        if (valuePressed != refs.selectedValue) {
            UpdateSelectedOption(valuePressed);
            ResetParameterState();

            //switch pra saber se o valor pressionado é de qual lista de operações booleanas
            // esse codigo é de all comparison operações, seria int comparison, caso o prefab instanciado fosse int, etc
            var dropdownLeft = Instantiate(ParameterPrefab,transform.parent); // pai do genericParameter (prefab que englobara um genericParameter, mas pode ter 3 no caso de operação)
            dropdownLeft.transform.SetAsFirstSibling();
            refs.Value_1 = dropdownLeft;
            var dropdownRight = Instantiate(ParameterPrefab,transform.parent);
            dropdownRight.transform.SetAsLastSibling();
            refs.Value_2 = dropdownRight; 

            refs.placeholderLabel.GetComponent<TMP_Text>().text = valuePressed.GetComponent<OptionInfo>().label.text;// refs.placeholderLabel vai ser o valor final, ou vai ter valorequivalente ao atribuido ao valor final
        }
        refs.selectionArea.SetActive(false);
    } */

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
        //RemoverPreviousrefs.selectedSection(refs.selectedSection);//if (inputText != null) inputText.SetActive(false);
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
            valueInfo.sectionCurrent = sectionInfo.sectionCurrent;
            valueInfo.value = valor;
        }
    }

     public void ResetParameterState() { //sepa que é melhor por reset state
        if (inputText != null) inputText.SetActive(false);
        if (refs.Value_1 != null){
             Destroy(refs.Value_1);
             refs.Value_1 = null;
        }
        if (refs.Value_2 != null){
             Destroy(refs.Value_2);
             Debug.Log("Value_2 apos destruição" + refs.Value_2);
             refs.Value_2 = null;
        }
    }
}
