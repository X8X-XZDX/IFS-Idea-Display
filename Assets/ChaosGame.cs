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

    public bool uncapped = false;

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

    public List<Matrix4x4> affineTransforms = new List<Matrix4x4>();

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

        Matrix4x4[] customAttractorPositions = new Matrix4x4[4];
        attractorsBuffer = new ComputeBuffer(4, System.Runtime.InteropServices.Marshal.SizeOf(typeof(Matrix4x4)));
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
        // switch(attractor) {

        // }

        cachedAttractor = attractor;
    }

    void IterateSystem() {
        if (currentGen < maxGenerations) {
            Graphics.CopyBuffer(destinationBuffer, originBuffer);

            for (int i = 0; (i < iterationCount) && (currentGen < maxGenerations); ++i) {
                particleUpdater.SetInt("_Seed", Mathf.CeilToInt(Random.Range(1, 1000000)));
                particleUpdater.SetFloat("_Size", size);
                particleUpdater.SetFloat("_RScale", rScale);
                particleUpdater.SetBuffer(4, "_PositionBuffer", destinationBuffer);
                particleUpdater.SetBuffer(4, "_Transformations", attractorsBuffer);
                particleUpdater.Dispatch(4, Mathf.CeilToInt(particleCount / 8.0f), 1, 1);
                currentGen += 1;
            }

            t = 0;
        }
    }

    void FillAffineTransform(ref Matrix4x4 matrix, Vector4 r1, Vector4 r2, Vector4 r3) {
        matrix.SetRow(0, r1);
        matrix.SetRow(1, r2);
        matrix.SetRow(2, r3);
        matrix.SetRow(3, new Vector4(0, 0, 0, 1));
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

        Matrix4x4[] customAttractorPositions = new Matrix4x4[4];

        // FillAffineTransform(ref customAttractorPositions[0], 
        //     new Vector4(0.14f, 0.01f, 0.0f, -0.08f),
        //     new Vector4(0.0f, 0.51f, 0.0f, -1.31f),
        //     new Vector4(0.0f, 0.0f, 1.0f, 0.0f)
        // );
        // FillAffineTransform(ref customAttractorPositions[1], 
        //     new Vector4(0.43f, 0.52f, 0.0f, 1.49f),
        //     new Vector4(-0.45f, 0.5f, 0.0f, -0.75f),
        //     new Vector4(0.0f, 0.0f, 1.0f, 0.0f)
        // );
        // FillAffineTransform(ref customAttractorPositions[2], 
        //     new Vector4(0.45f, -0.49f, 0.0f, -1.62f),
        //     new Vector4(0.47f, 0.47f, 0.0f, -0.74f),
        //     new Vector4(0.0f, 0.0f, 1.0f, 0.0f)
        // );
        // FillAffineTransform(ref customAttractorPositions[3], 
        //     new Vector4(0.49f, 0.0f, 0.0f, 0.02f),
        //     new Vector4(0.0f, 0.51f, 0.0f, 1.62f),
        //     new Vector4(0.0f, 0.0f, 1.0f, 0.0f)
        // );

        // particleUpdater.SetInt("_TransformationCount", 4);

        
        FillAffineTransform(ref customAttractorPositions[0], 
            new Vector4(0.5f, 0.0f, 0.0f, -0.08f),
            new Vector4(0.0f, 0.5f, 0.0f, 1.31f),
            new Vector4(0.0f, 0.0f, 1.0f, 0.0f)
        );
        FillAffineTransform(ref customAttractorPositions[1], 
            new Vector4(0.5f, 0.0f, 0.0f, 1.49f),
            new Vector4(0.0f, 0.5f, 0.0f, -0.75f),
            new Vector4(0.0f, 0.0f, 1.0f, 0.0f)
        );
        FillAffineTransform(ref customAttractorPositions[2], 
            new Vector4(0.5f, 0.0f, 0.0f, -1.62f),
            new Vector4(0.0f, 0.5f, 0.0f, -0.74f),
            new Vector4(0.0f, 0.0f, 1.0f, 0.0f)
        );

        particleUpdater.SetInt("_TransformationCount", affineTransforms.Count);

        attractorsBuffer.SetData(affineTransforms.ToArray());

        if (uncapped) {
            currentGen = 0;
            IterateSystem();
        } else {
            if (t <= 1)
                t += Time.deltaTime * speed;

            if (manual) {
                if (Input.GetKeyDown("space")) {
                    IterateSystem();
                }
            } else if (t >= 1) {
                IterateSystem();
            }
        }

        renderParams.matProps.SetFloat("_Interpolator", 1);
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
