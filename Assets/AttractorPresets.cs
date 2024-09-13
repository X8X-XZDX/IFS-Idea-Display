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

    [Serializable]
    public struct ProceduralSettings {
        public int instructionCount;
        public Vector3 scaleMin;
        public Vector3 scaleMax;
        public Vector3 shearXMin;
        public Vector3 shearXMax;
        public Vector3 shearYMin;
        public Vector3 shearYMax;
        public Vector3 shearZMin;
        public Vector3 shearZMax;
        public Vector3 rotateMin;
        public Vector3 rotateMax;
        public Vector3 translateMin;
        public Vector3 translateMax;
    }

    Vector3 GenerateRandomVector(Vector3 min, Vector3 max) {
        return new Vector3(Random.Range(min.x, max.x), Random.Range(min.y, max.y), Random.Range(min.z, max.z));
    }

    List<TransformInstructions> ProceduralInstructions() {
        List<TransformInstructions> instructions = new List<TransformInstructions>();

        for (int i = 0; i < proceduralSettings.instructionCount; ++i) {
            TransformInstructions t = new TransformInstructions();

            t.scale = GenerateRandomVector(proceduralSettings.scaleMin, proceduralSettings.scaleMax);
            t.shearX = GenerateRandomVector(proceduralSettings.shearXMin, proceduralSettings.shearXMax);
            t.shearY = GenerateRandomVector(proceduralSettings.shearYMin, proceduralSettings.shearYMax);
            t.shearZ = GenerateRandomVector(proceduralSettings.shearZMin, proceduralSettings.shearZMax);
            t.rotate = GenerateRandomVector(proceduralSettings.rotateMin, proceduralSettings.rotateMax);
            t.translate = GenerateRandomVector(proceduralSettings.translateMin, proceduralSettings.translateMax);

            instructions.Add(t);
        }

        return instructions;
    }
}
