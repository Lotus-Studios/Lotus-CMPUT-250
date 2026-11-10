using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class PlayerShadow : MonoBehaviour
{
    private DecalProjector decalProjector;

    [Header("Shadow Settings")]
    [SerializeField] private float maxDistance = 25f;
    [SerializeField] private float shrinkDistance = 4f;
    [SerializeField] private float baseSize = 0.7f;
    [SerializeField] private float playerRadius = 0.35f;
    [SerializeField] private float castStartOffset = 0.5f;
    [SerializeField] private Transform shadowSpriteTransform;

    [SerializeField] private PlayerController playerController; //Needed for shadow cheating

    public LayerMask groundLayer;

    void Start()
    {
        decalProjector = GetComponent<DecalProjector>();
        if (playerController == null)
        {
            playerController = GetComponentInParent<PlayerController>();
        }

    }

    void Update()
    {
        transform.rotation = Quaternion.LookRotation(Vector3.down, Vector3.forward);

        if (playerController != null && PredictLanding(out RaycastHit predictedHit) && !playerController.IsGrounded)
        {
            Debug.DrawLine(
                playerController.transform.position,
                predictedHit.point,
                Color.green
            );

            transform.position = new Vector3(
                predictedHit.point.x,
                playerController.transform.position.y,
                predictedHit.point.z
            );

        }
        else if (playerController != null)
        {
            transform.position = playerController.transform.position;
        }

        Vector3 origin = transform.position + Vector3.up * castStartOffset;
        if (Physics.SphereCast(origin, playerRadius, Vector3.down, out RaycastHit hit, maxDistance + castStartOffset, groundLayer))
        {
            // hit.distance is the spheres travel distance
            float surfaceDist = transform.position.y - hit.point.y;
            float heightPercent = Mathf.Clamp01(surfaceDist / shrinkDistance);
            decalProjector.fadeFactor = Mathf.Lerp(1f, 0.4f, heightPercent);

            // Shrink the width and height of the decal
            float currentSize = Mathf.Lerp(baseSize, baseSize * 0.5f, heightPercent);
            float totalDist = Mathf.Max(surfaceDist, 0.05f) + 0.2f;
            float downwardOffset = playerRadius;
            float depth = Mathf.Max(totalDist - downwardOffset, 0.05f);

            decalProjector.size = new Vector3(currentSize, currentSize, depth);
            // Push pivot forward (down along local Z) past the sprite
            decalProjector.pivot = new Vector3(0f, 0f, downwardOffset + (depth * 0.5f));
            
        }
        else
        {
            decalProjector.fadeFactor = 0f;
        }
    }

    private bool PredictLanding(out RaycastHit landingHit)
    {
        landingHit = default;

        Vector3 position = playerController.transform.position;
        Vector3 horizontalVelocity = playerController.CurrentHorizontalVelocity;
        float verticalVelocity = playerController.currentVerticalVelocity;

        float gravity = playerController.Gravity;
        float terminalVelocity = playerController.TerminalVelocity;

        float timeStep = 0.02f; //How much time passes with each prediction step
        float maxPredictionTime = 3f; //Limit how far into the future we predict landing
        float playerRadius = 0.35f;

        int steps = Mathf.CeilToInt(maxPredictionTime / timeStep); //How many steps the iteration will take

        for (int i = 0; i < steps; i++)
        {
            Vector3 previousPosition = position;

            // Simulate gravity
            verticalVelocity += gravity * timeStep;
            verticalVelocity = Mathf.Max(verticalVelocity, terminalVelocity);

            // Calculate the next predicted position
            position += (horizontalVelocity + Vector3.up * verticalVelocity) * timeStep;

            Vector3 movement = position - previousPosition;
            float distance = movement.magnitude;

            // Check for collisions along the predicted trajectory
            if (movement.y < 0f && Physics.SphereCast(previousPosition,playerRadius,movement.normalized,out RaycastHit hit,distance,groundLayer,QueryTriggerInteraction.Ignore))
            {
                // Only accept surfaces that face upward
                if (hit.normal.y >= 0.7f)
                {
                    landingHit = hit;
                    return true;
                }
            }
        }

        return false;
    }
}
