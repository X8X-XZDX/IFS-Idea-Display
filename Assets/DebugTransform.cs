using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugTransform : MonoBehaviour {
    public Mesh particleMesh;
    public Shader particleShader;
    public ComputeShader particleUpdater;

    public enum DebugMode {
        InterpolatePositionFromOrigin = 0,
        InterpolateAffineTransform,
        InterpolateInstructions,
        InterpolateInstructionsQuaternion
    } public DebugMode debugMode;

    public int debugIndex = 0;
    public Vector2 affineIndices = new Vector2(0, 0);

    [Range(0, 1)]
    public float t = 0;

    public bool animate = false;
    [Range(0, 10)]
    public float speed = 1.0f;
    
    public Vector3 translate = new Vector3(0, 0, 0);

    private float bounce = 1;

    private Material particleMaterial;

    private RenderParams renderParams;

    private GraphicsBuffer commandBuffer, positionBuffer, destinationBuffer;
    private GraphicsBuffer.IndirectDrawIndexedArgs[] commandIndexedData;
    
    private AffineTransformations affineTransformations;

    void OnEnable() {
        affineTransformations = GetComponent<AffineTransformations>();
        particleMaterial = new Material(particleShader);

        positionBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured, 512, System.Runtime.InteropServices.Marshal.SizeOf(typeof(Vector4)));
        destinationBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured, 512, System.Runtime.InteropServices.Marshal.SizeOf(typeof(Vector4)));

        
        commandBuffer = new GraphicsBuffer(GraphicsBuffer.Target.IndirectArguments, 1, GraphicsBuffer.IndirectDrawIndexedArgs.size);
        commandIndexedData = new GraphicsBuffer.IndirectDrawIndexedArgs[1];
        commandIndexedData[0].instanceCount = 512;
        commandIndexedData[0].indexCountPerInstance = particleMesh.GetIndexCount(0);

        commandBuffer.SetData(commandIndexedData);
        
        renderParams = new RenderParams(particleMaterial);
        renderParams.worldBounds = new Bounds(Vector3.zero, 10000 * Vector3.one);
        renderParams.matProps = new MaterialPropertyBlock();

        renderParams.matProps.SetBuffer("_Origins", positionBuffer);
        renderParams.matProps.SetBuffer("_Destinations", destinationBuffer);

        particleUpdater.SetInt("_CubeResolution", 8);
        particleUpdater.SetFloat("_CubeSize", 1.0f / 8.0f);
        particleUpdater.SetBuffer(0, "_PositionBuffer", positionBuffer);
        particleUpdater.Dispatch(0, Mathf.CeilToInt(512.0f / 8.0f), 1, 1);
        particleUpdater.SetBuffer(0, "_PositionBuffer", destinationBuffer);
        particleUpdater.Dispatch(0, Mathf.CeilToInt(512.0f / 8.0f), 1, 1);
    }

    void Update() {
        

        if (debugIndex > (affineTransformations.GetTransformCount() - 1))
            debugIndex = affineTransformations.GetTransformCount() - 1;

        if (debugMode == DebugMode.InterpolatePositionFromOrigin)
            particleUpdater.SetMatrix("_DebugTransform", affineTransformations.GetTransformData()[debugIndex]);
        else if (debugMode == DebugMode.InterpolateAffineTransform)
            particleUpdater.SetMatrix("_DebugTransform", affineTransformations.InterpolateAffineTransform((int)affineIndices.x, (int)affineIndices.y, t));
        else if (debugMode == DebugMode.InterpolateInstructions)
            particleUpdater.SetMatrix("_DebugTransform", affineTransformations.InterpolateInstructionsDebug((int)affineIndices.x, (int)affineIndices.y, t, false));
        else if (debugMode == DebugMode.InterpolateInstructionsQuaternion)
            particleUpdater.SetMatrix("_DebugTransform", affineTransformations.InterpolateInstructionsDebug((int)affineIndices.x, (int)affineIndices.y, t));


        // Reset Destination Particles
        particleUpdater.SetBuffer(0, "_PositionBuffer", destinationBuffer);
        particleUpdater.Dispatch(0, Mathf.CeilToInt(512.0f / 8.0f), 1, 1);

        // Apply Debug Transform
        particleUpdater.SetBuffer(1, "_PositionBuffer", destinationBuffer);
        particleUpdater.Dispatch(1, Mathf.CeilToInt(512.0f / 8.0f), 1, 1);

        
        if (animate) {
            t += Time.deltaTime * bounce * speed;
            if (t > 1) {
                t = 1;
                bounce = -1;
            } else if (t < 0) {
                t = 0;
                bounce = 1;
            }
        }

        if (debugMode == DebugMode.InterpolatePositionFromOrigin)
            renderParams.matProps.SetFloat("_Interpolator", t);
        else
            renderParams.matProps.SetFloat("_Interpolator", 1);

        renderParams.matProps.SetVector("_Translate", translate);

        Graphics.RenderMeshIndirect(renderParams, particleMesh, commandBuffer, 1);
    }

    void OnDisable() {
        commandBuffer.Release();
        positionBuffer.Release();
        destinationBuffer.Release();

        commandBuffer = null;
        positionBuffer = null;
        commandIndexedData = null;
        particleMaterial = null;
    }
}
