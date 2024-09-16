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

        public static TransformInstructions operator +(TransformInstructions a, TransformInstructions b) {
            Quaternion q1 = Quaternion.Euler(a.rotate);
            Quaternion q2 = Quaternion.Euler(b.rotate);
            Quaternion q3 = q1 * q2;

            return new TransformInstructions(Vector3.Scale(a.scale, b.scale), a.shearX + b.shearX, a.shearY + b.shearY, a.shearZ + b.shearZ, q3.eulerAngles, a.translate + b.translate);    
        } 
    }
    
    public AffinePreset affinePreset;

    public int randomInstructionCount = 8;

    public bool resetToPreset = false;
    
    public List<TransformInstructions> transformSet = new List<TransformInstructions>();
    
    public TransformInstructions postTransform = new TransformInstructions();

    private ProceduralWizard proceduralWizard;

    public static TransformInstructions GetIdentity() {
        TransformInstructions identity = new TransformInstructions();
        
        identity.scale = new Vector3(1, 1, 1);

        return identity;
    }

    List<TransformInstructions> GetPreset(AffinePreset preset) {
        switch (preset) {
            case AffinePreset.SierpinskiTriangle2D:
                return SierpinskiTriangle2D();
            case AffinePreset.Vicsek2D:
                return Vicsek2D();
            case AffinePreset.SierpinskiCarpet2D:
                return SierpinskiCarpet2D();
            case AffinePreset.SierpinskiTriangle3D:
                return SierpinskiTriangle3D();
            case AffinePreset.Vicsek3D:
                return Vicsek3D();
            case AffinePreset.SierpinskiCarpet3D:
                return SierpinskiCarpet3D();
            case AffinePreset.Procedural:
                return ProceduralInstructions();
        }

        return SierpinskiTriangle2D();
    }

    public void ApplyPreset() {
        transformSet.Clear();

        transformSet = GetPreset(affinePreset);

        // Apply Translation Template
        if (affinePreset == AffinePreset.Procedural && proceduralWizard.translationTemplate != AffinePreset.Procedural) {
            List<TransformInstructions> templateSet = GetPreset(proceduralWizard.translationTemplate);

            for (int i = 0; i < transformSet.Count; ++i) {
                TransformInstructions t = transformSet[i];

                t.translate += templateSet[i % templateSet.Count].translate;

                transformSet[i] = t;
            }
        }
    }

    void OnEnable() {
        proceduralWizard = GetComponent<ProceduralWizard>();
        ApplyPreset();
    }

    void Update() {
        if (resetToPreset) {
            ApplyPreset();
            resetToPreset = false;
        }
    }
}
