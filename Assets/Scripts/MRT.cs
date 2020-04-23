using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class MRT : MonoBehaviour
{
    private const int MRT_COUNT = 2;

    private Camera camera;
    private Material deffered_material;
 
    private RenderBuffer[] buffers = new RenderBuffer[MRT_COUNT];
    private RenderTexture[] texes = new RenderTexture[MRT_COUNT];

    void OnEnable()
    {
        for (int i = 0; i < MRT_COUNT; i++) {
            texes[i] = new RenderTexture(Screen.width, Screen.height, 24, RenderTextureFormat.ARGB32);
            buffers[i] = texes[i].colorBuffer;
        }

        if (!deffered_material) {
            var shader = Shader.Find("karonator/Deffered");

            if (shader != null) {
                deffered_material = new Material(shader);
                deffered_material.hideFlags = HideFlags.DontSave;
                
                for (int i = 0; i < texes.Length; i++) {
                    deffered_material.SetTexture("_Tex" + i, texes[i]);
                }
            }
        }

        camera = GetComponent<Camera>();
        camera.depthTextureMode = DepthTextureMode.Depth;
        camera.SetTargetBuffers(buffers, texes[0].depthBuffer);
    }

    void OnPreRender()
    {
        LightsManager LM = GetComponent<LightsManager>();
        deffered_material.SetVectorArray("_LightsPositions", LM.lightsPositions());
        deffered_material.SetVectorArray("_LightsColors", LM.lightsColors());
    }

    void OnDisable()
    {
        if (deffered_material) {
            DestroyImmediate(deffered_material);
            deffered_material = null;
        }
    }

    void OnRenderImage (RenderTexture source, RenderTexture destination)
    {
        Matrix4x4 matrixCameraToWorld = camera.cameraToWorldMatrix;
        Matrix4x4 matrixProjectionInverse = GL.GetGPUProjectionMatrix(camera.projectionMatrix, false).inverse;
        Matrix4x4 matrixHClipToWorld = matrixCameraToWorld * matrixProjectionInverse;

        deffered_material.SetMatrix("clipToWorld", matrixHClipToWorld);

        Graphics.Blit(source, destination, deffered_material, 0);
    }

}