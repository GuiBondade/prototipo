using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using TMPro;

public class CreateVariableUI : MonoBehaviour
{
    public TMP_InputField inputVarName;
    public TMP_Dropdown varTypeDropdown;

    private VariableManager variableManager; 

    private int selectedTypeIndex = -1; //qual tipo da variavel criada (precisa ser index?)

    [SerializeField] private string _variableName;
    public string variableName {
        get => _variableName;
        set => _variableName = value;
    }

    void Awake() {
        variableManager = VariableManager.instancia;
        Debug.Log(variableManager);
    }

    public void Setup() {
        if (variableManager == null) variableManager = VariableManager.instancia;
    
        variableName = variableManager.CreateVariable(variableManager.defaultVarName); // vai ser "Nova Variável", nova referencia dentro do variable Manager  
        
        varTypeDropdown.value = 0; // deixa o default
        selectedTypeIndex = -1; // deixa o default

        inputVarName.text = variableName;
        Debug.Log(variableName);
    }

    // DROPDOWN DE TIPOS MOSTRA SÓ AS SIMPLES (sem allType) harcoded nos options do proprio dropdown

    public void SelectVarType(int index) {
        Debug.Log("selectedTypeIndex: " + selectedTypeIndex);
        if (index != selectedTypeIndex) {
            selectedTypeIndex = index;
            string varTypeString = varTypeDropdown.options[index].text; 
            ParameterSections section = ParameterSections.BoolValues;
            ParameterType varType;
            if (Enum.TryParse<ParameterType>(varTypeString, out ParameterType type)) {
                varType = type;
            } else {
                varType = ParameterType.BooleanParameter;
            }
            Debug.Log("Tipo clicado no dropdown das variáveis foi traduzido para ParameterType: " + varTypeString);
            switch (varType) {
                case ParameterType.BooleanParameter:
                    section = ParameterSections.BoolValues;
                    break;
                case ParameterType.IntParameter:
                    section = ParameterSections.IntValues;
                    break;
                case ParameterType.DirectionParameter:
                    section = ParameterSections.DirectionValues;
                    break;
                case ParameterType.MaterialParameter:
                    section = ParameterSections.MaterialValues;
                    break;
            }
            variableManager.UpdateVariableData(variableName, varType, section, null); // ja chama handle variable retyped
        }
    }

    public void OnInputFieldEndEdit(string text) {
        var newName = variableManager.UpdateVariableName(variableName, text);
        variableName = newName;
        inputVarName.text = newName;
    }
}