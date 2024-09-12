using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class TransformSet : MonoBehaviour {

    [Serializable]
    public struct TransformInstructions {
        public Vector3 scale;
        public Vector3 shearX;
        public Vector3 shearY;
        public Vector3 shearZ;
        public Vector3 rotate;
        public Vector3 translate;
    }
    
    public AffinePreset affinePreset;

    public bool resetToPreset = false;
    
    public List<TransformInstructions> transformSet = new List<TransformInstructions>();
    
    public TransformInstructions postTransform = new TransformInstructions();

    void ApplyPreset() {
        transformSet.Clear();

        if (affinePreset == AffinePreset.SierpinskiTriangle2D) {
            transformSet = SierpinskiTriangle2D();
        }

        if (affinePreset == AffinePreset.Vicsek2D) {
            transformSet = Vicsek2D();
        }

        if (affinePreset == AffinePreset.SierpinskiCarpet2D) {
            transformSet = SierpinskiCarpet2D();
        }

        if (affinePreset == AffinePreset.SierpinskiTriangle3D) {
            transformSet = SierpinskiTriangle3D();
        }

        if (affinePreset == AffinePreset.Vicsek3D) {
            transformSet = Vicsek3D();
        }

        if (affinePreset == AffinePreset.SierpinskiCarpet3D) {
            transformSet = SierpinskiCarpet3D();
        }
    }

    void OnEnable() {
        ApplyPreset();
    }

    void Update() {
        if (resetToPreset) ApplyPreset();
    }
}
