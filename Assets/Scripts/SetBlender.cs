using System.Collections;
using TransformInstructions = TransformSet.TransformInstructions;
using System.Collections.Generic;
using UnityEngine;

public class SetBlender : MonoBehaviour {

    public TransformSet set1, set2;

    public AnimationCurve animationCurve;

    public bool useAnimationCurve = false;

    [Range(0.0f, 1.0f)]
    public float t = 0.0f;

    public bool animate = false;

    [Range(0.01f, 5.0f)]
    public float speed = 1.0f;

    [Range(0.001f, 1.0f)]
    public float epsilon = 0.01f;
    
    public TransformInstructions finalTransform = new TransformInstructions();

    private List<TransformInstructions> blendedSet = new List<TransformInstructions>();

    private float bounce = 1;
    private bool swap = false;

    public List<TransformInstructions> GetBlendedSet() {
        return blendedSet;
    }

    public TransformInstructions GetFinalTransform() {
        return finalTransform;
    }

    TransformInstructions InterpolateInstructions(TransformInstructions t1, TransformInstructions t2, float t) {
        TransformInstructions interpolatedInstructions = new TransformInstructions();

        interpolatedInstructions.scale = Vector3.LerpUnclamped(t1.scale, t2.scale, t);
        interpolatedInstructions.shearX = Vector3.LerpUnclamped(t1.shearX, t2.shearX, t);
        interpolatedInstructions.shearY = Vector3.LerpUnclamped(t1.shearY, t2.shearY, t);
        interpolatedInstructions.shearZ = Vector3.LerpUnclamped(t1.shearZ, t2.shearZ, t);
        interpolatedInstructions.translate = Vector3.LerpUnclamped(t1.translate, t2.translate, t);

        Quaternion r1 = Quaternion.Euler(t1.rotate);
        Quaternion r2 = Quaternion.Euler(t2.rotate);
        Quaternion r3 = Quaternion.SlerpUnclamped(r1, r2, t);

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
            float blendFactor = animationCurve.Evaluate(t); // TO DO: Easing Functions

            if (swap)
                blendedSet.Add(InterpolateInstructions(instructionSet2[i], instructionSet1[i], blendFactor));
            else
                blendedSet.Add(InterpolateInstructions(instructionSet1[i], instructionSet2[i], blendFactor));
        }
    }

    private List<TransformInstructions> moveTowardSet = new List<TransformInstructions>();

    Vector3 ExpDecay(Vector3 a, Vector3 b, float decay) {
        Vector3 v = new Vector3(0, 0, 0);
        v.x = b.x + (a.x - b.x) * Mathf.Exp(-decay * Time.deltaTime);
        v.y = b.y + (a.y - b.y) * Mathf.Exp(-decay * Time.deltaTime);
        v.z = b.z + (a.z - b.z) * Mathf.Exp(-decay * Time.deltaTime);

        return v;
    }

    TransformInstructions MoveTowardInstructions(TransformInstructions t1, TransformInstructions t2, float decay) {
        TransformInstructions i = new TransformInstructions();

        i.scale = ExpDecay(t1.scale, t2.scale, decay);
        i.shearX = ExpDecay(t1.shearX, t2.shearX, decay);
        i.shearY = ExpDecay(t1.shearY, t2.shearY, decay);
        i.shearZ = ExpDecay(t1.shearZ, t2.shearZ, decay);
        i.translate = ExpDecay(t1.translate, t2.translate, decay);

        Quaternion r1 = Quaternion.Euler(t1.rotate);
        Quaternion r2 = Quaternion.Euler(t2.rotate);
        Quaternion r3 = Quaternion.Slerp(r1, r2, Mathf.Min(decay * Time.deltaTime, 1.0f));

        i.rotate = r3.eulerAngles;

        return i;
    }

    private void MoveTowardSet() {
        blendedSet.Clear();

        for (int i = 0; i < moveTowardSet.Count; ++i) {
            moveTowardSet[i] = MoveTowardInstructions(moveTowardSet[i], set2.transformSet[i], speed);
            blendedSet.Add(moveTowardSet[i]);
        }
    }

    private void OnEnable() {
        t = 0;

        moveTowardSet = new List<TransformInstructions>(set1.transformSet);
        MoveTowardSet();
        // BlendSets();
    }

    private bool paused = false;
    private void Update() {

        if (useAnimationCurve) {
            if (Input.GetKeyDown("space")) paused = !paused;
            if (animate && !paused) {
                t += Time.deltaTime * bounce * speed;

                t = Mathf.Clamp(t, 0.0f, 1.0f);
                if (t >= 1) {
                    swap = !swap;
                    t = 0;

                    if (swap) set1.ApplyPreset();
                    else set2.ApplyPreset();
                }
            }

            BlendSets();
        } else {
            if (Input.GetKeyDown("space")) set2.ApplyPreset();
            MoveTowardSet();
        }
    }
}
