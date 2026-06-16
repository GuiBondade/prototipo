using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class ValueInfo : OptionInfo 
{
    [HideInInspector] public string value; // oq vai aparecer
    [HideInInspector] public string varName; // referencia pra pegar depois(vai ter o mesmo texto que o value, talvez por um bool isVar no lugar, e dai pegar o value depois) 
    
    protected override void ConfigurarBotao() {
        btn.onClick.AddListener(() => parentParamConfig.ValuePressionado(this));
    }
}