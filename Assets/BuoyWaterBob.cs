using UnityEngine;

public class BuoyManager : MonoBehaviour
{
    public Transform water;
    [SerializeField] private float bobSpeed = 2.5f;
    [SerializeField] private float bobHeight = 0.15f;
    [SerializeField] private float maxRiseOffset = 5f;

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
                float bob = Mathf.Sin((Time.time + offsets[i]) * bobSpeed) * bobHeight;

                float height = Mathf.Min(waterY + bob, waterY + bob + maxRiseOffset);

                buoys[i].position = new Vector3(buoys[i].position.x, height, buoys[i].position.z);
            }
        }
    }
}