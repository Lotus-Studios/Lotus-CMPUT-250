using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*  Videos:
 *  https://www.youtube.com/watch?v=v2pRG7n_-cs - Unity - Screen fader
 *  
 *  Forum:
 *  https://discussions.unity.com/t/fade-to-black/657361 - Ignore the code btw, its outdated and OnGUI is apparently notoriously laggy. The theory is good
 *  
 *  Documentation:
 *  https://docs.unity3d.com/2022.3/Documentation/ScriptReference/Canvas.html
 */
public class ScreenFadeToBlack : MonoBehaviour
{
    private static ScreenFadeToBlack _instance;
    public static ScreenFadeToBlack Instance { get { return _instance; } }

    [SerializeField] private float fadeDuration = 0.4f;
    [SerializeField] private float waitOnBlackDuration = 0.4f;

    private CanvasGroup canvasGroup;
    private float timer = 0f;

    private enum FadeState { Idle, FadingOut, WaitingOnBlack, FadingIn } // state the fade is at
    private FadeState currentState = FadeState.Idle;

    private enum FadePurpose { None, RespawnPlayer, BasicFade } // reason for the fade so I can track it down below
    private FadePurpose currentPurpose = FadePurpose.None;

    private PlayerController savedPlayer;
    private Vector3 savedPosition;

    public bool IsScreenBlack => currentState == FadeState.WaitingOnBlack;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        _instance = this;
        canvasGroup = GetComponent<CanvasGroup>();
    }

    // Fade and resets the player, use for deaths and such
    public void FadeAndRespawn(PlayerController player, Vector3 targetPosition)
    {
        if (currentState != FadeState.Idle) return;

        savedPlayer = player;
        savedPosition = targetPosition;
        currentPurpose = FadePurpose.RespawnPlayer;

        savedPlayer.canMove = false; 

        StartFade();
    }

    // Standard fade that menus and such can call to use
    public void BasicFade()
    {
        if (currentState != FadeState.Idle) return;

        currentPurpose = FadePurpose.BasicFade;
        StartFade();
    }

    private void StartFade()
    {
        timer = 0f;
        currentState = FadeState.FadingOut;
    }


    private void Update()
    {
        if (currentState == FadeState.Idle) return;

        timer += Time.deltaTime;

        if (currentState == FadeState.FadingOut)
        {
            canvasGroup.alpha = Mathf.Clamp01(timer / fadeDuration);

            if (timer >= fadeDuration)
            {
                canvasGroup.alpha = 1f;

                // midpoint so teleport the player
                if (currentPurpose == FadePurpose.RespawnPlayer)
                {
                    savedPlayer.teleport(savedPosition);
                }
                // standard fade does nothing at the midpoint

                timer = 0f;
                currentState = FadeState.WaitingOnBlack;
            }
        }
        else if (currentState == FadeState.WaitingOnBlack)
        {
            if (timer >= waitOnBlackDuration)
            {
                timer = 0f;
                currentState = FadeState.FadingIn;
            }
        }
        else if (currentState == FadeState.FadingIn)
        {
            canvasGroup.alpha = 1f - Mathf.Clamp01(timer / fadeDuration);

            if (timer >= fadeDuration)
            {
                canvasGroup.alpha = 0f;

                // Fade ended so clear everything
                if (currentPurpose == FadePurpose.RespawnPlayer)
                {
                    savedPlayer.canMove = true;
                }

                currentPurpose = FadePurpose.None;
                currentState = FadeState.Idle;
            }
        }
    }
}
