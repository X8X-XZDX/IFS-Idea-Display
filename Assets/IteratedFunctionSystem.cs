using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IteratedFunctionSystem : MonoBehaviour {
    public AffineTransformations affineTransformations;

    public Shader pointShader, cubeShader;

    public ComputeShader particleUpdater;

    public uint particlesPerBatch = 200000;
    public uint batchCount = 1;

    public bool uncapped = false;

    private Mesh pointMesh, quadMesh, cubeMesh;
    private Material pointMaterial, cubeMaterial;

    public GraphicsBuffer positionBuffer;
    private GraphicsBuffer pointCommandBuffer;
    private GraphicsBuffer.IndirectDrawIndexedArgs[] pointCommandData;

    private RenderParams pointRenderParams, meshRenderParams;

    void InitializeCommandBuffers() {
        pointCommandBuffer = new GraphicsBuffer(GraphicsBuffer.Target.IndirectArguments, (int)1, GraphicsBuffer.IndirectDrawIndexedArgs.size);
        pointCommandData = new GraphicsBuffer.IndirectDrawIndexedArgs[1];

        for (int i = 0; i < 1; ++i) {
            pointCommandData[i].instanceCount = particlesPerBatch * batchCount;
            pointCommandData[i].indexCountPerInstance = 1;
        }

        pointCommandBuffer.SetData(pointCommandData);
    }

    void InitializeRenderParams() {
        pointRenderParams = new RenderParams(pointMaterial);
        pointRenderParams.worldBounds = new Bounds(Vector3.zero, 10000 * Vector3.one);
        pointRenderParams.matProps = new MaterialPropertyBlock();
        
        pointRenderParams.matProps.SetBuffer("_Positions", positionBuffer);
    }

    void InitializeParticleBuffer() {
        int particleCount = (int)particlesPerBatch * (int)batchCount;
        positionBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured, particleCount, System.Runtime.InteropServices.Marshal.SizeOf(typeof(Vector4)));

        int cubeRootParticleCount = Mathf.CeilToInt(Mathf.Pow(particleCount, 1.0f / 3.0f));

        particleUpdater.SetInt("_CubeResolution", cubeRootParticleCount);
        particleUpdater.SetFloat("_CubeSize", 1.0f / cubeRootParticleCount);
        particleUpdater.SetBuffer(0, "_PositionBuffer", positionBuffer);
        particleUpdater.SetInt("_ParticleCount", (int)particlesPerBatch);

        for (int batch = 0; batch < (int)batchCount; ++batch) {
            particleUpdater.SetInt("_BatchIndex", batch);
            particleUpdater.Dispatch(0, Mathf.CeilToInt(particlesPerBatch / 8.0f), 1, 1);
        }
    }


    void OnEnable() {
        pointMesh = new Mesh();

        Vector3[] vertices = new Vector3[1];
        vertices[0] = new Vector3(0, 0, 0);

        pointMesh.vertices = vertices;

        int[] indices = new int[1];
        indices[0] = 0;

        pointMesh.SetIndices(indices, MeshTopology.Points, 0, false, 0);

        pointMaterial = new Material(pointShader);

        InitializeParticleBuffer();
        InitializeCommandBuffers();
        InitializeRenderParams();
    }
    
    public virtual void IterateSystem() {
        int particleCount = (int)particlesPerBatch * (int)batchCount;

        particleUpdater.SetInt("_TransformationCount", affineTransformations.GetTransformCount());

        particleUpdater.SetInt("_Seed", Mathf.CeilToInt(Random.Range(1, 1000000)));
        particleUpdater.SetBuffer(2, "_PositionBuffer", positionBuffer);
        particleUpdater.SetBuffer(2, "_Transformations", affineTransformations.GetAffineBuffer());
        particleUpdater.Dispatch(2, Mathf.CeilToInt(particleCount / 8.0f), 1, 1);
    }

    void InstanceParticles() {
        // pointRenderParams.matProps.SetMatrix("_FinalTransform", affineTransformations.GetFinalTransform());

        Graphics.RenderMeshIndirect(pointRenderParams, pointMesh, pointCommandBuffer, (int)1);
    }

    void Update() {
        if (uncapped) {
            IterateSystem();
        } else {
            if (Input.GetKeyDown("space")) {
                IterateSystem();
            }
        }

        InstanceParticles();
    }

    void OnDisable() {
        pointCommandBuffer.Release();
        positionBuffer.Release();
    }
}
