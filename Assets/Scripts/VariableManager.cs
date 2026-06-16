using System;
using System.Collections.Generic;
using UnityEngine;

public class VariableManager : MonoBehaviour
{
    public event Action<string, string> OnVariableRenamed; 
    public event Action<string, ParameterType> OnVariableRetyped; 

    public static VariableManager instancia { get; private set; }

    private int varCopyCounter = 0;
    public string defaultVarFirstOption;
    public string defaultVarName;

    public struct VariableInfo {
        public ParameterType type; // tipo de valor que recebe como parametro
        public ParameterSections section; // seção que faz parte [values somente (operações não entra)]
        public string value; // poderia ser um id pro parametro root, pra depois descobrir o valor, sei la
    }

    public Dictionary<string, VariableInfo> variables /* {get; private set;} */ = new Dictionary<string, VariableInfo>();
    private List<string> variablesNames = new List<string>(); //pra evitar alocação na memória de new()

    private void Awake()
    {
        // Padrão de segurança para Singletons em Unity
        if (instancia != null && instancia != this)
        {
            Destroy(gameObject);
            return;
        }
        instancia = this;
    }
    private void OnDestroy()
    {
        if (instancia == this)
        {
            instancia = null;
        }
    }

    public void DebugLog() { // PODE DELETAR, É SO PRA DEBUG
        foreach (KeyValuePair<string, VariableInfo> par in variables) {
            Debug.Log($"key: {par.Key}, type: {par.Value.type}");
        }
    } 

    public string CreateVariable(string name) {
        string formattedName = name.Trim();

        if (string.IsNullOrEmpty(formattedName))
        {
            formattedName = defaultVarName; // newVar
        }

        string finalName = formattedName;
        varCopyCounter = 2; 

        while (variables.ContainsKey(finalName))
        {
            finalName = $"{formattedName} ({varCopyCounter})";
            varCopyCounter++;
        }

        variables[finalName] = new VariableInfo();
        Debug.Log(variables[finalName].type.ToString());
        return finalName;
    }

    public void DeleteVariable(string name){
        if (variables.ContainsKey(name)) {
            // UpdateVariableName(name, defaultOption (selecionarVariavel) );
            //UpdateVariableData(name, ParameterType.Default, ParameterSections.Default, null);
            variables.Remove(name);
            OnVariableRenamed?.Invoke(name, null);
        } else {
            Debug.Log("Variável a ser deletada não foi encontrada");
        }
    }

    public void Clear() {
        variables.Clear();
    }

    public void UpdateVariableData(string name, ParameterType type, ParameterSections section, string value) {
        if (!variables.ContainsKey(name)) {
            name = CreateVariable(name);
            Debug.Log("variável criada ao atualizar dados: " + name);
        }
        var varInfo = new VariableInfo();
        varInfo.type = type;
        varInfo.section = section;
        varInfo.value = value; // depois ve isso direito
        Debug.Log("Update Variable Data: " + name + ", " + type);
        variables[name] = varInfo;
        OnVariableRetyped?.Invoke(name, type);
    }

    public string UpdateVariableName(string oldName, string newName) {
        var varInfo = variables[oldName];
        variables.Remove(oldName);
        var newVarName = CreateVariable(newName);
        variables[newVarName] = varInfo;

        OnVariableRenamed?.Invoke(oldName, newVarName);

        return newVarName;
    }

    /* 
    public void UpdateAllVariables() {
        foreach (key and value in dictionary) {
            UpdateVariableData(key, vale.type, value.section, value.value)
        }
    }    
     */

    public List<string> GetVarNamesBySection(ParameterSections section) {
        variablesNames.Clear();
        foreach (KeyValuePair<string, VariableInfo> par in variables) {
            if (par.Value.section == section) {
                variablesNames.Add(par.Key);
            }
        }
        return variablesNames;
    }

    public ParameterSections GetVarSectionByName(string name) {
        foreach (KeyValuePair<string, VariableInfo> par in variables) {
            if (par.Key == name) {
                return par.Value.section;
            }
        }
        Debug.Log("Tentou pegar section de variável por nome e não conseguiu");
        return ParameterSections.BoolValues;
    }

    public ParameterType GetVarTypeByName(string name) {
        foreach (KeyValuePair<string, VariableInfo> par in variables) {
            Debug.Log(par.Key + " é " + par.Value.type);
            if (par.Key == name) {
                return par.Value.type;
            }
        }
        Debug.Log("Tentou pegar tipo de variável por nome e não conseguiu");
        return ParameterType.BooleanParameter;
    }
}