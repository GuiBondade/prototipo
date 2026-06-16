using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ReferenceHolder : MonoBehaviour
{
    [HideInInspector] public GameObject rootParameter;

    public BlocksFactory blocksFactory;

    public VariableManager variableManager;

    public GameObject parentesesInstance;

    public GameObject childParamLayout;

    public GameObject inputTextPrefab;

    public GameObject inputTextInstance;

    public TMP_Text placeholderLabel;

    public GameObject sectionContent;

    public GameObject sectionPrefab;

    [HideInInspector] public ParameterType type;

    public ReferenceHolder leftOperand;

    public ReferenceHolder rightOperand;

    public GameObject selectionArea;

    public GameObject valueContent;

    public GameObject valuePrefab;

    public ValueInfo selectedValueVisual;

    public ParameterSections selectedValueSection;
    
    public string selectedValueKey; 

    public string selectedVariableName;

    public SectionInfo selectedSection;

    void Awake() {
        blocksFactory = BlocksFactory.instance;
        variableManager = VariableManager.instancia;
    }

    public void HandleVariableRenamed(string oldName, string newName) {
        if (selectedVariableName == oldName) {
            selectedVariableName = newName;
            placeholderLabel.text = newName; 
        }
    }

    public void HandleVariableRetyped(string name, ParameterType variableType) {
        if (selectedVariableName == name) {
            Debug.Log(variableType + " == " + name);
            if (variableType != type) {
                placeholderLabel.text = blocksFactory.blockRegistry.GetParamData(type).defaultParamName;
                placeholderLabel.ForceMeshUpdate();
                GetComponent<AdjustWidthByText>().AdjustWidth();
                selectedValueKey = null;
                selectedVariableName = null;
            }
        }
    }
}
