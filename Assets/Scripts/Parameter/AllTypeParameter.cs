using UnityEngine;
//using System.Collections;
using System.Collections.Generic;
//using UnityEngine.EventSystems;

public class AllTypeParameter : ParameterSetup
{
    public AllTypeParameter() {
        type = ParameterType.AllTypeParameter;

        sections = new List<ParameterSections> (){
            ParameterSections.IntValues,
            ParameterSections.IntOperations,
            ParameterSections.DirectionValues,
            ParameterSections.MaterialValues,
            ParameterSections.BoolValues,
            ParameterSections.BoolAllComparisonOperations,
            ParameterSections.BoolIntComparisonOperations,
            ParameterSections.BoolLogicOperations
            
        };

        CanInputInt = true;
    }

    public override void Setup(string paramName) {
        if (paramName == null) paramName = "Inserir Valor";
        base.Setup(paramName);
    }

}
