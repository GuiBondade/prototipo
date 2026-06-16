using UnityEngine;
using System;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "SectionDatabase", menuName = "Block Programming/Section Database")]
public class SectionDatabase : ScriptableObject
{
    [Serializable]
    public struct SectionEntry{
        public ParameterSections section;
        public List<string> values;
    }

    private static readonly List<string> EmptyList = new List<string>(0);//pra retornar caso der erro no get values
    [SerializeField] private List<SectionEntry> sectionEntries;
    public List<string>[] valuesArray;

    public void Awake() {
        if (valuesArray == null) Initialize();
    }

    public void Initialize() {
        var totalSections = Enum.GetValues(typeof(ParameterSections)).Length;
        valuesArray = new List<string>[totalSections];

        foreach (var entry in sectionEntries) {
            var index = (int)entry.section;
            valuesArray[index] = entry.values;
        }
    }

    public List<string> GetValuesForSection(ParameterSections section) {
        if (valuesArray == null) Initialize();

        var index = (int)section;
        if (index >= 0 && index < valuesArray.Length && valuesArray[index] != null) {
            return valuesArray[index];
        }             
        
        Debug.Log("Não conseguiu pegar os valores da seção");
        return EmptyList;
    }
}
