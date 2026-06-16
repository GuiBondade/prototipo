using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ParamDatas", menuName = "ScriptableObjects/ParamDatas")]
public class ParamDatas : ScriptableObject
{
    [SerializeField] private List<ParamData> listDatas;

    private ParamData[] datasArray;

    public void Initialize() {
        int totalTypes = Enum.GetValues(typeof(ParameterType)).Length;
        datasArray = new ParamData[totalTypes];

        foreach (var data in listDatas) {
            if (data == null) continue;
            int index = (int)data.type;
            if (index >= 0 && index < totalTypes) datasArray[index] = data;
        }
    }

    public ParamData GetData(ParameterType type) {
        if (datasArray == null) Initialize();

        int index = (int)type;
        Debug.Log("Get data: " + index);
        if (index >= 0 && index < datasArray.Length && datasArray[index] != null)
        {
            return datasArray[index];
        }

        Debug.Log($"Erro no GetData, id: '{type}' não settado");
        return null;
    }
}