using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugTransform : MonoBehaviour {
    public Mesh particleMesh;
    public Shader particleShader;
    public ComputeShader particleUpdater;

    private Material particleMaterial;

    private RenderParams renderParams;

    private GraphicsBuffer commandBuffer, positionBuffer;
    private GraphicsBuffer.IndirectDrawIndexedArgs[] commandIndexedData;
    private ComputeBuffer attractorsBuffer;
    
    private AffineTransformations affineTransformations;

    void OnEnable() {
        affineTransformations = GetComponent<AffineTransformations>();
        particleMaterial = new Material(particleShader);

        positionBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured, 32768, System.Runtime.InteropServices.Marshal.SizeOf(typeof(Vector4)));

        
        commandBuffer = new GraphicsBuffer(GraphicsBuffer.Target.IndirectArguments, 1, GraphicsBuffer.IndirectDrawIndexedArgs.size);
        commandIndexedData = new GraphicsBuffer.IndirectDrawIndexedArgs[1];
        commandIndexedData[0].instanceCount = 32768;
        commandIndexedData[0].indexCountPerInstance = particleMesh.GetIndexCount(0);

        commandBuffer.SetData(commandIndexedData);
        
        renderParams = new RenderParams(particleMaterial);
        renderParams.worldBounds = new Bounds(Vector3.zero, 10000 * Vector3.one);
        renderParams.matProps = new MaterialPropertyBlock();

        renderParams.matProps.SetBuffer("_Positions", positionBuffer);

        particleUpdater.SetBuffer(0, "_PositionBuffer", positionBuffer);
        particleUpdater.Dispatch(0, Mathf.CeilToInt(32768.0f / 8.0f), 1, 1);
        
        attractorsBuffer = new ComputeBuffer(32, System.Runtime.InteropServices.Marshal.SizeOf(typeof(Matrix4x4)));
    }

    void Update() {
        particleUpdater.SetInt("_TransformationCount", affineTransformations.GetTransformCount());

        attractorsBuffer.SetData(affineTransformations.GetTransformData());

        Graphics.RenderMeshIndirect(renderParams, particleMesh, commandBuffer, 1);
    }

    void OnDisable() {
        commandBuffer.Release();
        attractorsBuffer.Release();
        positionBuffer.Release();

        commandBuffer = null;
        attractorsBuffer = null;
        positionBuffer = null;
        commandIndexedData = null;
        particleMaterial = null;
    }
}
