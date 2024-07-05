using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleInstancer : MonoBehaviour {
    public Shader particleShader;
    public ComputeShader particleUpdater;

    [Range(0.0f, 2.0f)]
    public float r = 0.5f;

    private Material particleMaterial;

    private GraphicsBuffer commandBuffer;
    private GraphicsBuffer.IndirectDrawArgs[] commandData;

    private RenderParams renderParams;

    private ComputeBuffer particlePositionBuffer;

    private uint particleCount = 200000;

    void OnEnable() {
        particleMaterial = new Material(particleShader);

        particlePositionBuffer = new ComputeBuffer((int)particleCount, System.Runtime.InteropServices.Marshal.SizeOf(typeof(Vector3)));

        commandBuffer = new GraphicsBuffer(GraphicsBuffer.Target.IndirectArguments, 1, GraphicsBuffer.IndirectDrawArgs.size);
        commandData = new GraphicsBuffer.IndirectDrawArgs[1];

        renderParams = new RenderParams(particleMaterial);
        renderParams.worldBounds = new Bounds(Vector3.zero, 10000 * Vector3.one);
        renderParams.matProps = new MaterialPropertyBlock();
        commandData[0].instanceCount = particleCount;
        commandData[0].vertexCountPerInstance = 1;

        renderParams.matProps.SetBuffer("_ParticlePositions", particlePositionBuffer);

        commandBuffer.SetData(commandData);
        particleUpdater.SetBuffer(0, "_PositionBuffer", particlePositionBuffer);
        particleUpdater.Dispatch(0, Mathf.CeilToInt(particleCount / 8.0f), 1, 1);

        // Vector3[] data = new Vector3[particleCount];
        // particlePositionBuffer.GetData(data);

        // for (int i = 0; i < particleCount; ++i) {
        //     Debug.Log(data[i]);
        // }
    }

    void Update() {
        particleUpdater.SetFloat("_Time", Time.time);
        particleUpdater.SetFloat("_R", r);
        particleUpdater.SetBuffer(1, "_PositionBuffer", particlePositionBuffer);
        particleUpdater.Dispatch(1, Mathf.CeilToInt(particleCount / 8.0f), 1, 1);

        Graphics.RenderPrimitivesIndirect(renderParams, MeshTopology.Points, commandBuffer, 1);
    }

    void OnDisable() {
        commandBuffer.Release();
        particlePositionBuffer.Release();

        commandBuffer = null;
        particlePositionBuffer = null;
        commandData = null;
        particleMaterial = null;
    }
}