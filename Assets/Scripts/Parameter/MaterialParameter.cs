using UnityEngine;
//using System.Collections;
using System.Collections.Generic;
//using UnityEngine.EventSystems;

public class MaterialParameter : ParameterSetup
{
    public MaterialParameter() {
        sections = new List<ParameterSections> (){
            ParameterSections.MaterialValues
        };

        CanInputInt = false;
    }
    
    public override void Setup(string paramName) {
        if (paramName == null) paramName = "Inserir Valor Material";
        base.Setup(paramName);
    }

}
