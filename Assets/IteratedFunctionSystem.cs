using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using Unity.Collections.LowLevel.Unsafe;

public class IteratedFunctionSystem : MonoBehaviour {
    public AffineTransformations affineTransformations;

    public Shader pointShader;

    public ComputeShader particleUpdater;

    public uint particlesPerBatch = 200000;
    public uint batchCount = 1;

    public bool uncapped = false;

    public Mesh[] pointCloudMeshes;
    private Material pointMaterial;

    private RenderParams pointRenderParams;

    [NonSerialized]
    public int threadsPerGroup = 64;

    void InitializeRenderParams() {
        pointRenderParams = new RenderParams(pointMaterial);
        pointRenderParams.worldBounds = new Bounds(Vector3.zero, 10000 * Vector3.one);
        // pointRenderParams.matProps = new MaterialPropertyBlock();
    }

    void InitializeMeshes() {
        pointCloudMeshes = new Mesh[batchCount];

        for (int i = 0; i < batchCount; ++i) {
            // Create point cloud mesh
            pointCloudMeshes[i] = new Mesh();

            pointCloudMeshes[i].vertexBufferTarget |= GraphicsBuffer.Target.Raw;
            pointCloudMeshes[i].indexBufferTarget |= GraphicsBuffer.Target.Raw;

            var vp = new VertexAttributeDescriptor(UnityEngine.Rendering.VertexAttribute.Position, VertexAttributeFormat.Float32, 3);

            pointCloudMeshes[i].SetVertexBufferParams((int)particlesPerBatch, vp);
            pointCloudMeshes[i].SetIndexBufferParams((int)particlesPerBatch, IndexFormat.UInt32);

            pointCloudMeshes[i].SetSubMesh(0, new SubMeshDescriptor(0, (int)particlesPerBatch, MeshTopology.Points), MeshUpdateFlags.DontRecalculateBounds);

            // Initialize point cloud vertices
            int cubeRootParticleCount = Mathf.CeilToInt(Mathf.Pow(particlesPerBatch, 1.0f / 3.0f));
            particleUpdater.SetInt("_CubeResolution", cubeRootParticleCount);
            particleUpdater.SetFloat("_CubeSize", 1.0f / cubeRootParticleCount);
            particleUpdater.SetInt("_ParticleCount", (int)particlesPerBatch);

            particleUpdater.SetBuffer(0, "_VertexBuffer", pointCloudMeshes[i].GetVertexBuffer(0));
            particleUpdater.SetBuffer(0, "_IndexBuffer", pointCloudMeshes[i].GetIndexBuffer());
            particleUpdater.Dispatch(0, Mathf.CeilToInt(particlesPerBatch / threadsPerGroup), 1, 1);
        }
    }

    public Shader voxelShader;
    public Mesh voxelMesh;
    GraphicsBuffer voxelGrid, commandBuffer;
    GraphicsBuffer.IndirectDrawIndexedArgs[] commandIndexedData;
    public int voxelBounds;
    public float voxelSize;
    public bool renderVoxels = false;
    private int voxelDimension, voxelCount;
    private Material voxelMaterial;
    private RenderParams renderParams;

    void InitializeVoxelGrid() {
        voxelMaterial = new Material(voxelShader);

        voxelDimension = Mathf.FloorToInt(voxelBounds / voxelSize);
        voxelCount = voxelDimension * voxelDimension * voxelDimension;

        Debug.Log("Grid Dimension: " + voxelDimension.ToString());
        Debug.Log("Voxel Count: " + voxelCount.ToString());

        voxelGrid = new GraphicsBuffer(GraphicsBuffer.Target.Structured, voxelCount, System.Runtime.InteropServices.Marshal.SizeOf(typeof(int)));

        // Clear Voxel Grid
        particleUpdater.SetBuffer(4, "_VoxelGrid", voxelGrid);
        particleUpdater.Dispatch(4, Mathf.CeilToInt(voxelCount / threadsPerGroup), 1, 1);

        renderParams = new RenderParams(voxelMaterial);
        renderParams.worldBounds = new Bounds(Vector3.zero, 10000 * Vector3.one);
        renderParams.matProps = new MaterialPropertyBlock();

        renderParams.matProps.SetBuffer("_VoxelGrid", voxelGrid);
        renderParams.matProps.SetFloat("_VoxelSize", voxelSize);
        renderParams.matProps.SetInt("_GridSize", voxelDimension);
        renderParams.matProps.SetInt("_GridBounds", voxelBounds);

        commandBuffer = new GraphicsBuffer(GraphicsBuffer.Target.IndirectArguments, 1, GraphicsBuffer.IndirectDrawIndexedArgs.size);
        commandIndexedData = new GraphicsBuffer.IndirectDrawIndexedArgs[1];
        commandIndexedData[0].instanceCount = System.Convert.ToUInt32(voxelCount);
        commandIndexedData[0].indexCountPerInstance = voxelMesh.GetIndexCount(0);

        commandBuffer.SetData(commandIndexedData);
    }


    void OnEnable() {
        // Debug.Log(SystemInfo.graphicsDeviceName);

        // UnsafeUtility.SetLeakDetectionMode(Unity.Collections.NativeLeakDetectionMode.Enabled);
        pointMaterial = new Material(pointShader);

        InitializeMeshes();
        InitializeRenderParams();

        InitializeVoxelGrid();
    }
    
    public virtual void IterateSystem() {
        // Reset System
        int cubeRootParticleCount = Mathf.CeilToInt(Mathf.Pow(particlesPerBatch, 1.0f / 3.0f));
        particleUpdater.SetInt("_CubeResolution", cubeRootParticleCount);
        particleUpdater.SetFloat("_CubeSize", 0);

        particleUpdater.SetBuffer(0, "_VertexBuffer", pointCloudMeshes[0].GetVertexBuffer(0));
        particleUpdater.SetBuffer(0, "_IndexBuffer", pointCloudMeshes[0].GetIndexBuffer());
        particleUpdater.Dispatch(0, Mathf.CeilToInt(particlesPerBatch / threadsPerGroup), 1, 1);

        // Seed First Iteration
        int transformCount = affineTransformations.GetTransformCount();

        particleUpdater.SetInt("_TransformationCount", transformCount);
        particleUpdater.SetInt("_GenerationOffset", 0);
        particleUpdater.SetInt("_GenerationLimit", 3);
        particleUpdater.SetBuffer(3, "_VertexBuffer", pointCloudMeshes[0].GetVertexBuffer(0));
        particleUpdater.SetBuffer(3, "_Transformations", affineTransformations.GetAffineBuffer());
        particleUpdater.Dispatch(3, Mathf.CeilToInt(particlesPerBatch / threadsPerGroup), 1, 1);

        int iteratedParticles = 3;
        int previousGenerationSize = 3;
        while (iteratedParticles < particlesPerBatch) {
            int generationSize = previousGenerationSize * transformCount;

            particleUpdater.SetInt("_GenerationOffset", iteratedParticles);
            particleUpdater.SetInt("_GenerationLimit", (int)Mathf.Clamp(iteratedParticles + generationSize, 0, particlesPerBatch));

            particleUpdater.SetBuffer(3, "_VertexBuffer", pointCloudMeshes[0].GetVertexBuffer(0));
            particleUpdater.SetBuffer(3, "_Transformations", affineTransformations.GetAffineBuffer());

            
            particleUpdater.Dispatch(3, Mathf.CeilToInt(particlesPerBatch / threadsPerGroup), 1, 1);
            

            iteratedParticles += generationSize;
            previousGenerationSize = generationSize;
        }

    }

    void Voxelize() {
        // Clear Voxel Grid
        particleUpdater.SetBuffer(4, "_VoxelGrid", voxelGrid);
        particleUpdater.Dispatch(4, Mathf.CeilToInt(voxelCount / threadsPerGroup), 1, 1);

        particleUpdater.SetInt("_GridSize", Mathf.FloorToInt(voxelBounds / voxelSize));
        particleUpdater.SetInt("_GridBounds", voxelBounds);
        particleUpdater.SetBuffer(5, "_VertexBuffer", pointCloudMeshes[0].GetVertexBuffer(0));
        particleUpdater.SetBuffer(5, "_VoxelGrid", voxelGrid);
        particleUpdater.Dispatch(5, Mathf.CeilToInt(particlesPerBatch / threadsPerGroup), 1, 1);
    }

    void DrawParticles() {
        for (int i = 0; i < batchCount; ++i) {
            Graphics.RenderMesh(pointRenderParams, pointCloudMeshes[i], 0, affineTransformations.GetFinalTransform());
        }
    }

    void Update() {
        if (uncapped || Input.GetKeyDown("space")) {
            IterateSystem();
            Voxelize();
        }

        DrawParticles();

        if (renderVoxels) {
            Graphics.RenderMeshIndirect(renderParams, voxelMesh, commandBuffer, 1);
        }
    }

    void OnDisable() {
        for (int i = 0; i < batchCount; ++i) {
            var vertBuffer = pointCloudMeshes[i].GetVertexBuffer(0);
            var indexBuffer = pointCloudMeshes[i].GetIndexBuffer();

            vertBuffer.Release();
            indexBuffer.Release();
            UnityEngine.Object.Destroy(pointCloudMeshes[i]);
        }

        commandBuffer.Release();
        voxelGrid.Release();
    }

    void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(Vector3.zero, Vector3.one * voxelBounds);
    }
}
