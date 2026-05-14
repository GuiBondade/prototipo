using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class ValueInfo : OptionInfo 
{
    [HideInInspector] public string value;
    
    protected override void ConfigurarBotao() {
        btn.onClick.AddListener(() => parentParamConfig.ValuePressionado(this.gameObject));
    }
}