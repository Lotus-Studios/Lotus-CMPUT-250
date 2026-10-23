using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;
using Unity.Mathematics;
using System.ComponentModel;

public class barkAnimatedCircle : MonoBehaviour
{
    [SerializeField] private SplineContainer splineContainer;
    [SerializeField] private float speed = 3f;
    [SerializeField] private float offset = 0f;

    private float distance;
    
    void Start()
    {
        transform.position = splineContainer.EvaluatePosition(0f);
        distance += offset;
    }

    // Update is called once per frame
    void Update()
    {
        distance += speed * Time.deltaTime;

        float t = SplineUtility.GetNormalizedInterpolation(
            splineContainer.Spline, distance, PathIndexUnit.Distance);

        float3 pos = splineContainer.EvaluatePosition(t);
        transform.position = pos;
    }
}
