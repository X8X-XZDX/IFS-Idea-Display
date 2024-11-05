using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AffineTransformations : MonoBehaviour {

    public SetBlender setBlender;
    
    private List<Matrix4x4> affineTransforms = new List<Matrix4x4>();
    
    private Matrix4x4 finalAffine = new Matrix4x4();
    
    private ComputeBuffer attractorsBuffer;

    public int GetTransformCount() {
        return affineTransforms.Count;
    }

    public List<Matrix4x4> GetAffineTransforms() {
        return affineTransforms;
    }

    public ComputeBuffer GetAffineBuffer() {
        return attractorsBuffer;
    }

    public Matrix4x4 GetFinalTransform() {
        return finalAffine;
    }

    Matrix4x4 Scale(Vector3 s) {
        Matrix4x4 scaleMatrix = Matrix4x4.identity;

        scaleMatrix.SetRow(0, new Vector4(s.x, 0, 0, 0));
        scaleMatrix.SetRow(1, new Vector4(0, s.y, 0, 0));
        scaleMatrix.SetRow(2, new Vector4(0, 0, s.z, 0));

        return scaleMatrix;
    }

    Matrix4x4 ShearX(Vector3 s) {
        Matrix4x4 shearMatrix = Matrix4x4.identity;

        shearMatrix.SetRow(0, new Vector4(1, s.y, s.z, 0));
        shearMatrix.SetRow(1, new Vector4(0, 1, 0, 0));
        shearMatrix.SetRow(2, new Vector4(0, 0, 1, 0));

        return shearMatrix;
    }

    Matrix4x4 ShearY(Vector3 s) {
        Matrix4x4 shearMatrix = Matrix4x4.identity;

        shearMatrix.SetRow(0, new Vector4(1, 0, 0, 0));
        shearMatrix.SetRow(1, new Vector4(s.x, 1, s.z, 0));
        shearMatrix.SetRow(2, new Vector4(0, 0, 1, 0));

        return shearMatrix;
    }

    Matrix4x4 ShearZ(Vector3 s) {
        Matrix4x4 shearMatrix = Matrix4x4.identity;

        shearMatrix.SetRow(0, new Vector4(1, 0, 0, 0));
        shearMatrix.SetRow(1, new Vector4(0, 1, 0, 0));
        shearMatrix.SetRow(2, new Vector4(s.x, s.y, 1, 0));

        return shearMatrix;
    }

    Matrix4x4 Translate(Vector3 t) {
        Matrix4x4 transformMatrix = Matrix4x4.identity;

        transformMatrix.SetRow(0, new Vector4(1, 0, 0, t.x));
        transformMatrix.SetRow(1, new Vector4(0, 1, 0, t.y));
        transformMatrix.SetRow(2, new Vector4(0, 0, 1, t.z));

        return transformMatrix;
    }

    Matrix4x4 Rotation(Vector3 r) {
        float xRad = r.x * Mathf.Deg2Rad;
        float yRad = r.y * Mathf.Deg2Rad;
        float zRad = r.z * Mathf.Deg2Rad;

        Matrix4x4 rotateX = Matrix4x4.identity;

        rotateX.SetRow(0, new Vector4(1, 0, 0, 0));
        rotateX.SetRow(1, new Vector4(0, Mathf.Cos(xRad), -Mathf.Sin(xRad), 0));
        rotateX.SetRow(2, new Vector4(0, Mathf.Sin(xRad), Mathf.Cos(xRad), 0));

        Matrix4x4 rotateY = Matrix4x4.identity;

        rotateY.SetRow(0, new Vector4(Mathf.Cos(yRad), 0, Mathf.Sin(yRad), 0));
        rotateY.SetRow(1, new Vector4(0, 1, 0, 0));
        rotateY.SetRow(2, new Vector4(-Mathf.Sin(yRad), 0, Mathf.Cos(yRad), 0));
        
        Matrix4x4 rotateZ = Matrix4x4.identity;

        rotateZ.SetRow(0, new Vector4(Mathf.Cos(zRad), -Mathf.Sin(zRad), 0, 0));
        rotateZ.SetRow(1, new Vector4(Mathf.Sin(zRad), Mathf.Cos(zRad), 0, 0));
        rotateZ.SetRow(2, new Vector4(0, 0, 1, 0));

        Matrix4x4 rotationMatrix = rotateY * rotateX * rotateZ;

        return rotationMatrix;
    }

    Matrix4x4 AffineFromInstructions(TransformSet.TransformInstructions instructions) {
        Matrix4x4 affine = Matrix4x4.identity;

        Matrix4x4 scale = Scale(instructions.scale);
        Matrix4x4 shearX = ShearX(instructions.shearX);
        Matrix4x4 shearY = ShearY(instructions.shearY);
        Matrix4x4 shearZ = ShearZ(instructions.shearZ);
        Matrix4x4 shear = shearZ * shearY * shearX;
        Matrix4x4 translate = Translate(instructions.translate);
        Matrix4x4 rotation = Rotation(instructions.rotate);

        affine = scale * rotation * shear * translate;

        return affine;
    }

    public Matrix4x4 InterpolateAffineTransform(int i1, int i2, float t) {
        Matrix4x4 interpolatedMatrix = Matrix4x4.identity;

        Matrix4x4 m1 = affineTransforms[i1];
        Matrix4x4 m2 = affineTransforms[i2];

        interpolatedMatrix.SetRow(0, Vector4.Lerp(m1.GetRow(0), m2. GetRow(0), t));
        interpolatedMatrix.SetRow(1, Vector4.Lerp(m1.GetRow(1), m2. GetRow(1), t));
        interpolatedMatrix.SetRow(2, Vector4.Lerp(m1.GetRow(2), m2. GetRow(2), t));

        return interpolatedMatrix;
    }


    void PopulateAffineBuffer() {
        affineTransforms.Clear();

        List<TransformSet.TransformInstructions> instructionSet = setBlender.GetBlendedSet();

        for (int i = 0; i < instructionSet.Count; ++i) {
            affineTransforms.Add(AffineFromInstructions(instructionSet[i]));
        }

        finalAffine = AffineFromInstructions(setBlender.GetFinalTransform());
        
        attractorsBuffer.SetData(affineTransforms.ToArray());
    }

    void OnEnable() {
        attractorsBuffer = new ComputeBuffer(32, System.Runtime.InteropServices.Marshal.SizeOf(typeof(Matrix4x4)));

        PopulateAffineBuffer();
    }

    void Update() {
        PopulateAffineBuffer();
    }

    void OnDisable() {
        attractorsBuffer.Release();
        attractorsBuffer = null;
    }
}
