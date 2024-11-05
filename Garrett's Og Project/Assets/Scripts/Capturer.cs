using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Capturer : MonoBehaviour {
    public bool recording = false;

    public Vector2Int resolution = new Vector2Int(0, 0);
    public int framesToCapture = 1;

    private Camera cam;
    private int frameCount = 0;

    private RenderTexture rt;
    private Texture2D screenshot;

    void OnEnable() {
        frameCount = 0;
        cam = GetComponent<Camera>();
    }

    void LateUpdate() {
        if (recording && frameCount < framesToCapture) {
            if (frameCount == 0) {
                rt = new RenderTexture(resolution.x, resolution.y, 0);
                screenshot = new Texture2D(resolution.x, resolution.y, TextureFormat.RGB24, false);
            }
            
            GetComponent<Camera>().targetTexture = rt;
            GetComponent<Camera>().Render();

            RenderTexture.active = rt;
            screenshot.ReadPixels(new Rect(0, 0, resolution.x, resolution.y), 0, 0);

            GetComponent<Camera>().targetTexture = null;
            RenderTexture.active = null;
            
            string filename = string.Format("{0}/../Recordings/{1:000000}.png", Application.dataPath, frameCount);
            System.IO.File.WriteAllBytes(filename, screenshot.EncodeToPNG());


            ++frameCount;
            if (frameCount == framesToCapture) {
                recording = false;
                frameCount = 0;

                rt.Release();
                Destroy(screenshot);
                rt = null;
                screenshot = null;
            }
        }
    }

    void OnDisable() {
        if (rt != null) {
            rt.Release();
            rt = null;
        }

        if (screenshot != null) {
            Destroy(screenshot);
            screenshot = null;
        }
    }
}
