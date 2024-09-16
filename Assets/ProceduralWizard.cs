using System;
using Random = UnityEngine.Random;
using TransformInstructions = TransformSet.TransformInstructions;
using AffinePreset = TransformSet.AffinePreset;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralWizard : MonoBehaviour {
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
    public AffinePreset translationTemplate;

    Vector3 GenerateRandomVector(Vector3 min, Vector3 max) {
        return new Vector3(Random.Range(min.x, max.x), Random.Range(min.y, max.y), Random.Range(min.z, max.z));
    }

    public TransformInstructions GenerateRandomInstructions() {
        TransformInstructions t = new TransformInstructions();

        t.scale = GenerateRandomVector(scaleMin, scaleMax);
        t.shearX = GenerateRandomVector(shearXMin, shearXMax);
        t.shearY = GenerateRandomVector(shearYMin, shearYMax);
        t.shearZ = GenerateRandomVector(shearZMin, shearZMax);
        t.rotate = GenerateRandomVector(rotateMin, rotateMax);
        t.translate = GenerateRandomVector(translateMin, translateMax);

        return t;
    }
}
