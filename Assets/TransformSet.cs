using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformSet : MonoBehaviour {

    [Serializable]
    public struct TransformInstructions {
        public Vector3 scale;
        public Vector3 shearX;
        public Vector3 shearY;
        public Vector3 shearZ;
        public Vector3 rotate;
        public Vector3 translate;
    }

    public enum AffinePreset {
        SierpinskiTriangle2D,
        Vicsek2D,
        SierpinskiCarpet2D,
        SierpinskiTriangle3D,
        Vicsek3D,
        SierpinskiCarpet3D
    } public AffinePreset affinePreset;

    public bool resetToPreset = false;
    
    public List<TransformInstructions> transformSet = new List<TransformInstructions>();
    
    public TransformInstructions postTransform = new TransformInstructions();

    List<TransformInstructions> SierpinskiTriangle2D() {
        List<TransformInstructions> instructions = new List<TransformInstructions>();
        
        TransformInstructions t = new TransformInstructions();

        Vector3[] translations = {
            new Vector3(-0.5f, -0.5f, 0.0f),
            new Vector3(0.0f, 0.36f, 0.0f),
            new Vector3(0.5f, -0.5f, 0.0f)
        };

        t.scale = new Vector3(0.5f, 0.5f, 0.5f);

        for (int i = 0; i < translations.Length; ++i) {
            t.translate = translations[i];
            instructions.Add(t);
        }

        return instructions;
    }

    List<TransformInstructions> Vicsek2D() {
        List<TransformInstructions> instructions = new List<TransformInstructions>();
        
        TransformInstructions t = new TransformInstructions();

        Vector3[] translations = {
            new Vector3(-0.5f, -0.5f, 0.0f),
            new Vector3(-0.5f, 0.5f, 0.0f),
            new Vector3(0.5f, 0.5f, 0.0f),
            new Vector3(0.5f, -0.5f, 0.0f),
            new Vector3(0.0f, 0.0f, 0.0f)
        };

        t.scale = new Vector3(0.33f, 0.33f, 0.33f);

        for (int i = 0; i < translations.Length; ++i) {
            t.translate = translations[i];
            instructions.Add(t);
        }

        return instructions;
    }

    List<TransformInstructions> SierpinskiCarpet2D() {
        List<TransformInstructions> instructions = new List<TransformInstructions>();
        
        TransformInstructions t = new TransformInstructions();

        Vector3[] translations = {
            new Vector3(-0.5f, -0.5f, 0.0f),
            new Vector3(-0.5f, 0.5f, 0.0f),
            new Vector3(0.5f, 0.5f, 0.0f),
            new Vector3(0.5f, -0.5f, 0.0f),
            new Vector3(-0.5f, 0.0f, 0.0f),
            new Vector3(0.5f, 0.0f, 0.0f),
            new Vector3(0.0f, 0.5f, 0.0f),
            new Vector3(0.0f, -0.5f, 0.0f)
        };

        t.scale = new Vector3(0.33f, 0.33f, 0.33f);

        for (int i = 0; i < translations.Length; ++i) {
            t.translate = translations[i];
            instructions.Add(t);
        }

        return instructions;
    }

    List<TransformInstructions> SierpinskiTriangle3D() {
        List<TransformInstructions> instructions = new List<TransformInstructions>();
        
        TransformInstructions t = new TransformInstructions();

        Vector3[] translations = {
            new Vector3(-0.5f, -0.5f, 0.5f),
            new Vector3(-0.5f, -0.5f, -0.5f),
            new Vector3(0.5f, -0.5f, 0.5f),
            new Vector3(0.5f, -0.5f, -0.5f),
            new Vector3(0.0f, 0.36f, 0.0f),
        };

        t.scale = new Vector3(0.5f, 0.5f, 0.5f);

        for (int i = 0; i < translations.Length; ++i) {
            t.translate = translations[i];
            instructions.Add(t);
        }

        return instructions;
    }
    
    List<TransformInstructions> Vicsek3D() {
        List<TransformInstructions> instructions = new List<TransformInstructions>();
        
        TransformInstructions t = new TransformInstructions();

        Vector3[] translations = {
            new Vector3(-0.5f, -0.5f, -0.5f),
            new Vector3(-0.5f, -0.5f, 0.5f),
            new Vector3(0.5f, -0.5f, -0.5f),
            new Vector3(0.5f, -0.5f, 0.5f),
            new Vector3(-0.5f, 0.5f, -0.5f),
            new Vector3(-0.5f, 0.5f, 0.5f),
            new Vector3(0.5f, 0.5f, -0.5f),
            new Vector3(0.5f, 0.5f, 0.5f),
            new Vector3(0.0f, 0.0f, 0.0f)
        };

        t.scale = new Vector3(0.33f, 0.33f, 0.33f);

        for (int i = 0; i < translations.Length; ++i) {
            t.translate = translations[i];
            instructions.Add(t);
        }

        return instructions;
    }

    List<TransformInstructions> SierpinskiCarpet3D() {
        List<TransformInstructions> instructions = new List<TransformInstructions>();
        
        TransformInstructions t = new TransformInstructions();

        Vector3[] translations = {
        new Vector3(-0.5f, -0.5f, -0.5f),
        new Vector3(-0.5f, -0.5f, 0.5f),
        new Vector3(0.5f, -0.5f, -0.5f),
        new Vector3(0.5f, -0.5f, 0.5f),
        new Vector3(-0.5f, 0.5f, -0.5f),
        new Vector3(-0.5f, 0.5f, 0.5f),
        new Vector3(0.5f, 0.5f, -0.5f),
        new Vector3(0.5f, 0.5f, 0.5f),
        new Vector3(-0.5f, 0.5f, 0.0f),
        new Vector3(0.5f, 0.5f, 0.0f),
        new Vector3(-0.5f, -0.5f, 0.0f),
        new Vector3(0.5f, -0.5f, 0.0f),
        new Vector3(0.0f, 0.5f, -0.5f),
        new Vector3(0.0f, 0.5f, 0.5f),
        new Vector3(0.0f, -0.5f, -0.5f),
        new Vector3(0.0f, -0.5f, 0.5f),
        new Vector3(-0.5f, 0.0f, -0.5f),
        new Vector3(0.5f, 0.0f, 0.5f),
        new Vector3(0.5f, 0.0f, -0.5f),
        new Vector3(-0.5f, 0.0f, 0.5f)
    };

        t.scale = new Vector3(0.33f, 0.33f, 0.33f);

        for (int i = 0; i < translations.Length; ++i) {
            t.translate = translations[i];
            instructions.Add(t);
        }

        return instructions;
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
