using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    //[SerializeField] private bool pauseWater = true;
    [Header("Respawn Settings")]
    [SerializeField] private Transform customSpawnPoint;

    [Header("Water Settings")]
    [SerializeField] private RisingWater water;
    [SerializeField] private float waterOffsetBelowCheckpoint = 3f;
    [SerializeField] private float waterRiseSpeed = 0.7f; // For moving the water to the checkpoint

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {

                Vector3 mySpawnPos = customSpawnPoint != null ? customSpawnPoint.position : transform.position;

                if (player.currentCheckpointPosition == mySpawnPos) return;

                // save the previous checkpoint and water level to history before the update
                player.checkpointHistory.Add(player.currentCheckpointPosition);
                player.waterLevelHistory.Add(player.currentWaterLevel);

                player.currentCheckpointPosition = mySpawnPos;

                if (water != null)
                {
                    // Calculate where the water should be at below the checkpoint
                    float targetWaterLevel = player.currentCheckpointPosition.y - waterOffsetBelowCheckpoint;
                    player.currentWaterLevel = targetWaterLevel;
                    water.RiseToCheckpoint(targetWaterLevel, waterRiseSpeed);
                }

                if (AudioLayering.Instance != null)
                {
                    AudioLayering.Instance.FadeToChill();
                }
            }
        }
    }
}