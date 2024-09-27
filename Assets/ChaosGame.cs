using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaosGame : IteratedFunctionSystem {
    public override void IterateSystem() {
        for (int i = 0; i < (int)batchCount; ++i) {
            particleUpdater.SetInt("_TransformationCount", affineTransformations.GetTransformCount());
            particleUpdater.SetInt("_ParticleCount", (int)particlesPerBatch);
            particleUpdater.SetInt("_Seed", Mathf.CeilToInt(Random.Range(1, 1000000)));
            particleUpdater.SetBuffer(2, "_VertexBuffer", pointCloudMeshes[i].GetVertexBuffer(0));
            particleUpdater.SetBuffer(2, "_Transformations", affineTransformations.GetAffineBuffer());
            particleUpdater.Dispatch(2, Mathf.CeilToInt(particlesPerBatch / threadsPerGroup), 1, 1);
        }
    }
}
