using UnityEngine;
//using System.Collections;
using System.Collections.Generic;
//using UnityEngine.EventSystems;

public class DirectionParameter : ParameterSetup
{
    public DirectionParameter() {
        type = ParameterType.DirectionParameter;

        sections = new List<ParameterSections> (){
            ParameterSections.DirectionValues
        };

        CanInputInt = false;
    }
    public override void Setup(string paramName) {
        if (paramName == null) paramName = "Inserir Valor Direção";
        base.Setup(paramName);
    }

}
