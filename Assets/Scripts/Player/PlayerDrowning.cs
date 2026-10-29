using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;
using UnityEngine.Events;

public class PlayerDrowning : MonoBehaviour
{
    [SerializeField] RisingWater water;

    [SerializeField] private Transform topOfHead;
    [SerializeField] private float timeToDrown = 2.0f;

    [SerializeField] private float deadlySpeed = -30f;
    [SerializeField] private float requiredDepthForDeath = 4f;
    [SerializeField] private LayerMask groundLayerMask;

    [SerializeField] private Volume waterPostProcess;
    [SerializeField] private float waterPostProcessLerpSpeed = 0.3f;

    private float drownTimer;
    // private bool wasUnderWater;
    private bool isDying = false;

    
    public UnityEvent playerDied;
    // [SerializeField] private Transform playerResetPosition;

    [SerializeField] private PlayerController playerController;
    private Vignette underwaterVignette;

    public float DrowningIntensity => 1f - Mathf.Clamp01(drownTimer / timeToDrown);

    // Start is called before the first frame update
    void Start()
    {
        waterPostProcess.profile.TryGet(out underwaterVignette);
    }

    // Update is called once per frame
    void Update()
    {
        if (isDying)
        {
            if (UnderwaterAudio.Instance != null)
            {
                UnderwaterAudio.Instance.EnableUnderwaterAudio();
            }

            if (ScreenFadeToBlack.Instance.IsScreenBlack)
            {
                water.ResetMe(-2.5f);
                waterPostProcess.weight = 0f;
                drownTimer = timeToDrown;

                isDying = false;
            }
            return;
        }

        bool isUnderwater = topOfHead.transform.position.y < water.transform.position.y;

        if (isUnderwater)
        {
            if (UnderwaterAudio.Instance != null)
            {
                UnderwaterAudio.Instance.EnableUnderwaterAudio();
            }

            // checks if the player is within X units of ground, if not then die otherwise they can fall and land on it.
            bool isDeep = !Physics.Raycast(playerController.transform.position, Vector3.down, requiredDepthForDeath, groundLayerMask);
            if (playerController.currentVerticalVelocity <= deadlySpeed && isDeep)
            {
                Die();
                return;
            }

            drownTimer -= Time.deltaTime;
            if (drownTimer <= 0)
            {
                Die();
            }

            if (underwaterVignette != null)
            {
                underwaterVignette.intensity.value = Mathf.Lerp(0.3f, 0.7f, DrowningIntensity);
            }

            waterPostProcess.weight = Mathf.Lerp(waterPostProcess.weight, 1.0f, waterPostProcessLerpSpeed * Time.deltaTime);
        }
        else
        {
            if (UnderwaterAudio.Instance != null)
            {
                UnderwaterAudio.Instance.DisableUnderwaterAudio();
            }

            waterPostProcess.weight = Mathf.Lerp(waterPostProcess.weight, 0.0f, waterPostProcessLerpSpeed * Time.deltaTime);

            drownTimer = timeToDrown;
        }
    }

    private void Die()
    {
        if (isDying) return; // Prevent calling this multiple times while already dying
        isDying = true; 
        playerController.ResetToCurrentCheckpoint();

        if (AudioController.Instance != null) AudioController.Instance.StopAllAudio();

        if (underwaterVignette != null)
        {
            underwaterVignette.intensity.value = 0.3f;
        }


        playerDied?.Invoke();
    }

}
