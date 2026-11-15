using UnityEngine;

[ExecuteAlways]
public class BillboardManager: MonoBehaviour
{
    [Range(0f, 1f)]
    public float cameraTiltBlend = 0.3f;

    private Camera cachedMainCam;

    private static readonly int PitchID = Shader.PropertyToID("_BillboardPitch");
    private static readonly int SpritePitchID = Shader.PropertyToID("_BillboardSpritePitch");
    private static readonly int YawID = Shader.PropertyToID("_BillboardYaw");

    void OnEnable()
    {
        cachedMainCam = Camera.main;
    }

    void LateUpdate()
    {
        if (cachedMainCam == null) cachedMainCam = Camera.main;
        Camera targetCam = cachedMainCam;

#if UNITY_EDITOR
        if (!Application.isPlaying && UnityEditor.SceneView.lastActiveSceneView != null)
        {
            targetCam = UnityEditor.SceneView.lastActiveSceneView.camera;
        }
#endif
        if (targetCam == null) return;

        float camYaw = targetCam.transform.eulerAngles.y;
        float camPitch = targetCam.transform.eulerAngles.x;

        float targetX = 90f - (camPitch * cameraTiltBlend);
        Shader.SetGlobalFloat(SpritePitchID, -(camPitch * cameraTiltBlend));
        float targetY = camYaw + 180f;

        Shader.SetGlobalFloat(PitchID, targetX);
        Shader.SetGlobalFloat(YawID, targetY);
    }
}