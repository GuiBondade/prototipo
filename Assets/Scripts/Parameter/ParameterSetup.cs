using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
//using UnityEngine.EventSystems; */
using TMPro;

public enum ParameterSections {
    BoolValues,
    BoolAllComparisonOperations,
    BoolIntComparisonOperations,
    BoolLogicOperations,
    IntValues,
    IntOperations,
    DirectionValues,
    MaterialValues
}

public enum ParameterType {
    AllTypeParameter,
    BooleanParameter,
    DirectionParameter,
    IntParameter,
    MaterialParameter
}

public abstract class ParameterSetup : MonoBehaviour//, IIdentificadorScript
{   
    // criação de bloco: definicao de tipo por inspector (dicionario onde pode ser adicionado parametros e cada parametro tem uma variavel tipo a ser atribuida)
        // alterar imagem do slot, que nem no scratch (int é quadrado, boolean é losango, etc) [cor/contorno também?]

    public ReferenceHolder refs;

    [HideInInspector] public GameObject inputTextInstance; 
    
    [SerializeField] public ParameterType type;
    [SerializeField] protected bool CanInputInt;
    [SerializeField] public List<ParameterSections> sections; 

    // salvar em arquivo JSON ou algum BD?? ve depois
    //
    // VE JEITO MELHOR QUE ESSE DICIONARIO POR FAVOR
    //
    private readonly Dictionary<ParameterSections, List<string>> SectionValues = new()
    {
        {
            ParameterSections.BoolValues,
            new List<string> {"falso", "verdadeiro"}
        },
        {
            ParameterSections.BoolAllComparisonOperations,
            new List<string> {"==", "!="}
        },
        {
            ParameterSections.BoolIntComparisonOperations,
            new List<string> {">", ">=", "<", "<="}
        },
        {
            ParameterSections.BoolLogicOperations,
            new List<string> {"E", "OU"}
        },
        {
            ParameterSections.IntValues,
            new List<string> {"Digitar..."}
        },
        {
            ParameterSections.IntOperations,
            new List<string> {"+", "-", "*", "/"}
        },
        {
            ParameterSections.DirectionValues,
            new List<string> {"para cima", "para baixo", "para esquerda", "para direita"}
        },
        {
            ParameterSections.MaterialValues,
            new List<string> {"madeira", "pedra", "ferro"}
        }
    };

    public void Initialize(ReferenceHolder references)
    {
        this.refs = references;
    }

    public string GetIntValueAt(int index) {
        var valuesList = SectionValues[ParameterSections.IntValues];
        return valuesList[index];
    }
    
    public virtual void Setup(string name) // trocar essa função INTEIRA direto pelo InitializeSection()? (melhor nao, ja que essa da setup em varios sections e outras coisas)
    {
        var placeholderTMPText = refs.placeholderLabel.GetComponent<TMP_Text>();
        placeholderTMPText.text = name;
        placeholderTMPText.ForceMeshUpdate();

        foreach (var section in sections){
            var sectionOptions = InitializeSection(section, refs.sectionContent.transform);

            foreach (var value in SectionValues[section]) {
                sectionOptions.valueList.Add(value);
            }
        }

        refs.sectionContent.GetComponent<AdjustWidthByText>().AdjustWidth();// precisa?
        
        if (CanInputInt) { 
            var inputText = Instantiate(refs.inputTextPrefab, refs.childParamLayout.transform);
            refs.inputTextInstance = inputText;
        } 
    }

    
}

