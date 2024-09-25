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

    public GraphicsBuffer[] positionBuffers;
    private GraphicsBuffer pointCommandBuffer;
    private GraphicsBuffer.IndirectDrawIndexedArgs[] pointCommandData;

    private RenderParams[] pointRenderParams;

    void InitializeCommandBuffers() {
        pointCommandBuffer = new GraphicsBuffer(GraphicsBuffer.Target.IndirectArguments, 1, GraphicsBuffer.IndirectDrawIndexedArgs.size);
        pointCommandData = new GraphicsBuffer.IndirectDrawIndexedArgs[1];

        pointCommandData[0].instanceCount = particlesPerBatch;
        pointCommandData[0].indexCountPerInstance = 1;

        pointCommandBuffer.SetData(pointCommandData);
    }

    void InitializeRenderParams() {
        pointRenderParams = new RenderParams[batchCount];

        for (int i = 0; i < batchCount; ++i) {
            pointRenderParams[i] = new RenderParams(pointMaterial);
            pointRenderParams[i].worldBounds = new Bounds(Vector3.zero, 10000 * Vector3.one);
            pointRenderParams[i].matProps = new MaterialPropertyBlock();
            
            pointRenderParams[i].matProps.SetBuffer("_Positions", positionBuffers[i]);
            pointRenderParams[i].matProps.SetInt("_BatchIndex", i);
        }
    }

    void InitializeParticleBuffer() {
        positionBuffers = new GraphicsBuffer[batchCount];
        for (int i = 0; i < batchCount; ++i)
            positionBuffers[i] = new GraphicsBuffer(GraphicsBuffer.Target.Structured, (int)particlesPerBatch, System.Runtime.InteropServices.Marshal.SizeOf(typeof(Vector4)));

        int cubeRootParticleCount = Mathf.CeilToInt(Mathf.Pow(particlesPerBatch, 1.0f / 3.0f));

        particleUpdater.SetInt("_CubeResolution", cubeRootParticleCount);
        particleUpdater.SetFloat("_CubeSize", 1.0f / cubeRootParticleCount);
        particleUpdater.SetInt("_ParticleCount", (int)particlesPerBatch);

        for (int i = 0; i < (int)batchCount; ++i) {
            particleUpdater.SetBuffer(0, "_PositionBuffer", positionBuffers[i]);
            particleUpdater.Dispatch(0, Mathf.CeilToInt(particlesPerBatch / 8.0f), 1, 1);
        }
    }


    void OnEnable() {
        // Debug.Log(SystemInfo.graphicsDeviceName);

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
        for (int i = 0; i < batchCount; ++i) {
            particleUpdater.SetInt("_TransformationCount", affineTransformations.GetTransformCount());
            particleUpdater.SetInt("_Seed", Mathf.CeilToInt(Random.Range(1, 1000000)));
            particleUpdater.SetBuffer(2, "_PositionBuffer", positionBuffers[i]);
            particleUpdater.SetBuffer(2, "_Transformations", affineTransformations.GetAffineBuffer());
            particleUpdater.Dispatch(2, Mathf.CeilToInt(particlesPerBatch / 8.0f), 1, 1);
        }
    }

    void InstanceParticles() {
        for (int i = 0; i < batchCount; ++i)
            Graphics.RenderMeshIndirect(pointRenderParams[i], pointMesh, pointCommandBuffer, 1);
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
        for (int i = 0; i < batchCount; ++i)
            positionBuffers[i].Release();
    }
}
