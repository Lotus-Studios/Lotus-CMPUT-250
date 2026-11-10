using System.Collections;
using UnityEngine;

public class BuoyManager : MonoBehaviour
{
    [Header("Buoy bobbing")]
    public Transform water;
    [SerializeField] private float bobSpeed = 2.5f;
    [SerializeField] private float bobHeight = 0.15f;
    [SerializeField] private float maxRiseOffset = 2f;

    [Header("Rope")]
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private float sagAmount = 1.5f;
    [SerializeField] private int curveResolution = 10;

    private Transform[] buoys;
    private float[] initialY;
    private float[] offsets;

    void Start()
    {
        // Gather all the child buoys inside this folder
        int count = transform.childCount;
        buoys = new Transform[count];
        initialY = new float[count];
        offsets = new float[count];

        for (int i = 0; i < count; i++)
        {
            buoys[i] = transform.GetChild(i);
            initialY[i] = buoys[i].position.y; // Remember each buoy's unique starting height
            offsets[i] = (buoys[i].position.x + buoys[i].position.z) * 2f; // Offset their bobbing phase
        }
    }

    void Update()
    {
        if (water == null) return;
        float waterY = water.position.y;

        for (int i = 0; i < buoys.Length; i++)
        {
            // Only affect this specific buoy if the water has reached its starting height
            if (waterY >= initialY[i])
            {
                float maxY = initialY[i] + maxRiseOffset;
                float clampedHeight = Mathf.Min(waterY, maxY);

                float bob = Mathf.Sin((Time.time + offsets[i]) * bobSpeed) * bobHeight;

                float offset = clampedHeight + bob;

                buoys[i].position = new Vector3(buoys[i].position.x, offset, buoys[i].position.z);
            }
        }

        DrawRopes();
    }

    private void DrawRopes()
    {
        if (lineRenderer == null || buoys.Length < 2) return;

        if (water == null) return;
        float waterY = water.position.y;

        int linePoints = buoys.Length - 1;
        int totalPoints = (linePoints * curveResolution) + 1;
        lineRenderer.positionCount = totalPoints;

        int index = 0;
        for (int i = 0; i < linePoints; i++)
        {
            Vector3 point0 = buoys[i].position;
            Vector3 point2 = buoys[i + 1].position;
            Vector3 point1 = (point0 + point2) / 2;

            if (waterY < initialY[i])
            {
                point1.y -= sagAmount;
            }
            else
            {
                point1.y -= sagAmount * 0.5f;
            }

            for (int j = 0; j < curveResolution; j++)
            {
                float t = j / (float)curveResolution;

                Vector3 position = SmoothCurves(t, point0, point1, point2);
                lineRenderer.SetPosition(index, position);
                index++;
            }
        }

        // so it doesnt draw a line at 0,0,0 from the last buoy
        lineRenderer.SetPosition(index, buoys[buoys.Length - 1].position);
    }

    private Vector3 SmoothCurves(float t, Vector3 p0, Vector3 p1, Vector3 p2)
    {
        // find the point between the start (p0) and the curve control point (p1)
        Vector3 m0 = Vector3.Lerp(p0, p1, t);

        // find the point between the curve control point (p1) and the end (p2)
        Vector3 m1 = Vector3.Lerp(p1, p2, t);
        return Vector3.Lerp(m0, m1, t);
    }

    public void ResetBouys()
    {
        for (int i = 0; i < buoys.Length; i++)
        {
            buoys[i].position = new Vector3(buoys[i].position.x, initialY[i], buoys[i].position.z);
        }
    }
}