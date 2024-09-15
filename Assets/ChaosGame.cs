using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaosGame : MonoBehaviour {
    public bool usePoints = false;
    public Mesh particleMesh;
    public Shader particleShader;
    public ComputeShader particleUpdater;

    public bool manual = false;

    [Range(1, 20)]
    public int iterationCount = 1;

    public bool uncapped = false;

    private Material particleMaterial;

    private GraphicsBuffer commandBuffer, positionBuffer;
    private GraphicsBuffer.IndirectDrawArgs[] commandData;
    private GraphicsBuffer.IndirectDrawIndexedArgs[] commandIndexedData;

    private RenderParams renderParams;

    private uint particleCount = 200000;

    private AffineTransformations affineTransformations;

    void OnEnable() {
        affineTransformations = GetComponent<AffineTransformations>();

        particleMaterial = new Material(particleShader);

        positionBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured, (int)particleCount, System.Runtime.InteropServices.Marshal.SizeOf(typeof(Vector4)));
        
        if (usePoints) {
            commandBuffer = new GraphicsBuffer(GraphicsBuffer.Target.IndirectArguments, 1, GraphicsBuffer.IndirectDrawArgs.size);
            commandData = new GraphicsBuffer.IndirectDrawArgs[1];
            commandData[0].instanceCount = particleCount;
            commandData[0].vertexCountPerInstance = 1;

            commandBuffer.SetData(commandData);
        } else {
            commandBuffer = new GraphicsBuffer(GraphicsBuffer.Target.IndirectArguments, 1, GraphicsBuffer.IndirectDrawIndexedArgs.size);
            commandIndexedData = new GraphicsBuffer.IndirectDrawIndexedArgs[1];
            commandIndexedData[0].instanceCount = particleCount;
            commandIndexedData[0].indexCountPerInstance = particleMesh.GetIndexCount(0);

            commandBuffer.SetData(commandIndexedData);
        }


        renderParams = new RenderParams(particleMaterial);
        renderParams.worldBounds = new Bounds(Vector3.zero, 10000 * Vector3.one);
        renderParams.matProps = new MaterialPropertyBlock();

        renderParams.matProps.SetBuffer("_Positions", positionBuffer);

        int cubeRootParticleCount = Mathf.CeilToInt(Mathf.Pow(particleCount, 1.0f / 3.0f));
        particleUpdater.SetInt("_CubeResolution", cubeRootParticleCount);
        particleUpdater.SetFloat("_CubeSize", 1.0f / cubeRootParticleCount);

        particleUpdater.SetBuffer(0, "_PositionBuffer", positionBuffer);
        particleUpdater.Dispatch(0, Mathf.CeilToInt(particleCount / 8.0f), 1, 1);
    }


    void IterateSystem() {
        for (int i = 0; i < iterationCount; ++i) {
            particleUpdater.SetInt("_Seed", Mathf.CeilToInt(Random.Range(1, 1000000)));
            particleUpdater.SetBuffer(2, "_PositionBuffer", positionBuffer);
            particleUpdater.SetBuffer(2, "_Transformations", affineTransformations.GetAffineBuffer());
            particleUpdater.Dispatch(2, Mathf.CeilToInt(particleCount / 8.0f), 1, 1);
        }
    }

    void Update() {
        particleUpdater.SetInt("_TransformationCount", affineTransformations.GetTransformCount());

        if (uncapped) {
            IterateSystem();
        } else {
            if (Input.GetKeyDown("space")) {
                IterateSystem();
            }
        }
        
        renderParams.matProps.SetMatrix("_FinalTransform", affineTransformations.GetFinalTransform());

        if (usePoints)
            Graphics.RenderPrimitivesIndirect(renderParams, MeshTopology.Points, commandBuffer, 1);
        else
            Graphics.RenderMeshIndirect(renderParams, particleMesh, commandBuffer, 1);
    }

    void OnDisable() {
        commandBuffer.Release();
        positionBuffer.Release();

        commandBuffer = null;
        positionBuffer = null;
        commandData = null;
        particleMaterial = null;
    }
}
