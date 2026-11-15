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
}
