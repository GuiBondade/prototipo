using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasAreasManager : MonoBehaviour
{
    public static CanvasAreasManager instancia { get; private set; }

    public RectTransform canvasProgramming;
    public RectTransform visibleWorkspace;
    public RectTransform contentWorkspace;

    void Awake() 
    {
        instancia = this;
    }
}
