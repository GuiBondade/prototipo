using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewParamData", menuName = "Block Programming/Parameter Data")]
public class ParamData : ScriptableObject
{
    public ParameterType type;
    public string defaultParamName;
    public bool CanInputInt;
    public List<ParameterSections> sections;
}
