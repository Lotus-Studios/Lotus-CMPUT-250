using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreatheSquahAndStretch : MonoBehaviour
{
    [SerializeField] float breatheFrequency;
    [SerializeField] Vector3 breathScaleAddAmplitude;
    Vector3 baseScale;


    // Start is called before the first frame update
    void Start()
    {
        baseScale = transform.localScale;   
    }

    // Update is called once per frame
    void Update()
    {
        float sin = (Mathf.Sin(Time.time * breatheFrequency)+1.0f)/2;
        transform.localScale = Vector3.Lerp(baseScale, baseScale + breathScaleAddAmplitude, sin);
    }
}
