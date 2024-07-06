using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleInstancer : MonoBehaviour {
    public Shader particleShader;
    public ComputeShader particleUpdater;

    public enum Attractor {
        Custom = 1,
        SierpinskiTriangle2D,
        Vicsek2D,
    } public Attractor attractor = Attractor.SierpinskiTriangle2D;
    private Attractor cachedAttractor;

    [Range(0.0f, 2.0f)]
    public float r = 0.5f;

    private Material particleMaterial;

    private GraphicsBuffer commandBuffer, originBuffer, destinationBuffer;
    private GraphicsBuffer.IndirectDrawArgs[] commandData;

    private Vector3[] customAttractorPositions;
    private Vector3[] sierpinskiTriangle2DAttractors = {
        new Vector3(0.0f, 0.5f, 0.0f),
        new Vector3(0.35f, 0.0f, 0.0f),
        new Vector3(-0.35f, 0.0f, 0.0f)
    };

    private Vector3[] Vicsek2DAttractors = {
        new Vector3(-0.5f, 0.0f, 0.0f),
        new Vector3(0.5f, 0.0f, 0.0f),
        new Vector3(0.0f, 0.5f, 0.0f),
        new Vector3(-0.5f, 1.0f, 0.0f),
        new Vector3(0.5f, 1.0f, 0.0f)
    };

    private RenderParams renderParams;

    private ComputeBuffer attractorsBuffer;

    private float t = 0.0f;

    private uint particleCount = 200000;

    void OnEnable() {
        particleMaterial = new Material(particleShader);

        originBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured | GraphicsBuffer.Target.CopyDestination, (int)particleCount, System.Runtime.InteropServices.Marshal.SizeOf(typeof(Vector3)));
        destinationBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured | GraphicsBuffer.Target.CopySource, (int)particleCount, System.Runtime.InteropServices.Marshal.SizeOf(typeof(Vector3)));

        commandBuffer = new GraphicsBuffer(GraphicsBuffer.Target.IndirectArguments, 1, GraphicsBuffer.IndirectDrawArgs.size);
        commandData = new GraphicsBuffer.IndirectDrawArgs[1];

        renderParams = new RenderParams(particleMaterial);
        renderParams.worldBounds = new Bounds(Vector3.zero, 10000 * Vector3.one);
        renderParams.matProps = new MaterialPropertyBlock();
        commandData[0].instanceCount = particleCount;
        commandData[0].vertexCountPerInstance = 1;

        renderParams.matProps.SetBuffer("_Origins", originBuffer);
        renderParams.matProps.SetBuffer("_Destinations", destinationBuffer);

        commandBuffer.SetData(commandData);

        particleUpdater.SetBuffer(0, "_PositionBuffer", originBuffer);
        particleUpdater.Dispatch(0, Mathf.CeilToInt(particleCount / 8.0f), 1, 1);
        particleUpdater.SetBuffer(0, "_PositionBuffer", destinationBuffer);
        particleUpdater.Dispatch(0, Mathf.CeilToInt(particleCount / 8.0f), 1, 1);

        List<Transform> attractorTransforms = new List<Transform>();
        GetComponentsInChildren<Transform>(attractorTransforms);
        attractorTransforms.RemoveAt(0);

        customAttractorPositions = new Vector3[32];
        attractorsBuffer = new ComputeBuffer(32, System.Runtime.InteropServices.Marshal.SizeOf(typeof(Vector3)));
        attractorsBuffer.SetData(customAttractorPositions);

        UpdateAttractor();

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
                List<Transform> attractorTransforms = new List<Transform>();
                GetComponentsInChildren<Transform>(attractorTransforms);
                attractorTransforms.RemoveAt(0);

                for (int i = 0; i < attractorTransforms.Count; ++i) {
                    customAttractorPositions[i] = attractorTransforms[i].position;
                }
                
                attractorsBuffer.SetData(customAttractorPositions);
                particleUpdater.SetInt("_PointCount", attractorTransforms.Count);
                particleUpdater.SetFloat("_R", r);
            break;
            case Attractor.SierpinskiTriangle2D:
                attractorsBuffer.SetData(sierpinskiTriangle2DAttractors);
                particleUpdater.SetInt("_PointCount", 3);
                particleUpdater.SetFloat("_R", 0.5f);
            break;
            case Attractor.Vicsek2D:
                attractorsBuffer.SetData(Vicsek2DAttractors);
                particleUpdater.SetInt("_PointCount", 5);
                particleUpdater.SetFloat("_R", 0.33f);
            break;
        }

        cachedAttractor = attractor;
    }

    void Update() {
        particleUpdater.SetFloat("_Time", Time.time);
        particleUpdater.SetFloat("_DeltaTime", Time.deltaTime);

        if (attractor == Attractor.Custom || attractor != cachedAttractor)
            UpdateAttractor();

        t += Time.deltaTime * 2.0f;

        if (t >= 1) {
            Graphics.CopyBuffer(destinationBuffer, originBuffer);

            particleUpdater.SetBuffer(1, "_PositionBuffer", destinationBuffer);
            particleUpdater.SetBuffer(1, "_Attractors", attractorsBuffer);
            particleUpdater.Dispatch(1, Mathf.CeilToInt(particleCount / 8.0f), 1, 1);

            t = 0;
        }

        renderParams.matProps.SetFloat("_Interpolator", t);
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
