using UnityEngine;

[ExecuteAlways]
public class EnvironmentBillboard : MonoBehaviour
{
    [Header("Billboard Settings")]
    [Range(0f, 1f)]
    public float cameraTiltBlend = 0.3f;

    private Transform[] grassTransforms;

    void OnEnable()
    {
        RefreshGrassList();
    }

    void RefreshGrassList()
    {
        MeshRenderer[] meshes = GetComponentsInChildren<MeshRenderer>();
        grassTransforms = new Transform[meshes.Length];

        for (int i = 0; i < meshes.Length; i++)
        {
            grassTransforms[i] = meshes[i].transform;
        }
    }

    void LateUpdate()
    {
        Camera targetCam = Camera.main;

#if UNITY_EDITOR
        if (!Application.isPlaying && UnityEditor.SceneView.lastActiveSceneView != null)
        {
            targetCam = UnityEditor.SceneView.lastActiveSceneView.camera;
        }
#endif

        if (targetCam == null || grassTransforms == null) return;

        float camYaw = targetCam.transform.eulerAngles.y;
        float camPitch = targetCam.transform.eulerAngles.x;

        // Calculate the blended X pitch 
        float targetX = 90f - (camPitch * cameraTiltBlend);

        for (int i = 0; i < grassTransforms.Length; i++)
        {
            if (grassTransforms[i] != null)
            {
                grassTransforms[i].rotation = Quaternion.Euler(targetX, camYaw + 180f, 0f);
            }
        }
    }
}