using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReferenceHolder : MonoBehaviour
{
    public GameObject childParamLayout;

    public GameObject inputTextPrefab;

    public GameObject placeholderLabel;

    public GameObject sectionContent;

    public GameObject sectionPrefab;

    public GameObject Value_1; //[HideInInspector]

    public GameObject Value_2; //[HideInInspector]

    public GameObject selectionArea;

    public GameObject valueContent;

    public GameObject valuePrefab;

    [HideInInspector] public GameObject selectedValue;

    [HideInInspector] public GameObject selectedSection;
}
