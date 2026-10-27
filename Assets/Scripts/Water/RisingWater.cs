using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class RisingWater : MonoBehaviour
{
    //TODO: make use of allowedToMove by waiting for player to jump/move/finish talking to Lotus 
    public bool allowedToMove = true;

    // distance sampling is relative to this transform
    [SerializeField] Transform playerTarget;

    [Header("Water Respawn")]
    public float respawnLevel = -3f;

    //curve to sample for how fast to go
    [Header("Water Rising Speed")]
    [SerializeField] AnimationCurve waterRubberBandCurve;
    [SerializeField] float curveMaxSpeed = 3f;
    [SerializeField] float curveMinSpeed = 0.7f;
    [SerializeField] float maxDistance = 25;
    [SerializeField] float minDistance = 3;

    private bool isRisingToCheckpoint = false;
    private float targetRiseLevel;
    private float currentRiseSpeed;

    [HideInInspector]
    public UnityEvent resetWaterEvent;

    private List<Renderer> submergedRenderers = new List<Renderer>();
    private List<Collider> submergedColliders = new List<Collider>();

    // Update is called once per frame
    void Update()
    {
        if (isRisingToCheckpoint)
        {
            Vector3 pos = transform.position;
            pos.y = Mathf.Lerp(pos.y, targetRiseLevel, currentRiseSpeed * Time.deltaTime);
            transform.position = pos;

            // because Mathf.Lerp keeps halving the distance, if its small enough just snap it.
            if (Mathf.Abs(transform.position.y - targetRiseLevel) <= 0.05f)
            {
                pos.y = targetRiseLevel;
                transform.position = pos;

                isRisingToCheckpoint = false;
                allowedToMove = false; // for the start water rising trigger at the start of the actual stage
            }
            return; 

        }
        if (!allowedToMove) return;
        transform.position += Vector3.up* getCurrentRubberBandSpeed() * Time.deltaTime;
    }

    float getCurrentRubberBandSpeed()
    {
        float distance = playerTarget.position.y - transform.position.y;
        distance = Mathf.Clamp(distance,minDistance, maxDistance);
        float curveSamplePercent = (distance - minDistance) / (maxDistance - minDistance);

        // so it scales from min speed to max speed
        float curveWeight = waterRubberBandCurve.Evaluate(curveSamplePercent);
        return Mathf.Lerp(curveMinSpeed, curveMaxSpeed, curveWeight);
    }

    public void RiseToCheckpoint(float targetY, float riseSpeed)
    {
        respawnLevel = targetY;
        targetRiseLevel = targetY;
        currentRiseSpeed = riseSpeed;

        allowedToMove = false; 
        isRisingToCheckpoint = true;
        RestoreSubmergedObjects();
    }

    //We can decide later whether water is responsible for knowing about checkpoints or not, basic implemenation here
    public void ResetMe(float yPosition)
    {
        isRisingToCheckpoint = false;
        allowedToMove = false;

        // water is reset at the level of the checkpoint
        Vector3 pos = transform.position;
        pos.y = respawnLevel;
        transform.position = pos;

        RestoreSubmergedObjects();
        resetWaterEvent.Invoke();
    }

    private void RestoreSubmergedObjects()
    {
        foreach (Renderer rend in submergedRenderers)
        {
            if (rend != null) rend.enabled = true;
        }
        foreach (Collider col in submergedColliders)
        {
            if (col != null) col.enabled = true;
        }

        submergedRenderers.Clear();
        submergedColliders.Clear();
    }

    public void StartMe()
    {
        allowedToMove = true;
        isRisingToCheckpoint = false;
    }

    public bool isWaterRising()
    {
        return isRisingToCheckpoint;
    }

    // Game objects that hit the trigger stops being rendered and collision is disabled, gets culled
    private void OnTriggerEnter(Collider other)
    {
        // Ignore the player, checkpoints, or cameras
        if (other.CompareTag("Player") || other.CompareTag("MainCamera") || other.isTrigger) return;

        StartCoroutine(WaitUntilSubmerged(other));
    }

    // Only disable the game object if the water is higher than its highest point
    private IEnumerator WaitUntilSubmerged(Collider col)
    {
        yield return new WaitUntil(() => transform.position.y > col.bounds.max.y);

        Renderer rend = col.GetComponent<Renderer>();
        if (rend != null && rend.enabled)
        {
            rend.enabled = false;
            submergedRenderers.Add(rend);
        }

        if (col.enabled)
        {
            col.enabled = false;
            submergedColliders.Add(col);
        }
    }
}
