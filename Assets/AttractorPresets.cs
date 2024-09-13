using System;
using Random = UnityEngine.Random;
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

        for (int i = 0; i < this.randomInstructionCount; ++i) {
            instructions.Add(proceduralWizard.GenerateRandomInstructions());
        }

        return instructions;
    }
}
