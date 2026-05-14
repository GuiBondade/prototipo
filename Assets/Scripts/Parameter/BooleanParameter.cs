using UnityEngine;
//using System.Collections;
using System.Collections.Generic;
//using UnityEngine.EventSystems;

public class BooleanParameter : ParameterSetup
{
    public BooleanParameter() {
        sections = new List<ParameterSections> (){
            ParameterSections.BoolValues,
            ParameterSections.BoolAllComparisonOperations,
            ParameterSections.BoolIntComparisonOperations,
            ParameterSections.BoolLogicOperations
        };

        CanInputInt = false;
    }

    public override void Setup(string paramName) {
        if (paramName == null) paramName = "Inserir Valor Booleano";
        base.Setup(paramName);
    }

}
