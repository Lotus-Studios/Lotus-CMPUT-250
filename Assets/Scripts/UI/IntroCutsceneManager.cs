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

    public void PlayIntro()
    {
        if (isPlaying) return;

        isPlaying = true;

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

            // Keep the finished page visible.
            yield return new WaitForSecondsRealtime(pageHoldDuration);

            // Hide the page before displaying the next one.
            if (i < pages.Length - 1)
            {
                page.SetActive(false);
            }
        }

        
    }
}
