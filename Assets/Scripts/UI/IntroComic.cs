using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroComic : MonoBehaviour
{
    // Start is called before the first frame update
    public enum SlideDirection
    {
        Left,
        Right,
        Top,
        Bottom
    }

    [SerializeField] private SlideDirection slideFrom = SlideDirection.Left;
    [SerializeField] private float slideDuration = 0.75f;

    [SerializeField] private SlideDirection slideTo = SlideDirection.Right;
    [SerializeField] private float exitDuration = 0.5f;

    private RectTransform rectTransform;
    private Vector2 finalPosition;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();

        
        finalPosition = rectTransform.anchoredPosition; //The positions are already set in the editor
    }

    // Move the panel off-screen before its animation.
    public void Prepare()
    {
        RectTransform parent = (RectTransform)rectTransform.parent; //Get the panels parent page

        float horizontal = parent.rect.width + rectTransform.rect.width;
        float vertical = parent.rect.height + rectTransform.rect.height;

        Vector2 offset = slideFrom switch
        {
            SlideDirection.Left => Vector2.left * horizontal,
            SlideDirection.Right => Vector2.right * horizontal,
            SlideDirection.Top => Vector2.up * vertical,
            SlideDirection.Bottom => Vector2.down * vertical,
            _ => Vector2.zero
        };

        rectTransform.anchoredPosition = finalPosition + offset;
    }

    
    public IEnumerator SlideIn() //Animate the panel sliding into it's final position
    {
        Vector2 startPosition = rectTransform.anchoredPosition;
        float elapsed = 0f;
        float duration = Mathf.Max(0.01f, slideDuration);

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;

            float t = Mathf.Clamp01(elapsed / duration); //Calculate animatuib progress

            
            t = 1f - Mathf.Pow(1f - t, 3f); //Start fast then slow

            rectTransform.anchoredPosition = Vector2.Lerp(startPosition, finalPosition, t); //Move the panel

            yield return null;
        }

        rectTransform.anchoredPosition = finalPosition;
    }


    public IEnumerator SlideOut() //Animate all panels sliding out of the screen at the same time, basically same logic as slide in but reversed
    {
        RectTransform parent = (RectTransform)rectTransform.parent;

        float horizontal = parent.rect.width + rectTransform.rect.width;
        float vertical = parent.rect.height + rectTransform.rect.height;

        Vector2 offset = slideTo switch
        {
            SlideDirection.Left => Vector2.left * horizontal,
            SlideDirection.Right => Vector2.right * horizontal,
            SlideDirection.Top => Vector2.up * vertical,
            SlideDirection.Bottom => Vector2.down * vertical,
            _ => Vector2.zero
        };

        Vector2 startPosition = rectTransform.anchoredPosition;
        Vector2 exitPosition = finalPosition + offset;

        float elapsed = 0f;
        float duration = Mathf.Max(0.01f, exitDuration);

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;

            float t = Mathf.Clamp01(elapsed / duration);

            
            t = t * t * t; //Start slow then speed up

            rectTransform.anchoredPosition =
                Vector2.Lerp(startPosition, exitPosition, t);

            yield return null;
        }

        rectTransform.anchoredPosition = exitPosition;
    }

}
