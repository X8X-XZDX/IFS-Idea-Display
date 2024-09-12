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

        public TransformInstructions(Vector3 scale, Vector3 shearX, Vector3 shearY, Vector3 shearZ, Vector3 rotate, Vector3 translate) {
            this.scale = scale;
            this.shearX = shearX;
            this.shearY = shearY;
            this.shearZ = shearZ;
            this.rotate = rotate;
            this.translate = translate;
        }

        public static TransformInstructions operator +(TransformInstructions a, TransformInstructions b) 
            => new TransformInstructions(Vector3.Scale(a.scale, b.scale), a.shearX + b.shearX, a.shearY + b.shearY, a.shearZ + b.shearZ, a.rotate + b.rotate, a.translate + b.translate);
    }
    
    public AffinePreset affinePreset;

    public bool resetToPreset = false;
    
    public List<TransformInstructions> transformSet = new List<TransformInstructions>();
    
    public TransformInstructions postTransform = new TransformInstructions();

    public static TransformInstructions GetIdentity() {
        TransformInstructions identity = new TransformInstructions();
        
        identity.scale = new Vector3(1, 1, 1);

        return identity;
    }

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
