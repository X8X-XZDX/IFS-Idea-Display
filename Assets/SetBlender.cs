using System.Collections;
using TransformInstructions = TransformSet.TransformInstructions;
using System.Collections.Generic;
using UnityEngine;

public class SetBlender : MonoBehaviour {

    public TransformSet set1, set2;

    [Range(0.0f, 1.0f)]
    public float t = 0.0f;
    
    public TransformInstructions finalTransform = new TransformInstructions();

    private List<TransformInstructions> blendedSet = new List<TransformInstructions>();

    public List<TransformInstructions> GetBlendedSet() {
        return blendedSet;
    }

    public TransformInstructions GetFinalTransform() {
        return finalTransform;
    }

    TransformInstructions InterpolateInstructions(TransformInstructions t1, TransformInstructions t2, float t) {
        TransformInstructions interpolatedInstructions = new TransformInstructions();

        interpolatedInstructions.scale = Vector3.Lerp(t1.scale, t2.scale, t);
        interpolatedInstructions.shearX = Vector3.Lerp(t1.shearX, t2.shearX, t);
        interpolatedInstructions.shearY = Vector3.Lerp(t1.shearY, t2.shearY, t);
        interpolatedInstructions.shearZ = Vector3.Lerp(t1.shearZ, t2.shearZ, t);
        interpolatedInstructions.translate = Vector3.Lerp(t1.translate, t2.translate, t);

        Quaternion r1 = Quaternion.Euler(t1.rotate);
        Quaternion r2 = Quaternion.Euler(t2.rotate);
        Quaternion r3 = Quaternion.Slerp(r1, r2, t);

        interpolatedInstructions.rotate = r3.eulerAngles;

        return interpolatedInstructions;
    }

    private void BlendSets() {
        blendedSet.Clear();

        // Copy instruction sets so the following modifications don't ruin the originals
        List<TransformInstructions> instructionSet1 = new List<TransformInstructions>(set1.transformSet);
        List<TransformInstructions> instructionSet2 = new List<TransformInstructions>(set2.transformSet);

        // Apply post transform to all instruction sets
        for (int i = 0; i < instructionSet1.Count; ++i) {
            instructionSet1[i] += set1.postTransform;
        }

        for (int i = 0; i < instructionSet2.Count; ++i) {
            instructionSet2[i] += set2.postTransform;
        }

        // In order to blend smaller instruction sets with larger instruction sets, append identity matrices to smaller set
        int sizeDifference = Mathf.Abs(instructionSet1.Count - instructionSet2.Count);
        if (instructionSet1.Count < instructionSet2.Count) {
            for (int i = 0; i < sizeDifference; ++i) {
                instructionSet1.Add(TransformSet.GetIdentity());
            }
        } else if (instructionSet2.Count < instructionSet1.Count) {
            for (int i = 0; i < sizeDifference; ++i) {
                instructionSet2.Add(TransformSet.GetIdentity());
            }
        }

        // Blend instruction sets and create list of affine transformations
        for (int i = 0; i < instructionSet1.Count; ++i) {
            blendedSet.Add(InterpolateInstructions(instructionSet1[i], instructionSet2[i], t));
        }
    }

    private void OnEnable() {
        BlendSets();
    }

    private void Update() {
        BlendSets();
    }
}
