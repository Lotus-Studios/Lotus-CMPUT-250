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

    [Header("Object Culling")]
    [SerializeField] private Transform[] levelRoots;
    [SerializeField] private float disableDepth = 15f;

    // stores a prefab or game objects components (renderer and collider) and its higher point in world space
    private struct CullableObject
    {
        public Renderer[] renderers;
        public Collider[] colliders;
        public float highestY;
        public bool isSubmerged;
    }

    private List<CullableObject> levelObjects = new List<CullableObject>();

    public void Start()
    {
        if (levelRoots == null || levelRoots.Length == 0) return;

        // loop through the levels referenced
        foreach (Transform root in levelRoots) 
        {
            if (root == null) return;

            // check all direct children under the level root (so in beach there would be the seashells and palm trees)
            foreach (Transform child in root)
            {
                // checks if its an organizational folder by seeing if it has children and if it has no renderer and collider
                bool inFolder = child.childCount > 0 && child.GetComponent<Renderer>() == null && child.GetComponent<Collider>() == null;

                if (inFolder)
                {
                    // register all prefabs
                    foreach (Transform prefabInstance in child)
                    {
                        RegisterPrefabUnit(prefabInstance);
                    }
                }
                else
                {
                    RegisterPrefabUnit(child);
                }
            }
        }

        UpdateSubmergedObjects();
    }

    private void RegisterPrefabUnit(Transform root)
    {
        if (root.CompareTag("Player") || root.CompareTag("MainCamera")) return;

        // get all renderers
        Renderer[] rends = root.GetComponentsInChildren<Renderer>();
        if (rends.Length == 0) return;

        // get all colliders
        Collider[] allCols = root.GetComponentsInChildren<Collider>();
        List<Collider> solidCols = new List<Collider>();

        // loops through the colliders and ignores trigger colliders
        foreach (Collider c in allCols)
        {
            if (!c.isTrigger) solidCols.Add(c);
        }

        // highest Y point (so multi part prefabs dont disappear part by part and only if the entire thing is submerged)
        float maxY = float.MinValue;
        foreach (Renderer r in rends)
        {
            if (r.bounds.max.y > maxY)
            {
                maxY = r.bounds.max.y;
            }
        }

        // add to list with its stats
        levelObjects.Add(new CullableObject
        {
            renderers = rends,
            colliders = solidCols.ToArray(),
            highestY = maxY,
            isSubmerged = false
        });
    }

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
            UpdateSubmergedObjects();
            return; 

        }
        if (!allowedToMove) return;
        transform.position += Vector3.up* getCurrentRubberBandSpeed() * Time.deltaTime;
        UpdateSubmergedObjects();
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
        UpdateSubmergedObjects();
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

        UpdateSubmergedObjects();
        resetWaterEvent.Invoke();
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

    private void UpdateSubmergedObjects()
    {
        // subtracted by disableDepth so prefab must be submerged X amount befor ebeing disabled
        float waterCullY = transform.position.y - disableDepth;

        for (int i = 0; i < levelObjects.Count; i++) 
        {
            CullableObject obj = levelObjects[i];
            bool shouldBeSubmerged = waterCullY > obj.highestY;

            // if object submerged stat changed then loop through it
            if (shouldBeSubmerged != obj.isSubmerged)
            {
                obj.isSubmerged = shouldBeSubmerged;
                levelObjects[i] = obj;

                // toggle renderers and colliders
                bool visible = !shouldBeSubmerged;
                foreach (Renderer r in obj.renderers)
                {
                    if (r != null) r.enabled = visible;
                }
                foreach (Collider c in obj.colliders)
                { 
                    if (c != null) c.enabled = visible;
                }
            }
        }
    }
}
