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
            new List<string> {"Digitar Valor"}
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
        //OnInitialized();
    }

    //protected virtual void OnInitialized() { }

    public string GetIntValueAt(int index) {
        var valuesList = SectionValues[ParameterSections.IntValues];
        return valuesList[index];
    }
    
    public virtual void Setup(string name) 
    {
        refs.placeholderLabel.GetComponent<TMP_Text>().text = name;

        foreach (var section in sections){
            var sectionInstance = Instantiate(refs.sectionPrefab, refs.sectionContent.transform); //  adicionar optiondata pra cada opção *dependendo do tipo do parametro*
            //section.GetComponent<OptionInfo>().tipo = sectionName; //onde vai ser decidido as seções de section
            var sectionOptions = sectionInstance.GetComponent<SectionInfo>();
            sectionOptions.label.text = section.ToString();
            sectionOptions.label.ForceMeshUpdate();
            sectionOptions.sectionCurrent = section;
        
            // tem que settar numa var do option info a seção de cada option(vai ter o enum da seção, e a lista dos valores(na hora de instanciar os valores, le os valores da lista num foreach, e adicionar somente um item na lista(que é o valor lido), e vai ter o mesmo enum da seção pra identificação))
                // pra usar obter valores (no parameter config) atraves dos valores do option info
            foreach (var value in SectionValues[section]) {
                sectionOptions.valueList.Add(value);
            }
        }
        refs.sectionContent.GetComponent<AdjustWidthByText>().AdjustWidth();
        if (CanInputInt) { 
            var inputTextInstance = Instantiate(refs.inputTextPrefab, refs.childParamLayout.transform);
            GetComponent<ParameterConfig>().SetInputText(inputTextInstance);
        } 
    }

    
}

