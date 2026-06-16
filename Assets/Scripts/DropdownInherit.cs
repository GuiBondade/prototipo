using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using System.Collections.Generic;

public class DropdownInherit : TMP_Dropdown
{
    public VariableBlockUI variableBlockUI;
    private List<string> variableList = new List<string>();
    
    private int optionCounter = 0;

    public override void OnPointerClick(PointerEventData eventData) {
        if (!base.IsExpanded) {
            AtualizarLista(null, value);
        }

        base.OnPointerClick(eventData);
    }

    protected override GameObject CreateDropdownList(GameObject template)
    {
        // 1. Deixa a Unity criar a lista padrão primeiro
        GameObject dropdownList = base.CreateDropdownList(template);

        // 2. Acessa o componente que você precisa na lista criada
        if (dropdownList.TryGetComponent<AdjustWidthByText>(out AdjustWidthByText adjustWidth))
        {
            adjustWidth.AdjustWidth();
            Debug.Log("Componente acessado com sucesso na lista clonada!");
        } else {
            Debug.Log("Não achou o compoenente em: " + dropdownList);
        }

        return dropdownList;
    }

    public void AtualizarLista(string optionSelected, int index = -1) { // DA ERRO SE DELETAR UMA OPÇÃO QUE APARECE ANTES DA SELECIONADA
        int selectedIndex = 0;
        optionCounter = 0;
        if (optionSelected == null && index != -1) {
            optionSelected = options[index].text;
            Debug.Log("optionSelected: " + optionSelected + ", index: " + index);
        }
        base.ClearOptions();
        variableList.Clear();
        variableList.Add(VariableManager.instancia.defaultVarFirstOption); // vai ser "Selecionar Variável"

        foreach (KeyValuePair<string, VariableManager.VariableInfo> par in VariableManager.instancia.variables) {
            string varName = par.Key;
            variableList.Add(varName);
            optionCounter++;
            if (varName == optionSelected) {
                selectedIndex = optionCounter;
                Debug.Log("selectedIndex: " + selectedIndex);
            }
        }
        Debug.Log(string.Join(", ", variableList));

        base.AddOptions(variableList);
        this.value = selectedIndex;
        base.RefreshShownValue();
    }
}
