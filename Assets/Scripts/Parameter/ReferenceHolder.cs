using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReferenceHolder : MonoBehaviour
{
    [HideInInspector] public GameObject rootParameter;

    public GameObject parentesesInstance;

    public GameObject childParamLayout;

    public GameObject inputTextPrefab;

    public GameObject inputTextInstance;

    public GameObject placeholderLabel;

    public GameObject sectionContent;

    public GameObject sectionPrefab;

    public ParameterType type;

    public GameObject leftOperand; // INUTIL, agora que tem blocksFactory nem faz sentido mais guardar ref

    public GameObject rightOperand; // INUTIL, agora que tem blocksFactory nem faz sentido mais guardar ref

    public GameObject selectionArea;

    public GameObject valueContent;

    public GameObject valuePrefab;

    public GameObject selectedValue;

    [HideInInspector] public GameObject selectedSection;
}
