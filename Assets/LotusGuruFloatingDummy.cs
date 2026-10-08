using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class LotusGuruFloatingDummy : MonoBehaviour
{
    [SerializeField] RisingWater water;

    [SerializeField] PlayerDrowning playerDrowning;
    bool floatingState;

    [SerializeField] Transform mountainCenter;

    [Header("Bobbing")]
    [SerializeField] float bobFrequency = 1.0f;
    [SerializeField] float bobAmplitude = 0.2f;

    [SerializeField] float driftAwaySpeed = 0.2f;

    [SerializeField] float justFloatTime = 5.0f;
    float justFloatTimer = 5.0f;

    [SerializeField] Transform floatMark;

    [SerializeField] float distanceAwayToDestroy = 15.0f;

    [SerializeField] SpriteRenderer standingSprite;
    [SerializeField] SpriteRenderer lilypadSprite;

    [SerializeField] Transform resetToTransform;



    // [Header("drifting")]
    // [SerializeField] float driftFrequency = 0.2f;
    // [SerializeField] float driftAmplitude;



    // Start is called before the first frame update
    void Start()
    {
        ResetMe();
        water.resetWaterEvent.AddListener(ResetMe);
    }

    // Update is called once per frame
    void Update()
    {
        if (!floatingState)
        {
            if(water.transform.position.y >= floatMark.transform.position.y)
            {
                floatingState = true;
                standingSprite.enabled = false;
                lilypadSprite.enabled = true;
            }
        }

        if (floatingState)
        {
            transform.position = new Vector3(transform.position.x, 
            water.transform.position.y - floatMark.localPosition.y  + Mathf.Sin(Time.time * bobFrequency)*bobAmplitude, 
            transform.position.z);

            if(justFloatTimer > 0) 
            {
                justFloatTimer -= Time.deltaTime;
            }
            else
            {
                Vector3 flatDirectionVector = new Vector3(transform.position.x - mountainCenter.position.x, 0, transform.position.z - mountainCenter.position.z).normalized;
                transform.position += flatDirectionVector * driftAwaySpeed * Time.deltaTime;
            }
        }
    }

    public void setResetTransform(Transform tr)
    {
        resetToTransform = tr;
    }
    
    public void ResetMe()
    {
        justFloatTimer = justFloatTime;
        transform.position = resetToTransform.position;
        floatingState = false;
        standingSprite.enabled = true;
        lilypadSprite.enabled = false;
    }
}
