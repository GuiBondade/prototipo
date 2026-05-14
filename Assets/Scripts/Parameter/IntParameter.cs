using UnityEngine;
//using System.Collections;
using System.Collections.Generic;
//using UnityEngine.EventSystems;

public class IntParameter : ParameterSetup
{
    public IntParameter() {
        sections = new List<ParameterSections> (){
            ParameterSections.IntValues,
            ParameterSections.IntOperations
        };

        CanInputInt = true;
    }

    public override void Setup(string paramName) {
        if (paramName == null) paramName = "Inserir Valor Inteiro";
        base.Setup(paramName);
    }

}
