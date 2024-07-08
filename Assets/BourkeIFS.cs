using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BourkeIFS : MonoBehaviour {
    public Shader particleShader;
    public ComputeShader particleUpdater;

    struct IFSParameters {
        public float a;
        public float b;
        public float c;
        public float d;
        public float e;
        public float f;
    };

    IFSParameters CreateParams(float x, float y, float z, float w, float v, float u) {
        IFSParameters ifs = new IFSParameters();

        ifs.a = x;
        ifs.b = y;
        ifs.c = z;
        ifs.d = w;
        ifs.e = v;
        ifs.f = u;

        return ifs;
    }

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

    [Range(0.0f, 5.0f)]
    public float size = 1.0f;

    [Range(0.0f, 20.0f)]
    public float speed = 1.0f;

    public bool manual = false;
    public bool pauseAtTop = false;

    [Range(1, 100)]
    public int maxGenerations = 10;
    [Range(1, 20)]
    public int iterationCount = 1;

    private Material particleMaterial;

    private GraphicsBuffer commandBuffer, originBuffer, destinationBuffer;
    private GraphicsBuffer.IndirectDrawArgs[] commandData;


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

        
        IFSParameters[] customAttractorPositions = new IFSParameters[4];
        customAttractorPositions[0] = CreateParams(0.14f, 0.01f, 0.0f, 0.51f, -0.08f, -1.31f);
        customAttractorPositions[1] = CreateParams(0.43f, 0.52f, -0.45f, 0.5f, 1.49f, -0.75f);
        customAttractorPositions[2] = CreateParams(0.45f, -0.49f, 0.47f, 0.47f, -1.62f, -0.74f);
        customAttractorPositions[3] = CreateParams(0.49f, 0.0f, 0.0f, 0.51f, 0.02f, 1.62f);
        
        attractorsBuffer = new ComputeBuffer(4, System.Runtime.InteropServices.Marshal.SizeOf(typeof(IFSParameters)));
        attractorsBuffer.SetData(customAttractorPositions);

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
            break;
        }

        cachedAttractor = attractor;
    }

    void IterateSystem() {
        Graphics.CopyBuffer(destinationBuffer, originBuffer);

        for (int i = 0; i < iterationCount; ++i) {
            particleUpdater.SetFloat("_Size", size);
            particleUpdater.SetInt("_PointCount", 4);
            particleUpdater.SetBuffer(2, "_PositionBuffer", destinationBuffer);
            particleUpdater.SetBuffer(2, "_BourkeAttractors", attractorsBuffer);
            particleUpdater.Dispatch(2, Mathf.CeilToInt(particleCount / 8.0f), 1, 1);

            currentGen += 1;
        }

        t = 0;
    }

    void Update() {
        particleUpdater.SetFloat("_Time", Time.time);
        particleUpdater.SetFloat("_DeltaTime", Time.deltaTime);

        if (attractor != cachedAttractor) {
            currentGen = 0;
            UpdateAttractor();
        }

        if (t <= 1.0f)
            t += Time.deltaTime * speed;

        if ( currentGen >= maxGenerations && !pauseAtTop) t = 1.0f;

        if (manual) {
            if (Input.GetKeyDown("space")) {
                IterateSystem();
            }
        } else if (t >= 1.0f) {
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
