using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaosGame : MonoBehaviour {
    public bool usePoints = false;
    public Mesh particleMesh;
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

    public bool manual = false;

    [Range(1, 20)]
    public int iterationCount = 1;

    public bool uncapped = false;

    private Material particleMaterial;

    private GraphicsBuffer commandBuffer, positionBuffer;
    private GraphicsBuffer.IndirectDrawArgs[] commandData;
    private GraphicsBuffer.IndirectDrawIndexedArgs[] commandIndexedData;

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

    private uint particleCount = 200000;

    private AffineTransformations affineTransformations;

    void OnEnable() {
        affineTransformations = GetComponent<AffineTransformations>();

        particleMaterial = new Material(particleShader);

        positionBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured, (int)particleCount, System.Runtime.InteropServices.Marshal.SizeOf(typeof(Vector4)));
        
        if (usePoints) {
            commandBuffer = new GraphicsBuffer(GraphicsBuffer.Target.IndirectArguments, 1, GraphicsBuffer.IndirectDrawArgs.size);
            commandData = new GraphicsBuffer.IndirectDrawArgs[1];
            commandData[0].instanceCount = particleCount;
            commandData[0].vertexCountPerInstance = 1;

            commandBuffer.SetData(commandData);
        } else {
            commandBuffer = new GraphicsBuffer(GraphicsBuffer.Target.IndirectArguments, 1, GraphicsBuffer.IndirectDrawIndexedArgs.size);
            commandIndexedData = new GraphicsBuffer.IndirectDrawIndexedArgs[1];
            commandIndexedData[0].instanceCount = particleCount;
            commandIndexedData[0].indexCountPerInstance = particleMesh.GetIndexCount(0);

            commandBuffer.SetData(commandIndexedData);
        }


        renderParams = new RenderParams(particleMaterial);
        renderParams.worldBounds = new Bounds(Vector3.zero, 10000 * Vector3.one);
        renderParams.matProps = new MaterialPropertyBlock();

        renderParams.matProps.SetBuffer("_Positions", positionBuffer);

        int cubeRootParticleCount = Mathf.CeilToInt(Mathf.Pow(particleCount, 1.0f / 3.0f));
        particleUpdater.SetInt("_CubeResolution", cubeRootParticleCount);
        particleUpdater.SetFloat("_CubeSize", 1.0f / cubeRootParticleCount);

        particleUpdater.SetBuffer(0, "_PositionBuffer", positionBuffer);
        particleUpdater.Dispatch(0, Mathf.CeilToInt(particleCount / 8.0f), 1, 1);

        attractorsBuffer = new ComputeBuffer(32, System.Runtime.InteropServices.Marshal.SizeOf(typeof(Matrix4x4)));
    }


    void IterateSystem() {
        for (int i = 0; i < iterationCount; ++i) {
            particleUpdater.SetInt("_Seed", Mathf.CeilToInt(Random.Range(1, 1000000)));
            particleUpdater.SetBuffer(2, "_PositionBuffer", positionBuffer);
            particleUpdater.SetBuffer(2, "_Transformations", attractorsBuffer);
            particleUpdater.Dispatch(2, Mathf.CeilToInt(particleCount / 8.0f), 1, 1);
        }
    }

    void FillAffineTransform(ref Matrix4x4 matrix, Vector4 r1, Vector4 r2, Vector4 r3) {
        matrix.SetRow(0, r1);
        matrix.SetRow(1, r2);
        matrix.SetRow(2, r3);
        matrix.SetRow(3, new Vector4(0, 0, 0, 1));
    }

    void Update() {
        particleUpdater.SetInt("_TransformationCount", affineTransformations.GetTransformCount());

        attractorsBuffer.SetData(affineTransformations.GetTransformData());

        if (uncapped) {
            IterateSystem();
        } else {
            if (Input.GetKeyDown("space")) {
                IterateSystem();
            }
        }

        if (usePoints)
            Graphics.RenderPrimitivesIndirect(renderParams, MeshTopology.Points, commandBuffer, 1);
        else
            Graphics.RenderMeshIndirect(renderParams, particleMesh, commandBuffer, 1);
    }

    void OnDisable() {
        commandBuffer.Release();
        attractorsBuffer.Release();
        positionBuffer.Release();

        commandBuffer = null;
        attractorsBuffer = null;
        positionBuffer = null;
        commandData = null;
        particleMaterial = null;
    }
}
