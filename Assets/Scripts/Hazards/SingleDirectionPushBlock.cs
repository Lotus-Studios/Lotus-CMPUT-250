using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class SingleDirectionPushBlock : MonoBehaviour
{
    private Vector3 startPosition;
    private Vector3 targetPosition;
    private float stateTimer;
    private Vector3 lastPosition;
    private Vector3 movementDelta;

    //[SerializeField] private float targetDistance = 4f;
    [SerializeField] private float waitTimeAtStart = 1.5f;
    [SerializeField] private float waitTimeAtEnd = 0.5f;
    [SerializeField] private float moveTime = 1f;
    [SerializeField] private float initialStartDelay = 0f; // to stagger push blocks if you want to

    [SerializeField] float inWallZDistance = 7f;
    [SerializeField] float extendedZDistance = 0f;

    [SerializeField] private AudioClip pushBlockSound;
    [SerializeField] private AudioSource audioSource;


    private PlayerController playerOnBlock;

    private enum MoveState
    {
        WaitingAtStart,
        MovingOut,
        WaitingAtEnd,
        MovingBack
    };

    private MoveState currentState = MoveState.WaitingAtStart;

    void Start()
    {
        startPosition = new Vector3(0,0, inWallZDistance);
        targetPosition = new Vector3(0,0,extendedZDistance);
        lastPosition = transform.position;
        stateTimer = -initialStartDelay;
    }

    void Update()
    {
        stateTimer += Time.deltaTime;

        if (currentState == MoveState.WaitingAtStart)
        {
            if (stateTimer >= waitTimeAtStart)
            {
                currentState = MoveState.MovingOut;
                stateTimer = 0f;
            }
        }

        else if (currentState == MoveState.MovingOut)
        {
            float progress = stateTimer / moveTime;

            transform.localPosition = Vector3.Lerp(
                startPosition,
                targetPosition,
                progress
            );
            AudioController.Instance.PlayFromAudioSource(pushBlockSound, audioSource);

            if (progress >= 1f)
            {
                transform.localPosition = targetPosition;
                currentState = MoveState.WaitingAtEnd;
                stateTimer = 0f;
                StartCoroutine(AudioController.Instance.StopAudioSource(audioSource));
            }
        }

        else if (currentState == MoveState.WaitingAtEnd)
        {
            if (stateTimer >= waitTimeAtEnd)
            {
                currentState = MoveState.MovingBack;
                stateTimer = 0f;
            }
        }

        else if (currentState == MoveState.MovingBack)
        {
            float progress = stateTimer / moveTime;

            transform.localPosition = Vector3.Lerp(
                targetPosition,
                startPosition,
                progress
            );
            AudioController.Instance.PlayFromAudioSource(pushBlockSound, audioSource);

            if (progress >= 1f)
            {
                transform.localPosition = startPosition;
                currentState = MoveState.WaitingAtStart;
                stateTimer = 0f;
                StartCoroutine(AudioController.Instance.StopAudioSource(audioSource));
            }
        }

        movementDelta = transform.position - lastPosition;
        lastPosition = transform.position;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.TryGetComponent<PlayerController>(out PlayerController player))
        {
            playerOnBlock = player;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<PlayerController>(out PlayerController player))
        {
            if (player == playerOnBlock)
            {
                playerOnBlock = null;
            }
        }
    }

    private void LateUpdate()
    {
        if (playerOnBlock != null)
        {
            playerOnBlock.ApplyExternalMovement(movementDelta);
        }
    }
}
