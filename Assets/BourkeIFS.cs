using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BourkeIFS : MonoBehaviour {
    public Shader particleShader;
    public ComputeShader particleUpdater;

    public enum Attractor {
        Custom = 1,
        SierpinskiTriangle2D,
        Vicsek2D,
        SierpinskiCarpet2D,
        SierpinskiTriangle3D,
        Vicsek3D,
        SierpinskiCarpet3D
    } public Attractor attractor = Attractor.SierpinskiTriangle2D;
    private Attractor cachedAttractor;

    [Range(0.0f, 1.0f)]
    public float size = 1.0f;

    [Range(0.0f, 20.0f)]
    public float speed = 1.0f;

    public bool manual = false;
    public bool pauseAtTop = false;

    [Range(1, 100)]
    public int maxGenerations = 10;
    [Range(1, 20)]
    public int iterationCount = 1;

    private Material particleMaterial;

    private GraphicsBuffer commandBuffer, originBuffer, destinationBuffer;
    private GraphicsBuffer.IndirectDrawArgs[] commandData;


    private RenderParams renderParams;

    private ComputeBuffer attractorsBuffer;

    private float t = 0.0f;
    private int currentGen = 1;

    private int maxParticleCount, currentParticleCount;

    void OnEnable() {
        particleMaterial = new Material(particleShader);
        
        Matrix4x4[] customAttractorPositions = new Matrix4x4[4];
        customAttractorPositions[0].SetRow(0, new Vector4(0.14f, 0.01f, 0.0f, -0.08f));
        customAttractorPositions[0].SetRow(1, new Vector4(0.0f, 0.51f, 0.0f, -1.31f));
        customAttractorPositions[0].SetRow(2, new Vector4(0.0f, 0.0f, 1.0f, 0.0f));
        customAttractorPositions[0].SetRow(3, new Vector4(0.0f, 0.0f, 0.0f, 1.0f));

        customAttractorPositions[1].SetRow(0, new Vector4(0.43f, 0.52f, 0.0f, 1.49f));
        customAttractorPositions[1].SetRow(1, new Vector4(-0.45f, 0.5f, 0.0f, -0.75f));
        customAttractorPositions[1].SetRow(2, new Vector4(0.0f, 0.0f, 1.0f, 0.0f));
        customAttractorPositions[1].SetRow(3, new Vector4(0.0f, 0.0f, 0.0f, 1.0f));

        customAttractorPositions[2].SetRow(0, new Vector4(0.45f, -0.49f, 0.0f, -1.62f));
        customAttractorPositions[2].SetRow(1, new Vector4(0.47f, 0.47f, 0.0f, -0.74f));
        customAttractorPositions[2].SetRow(2, new Vector4(0.0f, 0.0f, 1.0f, 0.0f));
        customAttractorPositions[2].SetRow(3, new Vector4(0.0f, 0.0f, 0.0f, 1.0f));

        customAttractorPositions[3].SetRow(0, new Vector4(0.49f, 0.0f, 0.0f, 0.02f));
        customAttractorPositions[3].SetRow(1, new Vector4(0.0f, 0.51f, 0.0f, 1.62f));
        customAttractorPositions[3].SetRow(2, new Vector4(0.0f, 0.0f, 1.0f, 0.0f));
        customAttractorPositions[3].SetRow(3, new Vector4(0.0f, 0.0f, 0.0f, 1.0f));

        
        attractorsBuffer = new ComputeBuffer(4, System.Runtime.InteropServices.Marshal.SizeOf(typeof(Matrix4x4)));
        attractorsBuffer.SetData(customAttractorPositions);

        int particleSum = 0;

        for (int gen = 0; gen < maxGenerations; ++gen) {
            int partitionSize = (int)Mathf.Pow(customAttractorPositions.Length, gen);
            Debug.Log("Generation: " + gen.ToString() + " Space Needed: " + partitionSize.ToString() + " Space Used: " + particleSum.ToString());
            particleSum += (int)Mathf.Pow(customAttractorPositions.Length, gen);

        }

        Debug.Log("Space needed for " + maxGenerations.ToString() + " generations: " + particleSum.ToString());

        maxParticleCount = particleSum;

        originBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured | GraphicsBuffer.Target.CopyDestination, (int)maxParticleCount, System.Runtime.InteropServices.Marshal.SizeOf(typeof(Vector4)));
        destinationBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured | GraphicsBuffer.Target.CopySource, (int)maxParticleCount, System.Runtime.InteropServices.Marshal.SizeOf(typeof(Vector4)));

        commandBuffer = new GraphicsBuffer(GraphicsBuffer.Target.IndirectArguments, 1, GraphicsBuffer.IndirectDrawArgs.size);
        commandData = new GraphicsBuffer.IndirectDrawArgs[1];

        renderParams = new RenderParams(particleMaterial);
        renderParams.worldBounds = new Bounds(Vector3.zero, 10000 * Vector3.one);
        renderParams.matProps = new MaterialPropertyBlock();
        commandData[0].instanceCount = (uint)maxParticleCount;
        commandData[0].vertexCountPerInstance = 1;

        renderParams.matProps.SetBuffer("_Origins", originBuffer);
        renderParams.matProps.SetBuffer("_Destinations", destinationBuffer);

        commandBuffer.SetData(commandData);

        particleUpdater.SetBuffer(0, "_PositionBuffer", originBuffer);
        particleUpdater.Dispatch(0, Mathf.CeilToInt(maxParticleCount / 8.0f), 1, 1);
        particleUpdater.SetBuffer(0, "_PositionBuffer", destinationBuffer);
        particleUpdater.Dispatch(0, Mathf.CeilToInt(maxParticleCount / 8.0f), 1, 1);

        currentParticleCount = 1;
        currentGen = 1;

        // Vector3[] data = new Vector3[attractorTransforms.Count];
        // attractorsBuffer.GetData(data);

        // for (int i = 0; i < attractorTransforms.Count; ++i) {
        //     Debug.Log(data[i]);
        // }

        t = 0;
    }

    void UpdateAttractor() {
        switch(attractor) {
            case Attractor.Custom:
            break;
        }

        cachedAttractor = attractor;
    }

    void IterateSystem() {
        // Graphics.CopyBuffer(destinationBuffer, originBuffer);

        // for (int i = 0; i < iterationCount; ++i) {
        //     particleUpdater.SetFloat("_Size", size);
        //     particleUpdater.SetInt("_PointCount", 4);
        //     particleUpdater.SetBuffer(2, "_PositionBuffer", destinationBuffer);
        //     particleUpdater.SetBuffer(2, "_BourkeAttractors", attractorsBuffer);
        //     particleUpdater.Dispatch(2, Mathf.CeilToInt(particleCount / 8.0f), 1, 1);

        //     currentGen += 1;
        // }

        if (currentGen >= maxGenerations) return;

        int previousParticleCount = currentParticleCount;

        currentParticleCount += (int)Mathf.Pow(4, currentGen);

        particleUpdater.SetInt("_PointCount", 4);
        particleUpdater.SetInt("_Offset", (int)previousParticleCount);
        particleUpdater.SetInt("_Generation", (int)currentGen);
        particleUpdater.SetInt("_ParticleCount", (int)currentParticleCount);
        particleUpdater.SetBuffer(3, "_PositionBuffer", originBuffer);
        particleUpdater.SetBuffer(3, "_BourkeAttractors", attractorsBuffer);

        int threadCount = Mathf.CeilToInt(currentParticleCount / 8.0f);
        particleUpdater.Dispatch(3, threadCount, 1, 1);

        // Debug.Log("Generation: " + currentGen.ToString());
        // Debug.Log("Memory Offset: " + previousParticleCount.ToString());
        // Debug.Log("Memory Use: " + currentParticleCount.ToString());
        // Vector4[] data = new Vector4[maxParticleCount];
        // originBuffer.GetData(data);
        // for (int i = 0; i < currentParticleCount; ++i) {
        //     Debug.Log(data[i]);
        // }

        t = 0;
        
        currentGen += 1;
    }

    void Update() {
        particleUpdater.SetFloat("_Time", Time.time);
        particleUpdater.SetFloat("_DeltaTime", Time.deltaTime);

        if (attractor != cachedAttractor) {
            // currentGen = 0;
            UpdateAttractor();
        }

        if (t <= 1.0f)
            t += Time.deltaTime * speed;

        if (manual) {
            if (Input.GetKeyDown("space")) {
                IterateSystem();
            }
        } else if (t >= 1.0f) {
            IterateSystem();
        }

        renderParams.matProps.SetFloat("_Interpolator", size);
        
        commandData[0].instanceCount = (uint)currentParticleCount;

        commandBuffer.SetData(commandData);
        Graphics.RenderPrimitivesIndirect(renderParams, MeshTopology.Points, commandBuffer, 1);
    }

    void OnDisable() {
        commandBuffer.Release();
        attractorsBuffer.Release();
        originBuffer.Release();
        destinationBuffer.Release();

        commandBuffer = null;
        attractorsBuffer = null;
        originBuffer = null;
        destinationBuffer = null;
        commandData = null;
        particleMaterial = null;
    }
}
