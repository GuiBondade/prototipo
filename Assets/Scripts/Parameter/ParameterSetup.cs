using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
//using UnityEngine.EventSystems; */
using TMPro;

public enum ParameterSections {
    BoolValues, //
    BoolAllComparisonOperations,
    BoolIntComparisonOperations,
    BoolLogicOperations,
    IntValues, //
    IntOperations,
    DirectionValues, //
    MaterialValues //
}

public enum ParameterType {
    BooleanParameter,
    AllTypeParameter,
    DirectionParameter,
    IntParameter,
    MaterialParameter
}

public class ParameterSetup : MonoBehaviour//, IIdentificadorScript
{   
    // ???criação de bloco: definicao de tipo por inspector (dicionario onde pode ser adicionado parametros e cada parametro tem uma variavel tipo a ser atribuida)
        // alterar imagem do slot, que nem no scratch (int é quadrado, boolean é losango, etc) [cor/contorno também?]

    public ReferenceHolder refs;
    [SerializeField] private SectionDatabase sectionDatabase;

    [HideInInspector] public GameObject inputTextInstance; 
    
    [HideInInspector] public ParamData paramData;

    public string GetValueAt(ParameterSections section, int index) {  
        var valuesList = sectionDatabase.GetValuesForSection(section);
        
        return valuesList[index];
    }
    
    public virtual void Setup(string name) // trocar essa função INTEIRA direto pelo InitializeSection()? (melhor nao, ja que essa da setup em varios sections e outras coisas)
    {
        refs.placeholderLabel.text = string.IsNullOrEmpty(name) ? paramData.defaultParamName : name;
        refs.placeholderLabel.ForceMeshUpdate();

        Debug.Log(string.Join(" ,", paramData.sections));

        foreach (var section in paramData.sections){
            var sectionOptions = refs.blocksFactory.InitializeSection(section, refs.sectionContent.transform);

            // if var troca por variable manager.GetVarNamesForSection
            var values = sectionDatabase.GetValuesForSection(section);
            foreach (var value in values) {
                sectionOptions.valueList.Add(value);
            }
        }

        refs.sectionContent.GetComponent<AdjustWidthByText>().AdjustWidth();// precisa?
        
        if (paramData.CanInputInt) { 
            var inputText = Instantiate(refs.inputTextPrefab, refs.childParamLayout.transform);
            refs.inputTextInstance = inputText;
        } 

        refs.variableManager.OnVariableRenamed += refs.HandleVariableRenamed;
        refs.variableManager.OnVariableRetyped += refs.HandleVariableRetyped;
    }
}

