using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaosGame : MonoBehaviour {
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

    [Range(0.0f, 2.0f)]
    public float r = 0.5f;
    
    [Range(0.0f, 2.0f)]
    public float rScale = 1.0f;

    [Range(0.0f, 5.0f)]
    public float size = 1.0f;

    [Range(0.0f, 20.0f)]
    public float speed = 1.0f;

    public bool manual = false;

    [Range(1, 100)]
    public int maxGenerations = 10;
    [Range(1, 20)]
    public int iterationCount = 1;

    private Material particleMaterial;

    private GraphicsBuffer commandBuffer, originBuffer, destinationBuffer;
    private GraphicsBuffer.IndirectDrawArgs[] commandData;

    private Vector3[] customAttractorPositions;

    private Vector3[] SierpinskiTriangle2DAttractors = {
        new Vector3(-0.5f, -0.5f, 0.0f),
        new Vector3(0.0f, 0.36f, 0.0f),
        new Vector3(0.5f, -0.5f, 0.0f)
    };

    private Vector3[] Vicsek2DAttractors = {
        new Vector3(-0.5f, -0.5f, 0.0f),
        new Vector3(-0.5f, 0.5f, 0.0f),
        new Vector3(0.5f, 0.5f, 0.0f),
        new Vector3(0.5f, -0.5f, 0.0f),
        new Vector3(0.0f, 0.0f, 0.0f)
    };

    private Vector3[] SierpinskiCarpet2DAttractors = {
        new Vector3(-0.5f, -0.5f, 0.0f),
        new Vector3(-0.5f, 0.5f, 0.0f),
        new Vector3(0.5f, 0.5f, 0.0f),
        new Vector3(0.5f, -0.5f, 0.0f),
        new Vector3(-0.5f, 0.0f, 0.0f),
        new Vector3(0.5f, 0.0f, 0.0f),
        new Vector3(0.0f, 0.5f, 0.0f),
        new Vector3(0.0f, -0.5f, 0.0f)
    };

    private Vector3[] SierpinskiTriangle3DAttractors = {
        new Vector3(-0.5f, -0.5f, 0.5f),
        new Vector3(-0.5f, -0.5f, -0.5f),
        new Vector3(0.5f, -0.5f, 0.5f),
        new Vector3(0.5f, -0.5f, -0.5f),
        new Vector3(0.0f, 0.36f, 0.0f),
        new Vector3(0.5f, -0.5f, 0.0f)
    };

    private Vector3[] Vicsek3DAttractors = {
        new Vector3(-0.5f, -0.5f, -0.5f),
        new Vector3(-0.5f, -0.5f, 0.5f),
        new Vector3(0.5f, -0.5f, -0.5f),
        new Vector3(0.5f, -0.5f, 0.5f),
        new Vector3(-0.5f, 0.5f, -0.5f),
        new Vector3(-0.5f, 0.5f, 0.5f),
        new Vector3(0.5f, 0.5f, -0.5f),
        new Vector3(0.5f, 0.5f, 0.5f),
        new Vector3(0.0f, 0.0f, 0.0f)
    };

    private Vector3[] SierpinskiCarpet3DAttractors = {
        new Vector3(-0.5f, -0.5f, -0.5f),
        new Vector3(-0.5f, -0.5f, 0.5f),
        new Vector3(0.5f, -0.5f, -0.5f),
        new Vector3(0.5f, -0.5f, 0.5f),
        new Vector3(-0.5f, 0.5f, -0.5f),
        new Vector3(-0.5f, 0.5f, 0.5f),
        new Vector3(0.5f, 0.5f, -0.5f),
        new Vector3(0.5f, 0.5f, 0.5f),
        new Vector3(-0.5f, 0.5f, 0.0f),
        new Vector3(0.5f, 0.5f, 0.0f),
        new Vector3(-0.5f, -0.5f, 0.0f),
        new Vector3(0.5f, -0.5f, 0.0f),
        new Vector3(0.0f, 0.5f, -0.5f),
        new Vector3(0.0f, 0.5f, 0.5f),
        new Vector3(0.0f, -0.5f, -0.5f),
        new Vector3(0.0f, -0.5f, 0.5f),
        new Vector3(-0.5f, 0.0f, -0.5f),
        new Vector3(0.5f, 0.0f, 0.5f),
        new Vector3(0.5f, 0.0f, -0.5f),
        new Vector3(-0.5f, 0.0f, 0.5f)
    };

    private RenderParams renderParams;

    private ComputeBuffer attractorsBuffer;

    private float t = 0.0f;
    private int currentGen = 0;

    private uint particleCount = 200000;

    void OnEnable() {
        particleMaterial = new Material(particleShader);

        originBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured | GraphicsBuffer.Target.CopyDestination, (int)particleCount, System.Runtime.InteropServices.Marshal.SizeOf(typeof(Vector4)));
        destinationBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured | GraphicsBuffer.Target.CopySource, (int)particleCount, System.Runtime.InteropServices.Marshal.SizeOf(typeof(Vector4)));

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
        currentGen = 0;
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
                attractorsBuffer.SetData(SierpinskiTriangle2DAttractors);
                particleUpdater.SetInt("_PointCount", SierpinskiTriangle2DAttractors.Length);
                particleUpdater.SetFloat("_R", 0.5f);
            break;
            case Attractor.Vicsek2D:
                attractorsBuffer.SetData(Vicsek2DAttractors);
                particleUpdater.SetInt("_PointCount", Vicsek2DAttractors.Length);
                particleUpdater.SetFloat("_R", 0.33f);
            break;
            case Attractor.SierpinskiCarpet2D:
                attractorsBuffer.SetData(SierpinskiCarpet2DAttractors);
                particleUpdater.SetInt("_PointCount", SierpinskiCarpet2DAttractors.Length);
                particleUpdater.SetFloat("_R", 0.33f);
            break;
            case Attractor.SierpinskiTriangle3D:
                attractorsBuffer.SetData(SierpinskiTriangle3DAttractors);
                particleUpdater.SetInt("_PointCount", SierpinskiTriangle3DAttractors.Length);
                particleUpdater.SetFloat("_R", 0.5f);
            break;
            case Attractor.Vicsek3D:
                attractorsBuffer.SetData(Vicsek3DAttractors);
                particleUpdater.SetInt("_PointCount", Vicsek3DAttractors.Length);
                particleUpdater.SetFloat("_R", 0.33f);
            break;
            case Attractor.SierpinskiCarpet3D:
                attractorsBuffer.SetData(SierpinskiCarpet3DAttractors);
                particleUpdater.SetInt("_PointCount", SierpinskiCarpet3DAttractors.Length);
                particleUpdater.SetFloat("_R", 0.33f);
            break;
        }

        cachedAttractor = attractor;
    }

    void IterateSystem() {
        if (currentGen < maxGenerations) {
            Graphics.CopyBuffer(destinationBuffer, originBuffer);

            for (int i = 0; (i < iterationCount) && (currentGen < maxGenerations); ++i) {
                particleUpdater.SetFloat("_Size", size);
                particleUpdater.SetFloat("_RScale", rScale);
                particleUpdater.SetBuffer(1, "_PositionBuffer", destinationBuffer);
                particleUpdater.SetBuffer(1, "_Attractors", attractorsBuffer);
                particleUpdater.Dispatch(1, Mathf.CeilToInt(particleCount / 8.0f), 1, 1);
                currentGen += 1;
            }

            t = 0;
        }
    }

    void Update() {
        particleUpdater.SetFloat("_Time", Time.time);
        particleUpdater.SetFloat("_DeltaTime", Time.deltaTime);

        if (attractor == Attractor.Custom)
            UpdateAttractor();

        if (attractor != cachedAttractor) {
            currentGen = 0;
            UpdateAttractor();
        }

        if (t <= 1)
            t += Time.deltaTime * speed;

        if (manual) {
            if (Input.GetKeyDown("space")) {
                IterateSystem();
            }
        } else if (t >= 1) {
            IterateSystem();
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
