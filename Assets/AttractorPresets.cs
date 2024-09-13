using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class TransformSet : MonoBehaviour {
    public enum AffinePreset {
        SierpinskiTriangle2D,
        Vicsek2D,
        SierpinskiCarpet2D,
        SierpinskiTriangle3D,
        Vicsek3D,
        SierpinskiCarpet3D,
        Procedural
    } 

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

    List<TransformInstructions> ProceduralInstructions() {
        List<TransformInstructions> instructions = new List<TransformInstructions>();
            
        int instructionCount = 9;

        for (int i = 0; i < instructionCount; ++i) {
            TransformInstructions t = new TransformInstructions();

            t.scale = new Vector3(Random.value, Random.value, Random.value);
            t.shearX = new Vector3(0, Random.value * 0.1f, Random.value * 0.1f);
            t.shearY = new Vector3(Random.value * 0.1f, 0, Random.value * 0.1f);
            t.shearZ = new Vector3(Random.value * 0.1f, Random.value * 0.1f, 0);
            t.rotate = new Vector3(Random.value * 360.0f, Random.value * 360.0f, Random.value * 360.0f);
            t.translate = new Vector3(Random.value * 10, Random.value * 10, Random.value * 10);

            instructions.Add(t);
        }

        return instructions;
    }
}