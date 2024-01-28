using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[ExecuteAlways]
public class RaymarchRuntimeManager : MonoBehaviour
{
    public RaymarchRenderFeature feature;
    private List<BaseShape> _shapes;

    private void Awake() {
        SetShapes();
    }
    
    private void Update() {
        SetShapes();
    }

    private void SetShapes() {
        if (feature == null) return;
    }
}
