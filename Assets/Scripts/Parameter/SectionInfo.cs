using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class SectionInfo : OptionInfo 
{
    public List<string> valueList;

    protected override void ConfigurarBotao() {
        btn.onClick.AddListener(() => parentParamConfig.SectionPressionado(this));
    }

}