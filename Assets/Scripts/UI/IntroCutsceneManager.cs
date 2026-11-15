using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroCutsceneManager : MonoBehaviour
{
    [SerializeField] private GameObject cutsceneCanvas;
    [SerializeField] private GameObject[] pages;

    [SerializeField] private float delayBetweenPanels = 0.2f;
    [SerializeField] private float pageHoldDuration = 2f;

    private bool isPlaying = false;
    [SerializeField] private bool skipIntroForDebug = false;
    private bool hasPlayedIntro = false;

    public void PlayIntro()
    {
        if (isPlaying) return;

        
        if (skipIntroForDebug || hasPlayedIntro)// Skip if debugging or the intro has already played.
        {
            GameManager.Instance.GoToGame();
            return;
        }

        isPlaying = true;
        hasPlayedIntro = true;

        // Make sure all pages start hidden.
        foreach (GameObject page in pages)
        {
            page.SetActive(false);
        }

        cutsceneCanvas.SetActive(true);

        StartCoroutine(PlayPages());
    }

    private IEnumerator PlayPages()
    {
        for (int i = 0; i < pages.Length; i++)
        {
            GameObject page = pages[i];

            
            page.SetActive(true);// Activate the current page.

            // Find all animated panels on this page.
            IntroComic[] panels = page.GetComponentsInChildren<IntroComic>(true);

            
            foreach (IntroComic panel in panels)// Move every panel to its starting position.
            {
                panel.Prepare();
            }

            // Animate the panels sequentially.
            for (int j = 0; j < panels.Length; j++)
            {
                yield return StartCoroutine(panels[j].SlideIn());

                if (j < panels.Length - 1)
                {
                    yield return new WaitForSecondsRealtime(delayBetweenPanels);
                }
            }

                
                yield return new WaitForSecondsRealtime(pageHoldDuration);

                
                Coroutine[] exitCoroutines = new Coroutine[panels.Length]; //Run all exit animations at the same time

                for (int j = 0; j < panels.Length; j++)
                {
                    exitCoroutines[j] = StartCoroutine(panels[j].SlideOut());
                }

                // Wait for every panel to finish exiting.
                foreach (Coroutine exit in exitCoroutines)
                {
                    yield return exit;
                }

                // Hide the page once all panels have exited.
                page.SetActive(false);
        }


        ScreenFadeToBlack fade = ScreenFadeToBlack.Instance;

        void OnBlack()
        {
            Destroy(cutsceneCanvas);


            fade.beginWaitOnBlack.RemoveListener(OnBlack);// Remove the listener

            isPlaying = false;
            Resources.UnloadUnusedAssets();
        }

        
        fade.beginWaitOnBlack.AddListener(OnBlack);// Listen for the midpoint of the fade.

        
        GameManager.Instance.GoToGame(); //Start the game


        
    }
}
