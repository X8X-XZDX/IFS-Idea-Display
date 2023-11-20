using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleInstancer : MonoBehaviour {
    public Shader particleShader;

    private Material particleMaterial;

    GraphicsBuffer commandBuffer;
    GraphicsBuffer.IndirectDrawArgs[] commandData;

    RenderParams renderParams;

    void OnEnable() {
        particleMaterial = new Material(particleShader);

        commandBuffer = new GraphicsBuffer(GraphicsBuffer.Target.IndirectArguments, 1, GraphicsBuffer.IndirectDrawArgs.size);
        commandData = new GraphicsBuffer.IndirectDrawArgs[1];

        renderParams = new RenderParams(particleMaterial);
        renderParams.worldBounds = new Bounds(Vector3.zero, 10000 * Vector3.one);
        renderParams.matProps = new MaterialPropertyBlock();
        commandData[0].instanceCount = 10;
        commandData[0].vertexCountPerInstance = 1;

        commandBuffer.SetData(commandData);
    }

    void Update() {
        Graphics.RenderPrimitivesIndirect(renderParams, MeshTopology.Points, commandBuffer, 1);
    }

    void OnDisable() {
        commandBuffer?.Dispose();

        commandBuffer = null;
        commandData = null;
        particleMaterial = null;
    }
}
