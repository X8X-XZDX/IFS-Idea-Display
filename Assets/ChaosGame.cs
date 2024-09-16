using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaosGame : IteratedFunctionSystem {
    public override void IterateSystem() {
        particleUpdater.SetInt("_TransformationCount", affineTransformations.GetTransformCount());
        particleUpdater.SetInt("_ParticleCount", (int)particlesPerBatch);
        particleUpdater.SetInt("_Seed", Mathf.CeilToInt(Random.Range(1, 1000000)));
        particleUpdater.SetBuffer(2, "_PositionBuffer", positionBuffer);
        particleUpdater.SetBuffer(2, "_Transformations", affineTransformations.GetAffineBuffer());

        for (int batch = 0; batch < (int)batchCount; ++batch) {
            particleUpdater.SetInt("_BatchIndex", batch);
            particleUpdater.Dispatch(2, Mathf.CeilToInt(particlesPerBatch / 8.0f), 1, 1);
        }
    }
}
