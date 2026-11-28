using UnityEngine;

public class ContinuePromptBob : MonoBehaviour
{
    [SerializeField] private float amplitude = 8f; //How far the icon will bob up and down
    [SerializeField] private float speed = 3f;

    private RectTransform rectTransform;
    private Vector2 startPosition;
    private float elapsedTime;

    [SerializeField] private float fadeDuration = 0.5f;
    private CanvasGroup canvasGroup;

    private void Awake()
    {//Get starting point information
        rectTransform = GetComponent<RectTransform>();
        startPosition = rectTransform.anchoredPosition;
        canvasGroup = GetComponent<CanvasGroup>(); //Get the canvas group to use the alpha for fade in
    }

    private void OnEnable()
    {//Start bobbing from the same position every time the icon is enabled
        elapsedTime = 0f;
        rectTransform.anchoredPosition = startPosition;
        canvasGroup.alpha = 0f;
    }

    private void Update()
    {
        canvasGroup.alpha = Mathf.Clamp01(elapsedTime / fadeDuration); //Increase alpha from 0 to 1 over fadeDuration
        elapsedTime += Time.unscaledDeltaTime; //Add time since last frame 

        float offset = Mathf.Sin(elapsedTime * speed) * amplitude; //Calculate offset using sine of the time, mulyiplied by amplitude to change vertical distance

        rectTransform.anchoredPosition = //Get new position
            startPosition + new Vector2(0f, offset);
    }
}