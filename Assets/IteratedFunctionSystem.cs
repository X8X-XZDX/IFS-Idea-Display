using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IteratedFunctionSystem : MonoBehaviour {
    public AffineTransformations affineTransformations;
    
    public Shader pointShader, cubeShader;

    public ComputeShader particleUpdater;

    public enum ParticleMeshMode {
        Point = 0,
        Quad,
        Cube
    } public ParticleMeshMode particleMeshMode;

    public uint particleCount = 200000;

    public bool uncapped = false;

    private Mesh quadMesh, cubeMesh;
    private Material pointMaterial, cubeMaterial;

    public GraphicsBuffer positionBuffer;
    private GraphicsBuffer pointCommandBuffer, quadCommandBuffer, cubeCommandBuffer;
    private GraphicsBuffer.IndirectDrawArgs[] pointCommandData;
    private GraphicsBuffer.IndirectDrawIndexedArgs[] quadCommandData, cubeCommandData;

    private RenderParams pointRenderParams, meshRenderParams;

    void InitializeCommandBuffers() {
        pointCommandBuffer = new GraphicsBuffer(GraphicsBuffer.Target.IndirectArguments, 1, GraphicsBuffer.IndirectDrawArgs.size);

        pointCommandData = new GraphicsBuffer.IndirectDrawArgs[1];
        pointCommandData[0].instanceCount = particleCount;
        pointCommandData[0].vertexCountPerInstance = 1;

        pointCommandBuffer.SetData(pointCommandData);

        quadCommandBuffer = new GraphicsBuffer(GraphicsBuffer.Target.IndirectArguments, 1, GraphicsBuffer.IndirectDrawIndexedArgs.size);

        quadCommandData = new GraphicsBuffer.IndirectDrawIndexedArgs[1];
        quadCommandData[0].instanceCount = particleCount;
        quadCommandData[0].indexCountPerInstance = quadMesh.GetIndexCount(0);

        quadCommandBuffer.SetData(quadCommandData);

        cubeCommandBuffer = new GraphicsBuffer(GraphicsBuffer.Target.IndirectArguments, 1, GraphicsBuffer.IndirectDrawIndexedArgs.size);
        
        cubeCommandData = new GraphicsBuffer.IndirectDrawIndexedArgs[1];
        cubeCommandData[0].instanceCount = particleCount;
        cubeCommandData[0].indexCountPerInstance = cubeMesh.GetIndexCount(0);

        cubeCommandBuffer.SetData(cubeCommandData);
    }

    void InitializeRenderParams() {
        pointRenderParams = new RenderParams(pointMaterial);
        pointRenderParams.worldBounds = new Bounds(Vector3.zero, 10000 * Vector3.one);
        pointRenderParams.matProps = new MaterialPropertyBlock();
        
        pointRenderParams.matProps.SetBuffer("_Positions", positionBuffer);

        meshRenderParams = new RenderParams(cubeMaterial);
        meshRenderParams.worldBounds = new Bounds(Vector3.zero, 10000 * Vector3.one);
        meshRenderParams.matProps = new MaterialPropertyBlock();
        
        meshRenderParams.matProps.SetBuffer("_Positions", positionBuffer);
    }

    void InitializeParticleBuffer() {
        positionBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured, (int)particleCount, System.Runtime.InteropServices.Marshal.SizeOf(typeof(Vector4)));

        int cubeRootParticleCount = Mathf.CeilToInt(Mathf.Pow(particleCount, 1.0f / 3.0f));

        particleUpdater.SetInt("_CubeResolution", cubeRootParticleCount);
        particleUpdater.SetFloat("_CubeSize", 1.0f / cubeRootParticleCount);
        particleUpdater.SetBuffer(0, "_PositionBuffer", positionBuffer);
        particleUpdater.Dispatch(0, Mathf.CeilToInt(particleCount / 8.0f), 1, 1);
    }


    void OnEnable() {
        quadMesh = Resources.GetBuiltinResource<Mesh>("Quad.fbx");
        cubeMesh = Resources.GetBuiltinResource<Mesh>("Cube.fbx");

        pointMaterial = new Material(pointShader);
        cubeMaterial = new Material(cubeShader);

        InitializeParticleBuffer();
        InitializeCommandBuffers();
        InitializeRenderParams();
    }

    GraphicsBuffer GetCommandBuffer() {
        switch(particleMeshMode) {
            case ParticleMeshMode.Point:
                return pointCommandBuffer;
            case ParticleMeshMode.Quad:
                return quadCommandBuffer;
            case ParticleMeshMode.Cube:
                return cubeCommandBuffer;
        }

        return pointCommandBuffer;
    }

    RenderParams GetRenderParams() {
        switch(particleMeshMode) {
            case ParticleMeshMode.Point:
                return pointRenderParams;
            case ParticleMeshMode.Quad:
                return meshRenderParams;
            case ParticleMeshMode.Cube:
                return meshRenderParams;
        }

        return pointRenderParams;
    }

    Mesh GetMesh() {
        switch(particleMeshMode) {
            case ParticleMeshMode.Quad:
                return quadMesh;
            case ParticleMeshMode.Cube:
                return cubeMesh;
        }

        return quadMesh;
    }
    
    public virtual void IterateSystem() {
        particleUpdater.SetInt("_TransformationCount", affineTransformations.GetTransformCount());

        particleUpdater.SetInt("_Seed", Mathf.CeilToInt(Random.Range(1, 1000000)));
        particleUpdater.SetBuffer(2, "_PositionBuffer", positionBuffer);
        particleUpdater.SetBuffer(2, "_Transformations", affineTransformations.GetAffineBuffer());
        particleUpdater.Dispatch(2, Mathf.CeilToInt(particleCount / 8.0f), 1, 1);
    }

    void InstanceParticles() {
        GraphicsBuffer commandBuffer = GetCommandBuffer();
        RenderParams renderParams = GetRenderParams();

        renderParams.matProps.SetMatrix("_FinalTransform", affineTransformations.GetFinalTransform());

        if (particleMeshMode == ParticleMeshMode.Point)
            Graphics.RenderPrimitivesIndirect(renderParams, MeshTopology.Points, commandBuffer, 1);
        else
            Graphics.RenderMeshIndirect(renderParams, GetMesh(), commandBuffer, 1);
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
        quadCommandBuffer.Release();
        cubeCommandBuffer.Release();
        positionBuffer.Release();
    }
}
