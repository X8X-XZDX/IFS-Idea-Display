using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaosGame : IteratedFunctionSystem {
    public override void IterateSystem() {
        particleUpdater.SetInt("_Seed", Mathf.CeilToInt(Random.Range(1, 1000000)));
        particleUpdater.SetBuffer(2, "_PositionBuffer", positionBuffer);
        particleUpdater.SetBuffer(2, "_Transformations", affineTransformations.GetAffineBuffer());
        particleUpdater.Dispatch(2, Mathf.CeilToInt(particleCount / 8.0f), 1, 1);
    }
}
