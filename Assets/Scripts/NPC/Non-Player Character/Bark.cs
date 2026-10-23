using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Bark : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textDisplay;

    [SerializeField] private string text;
    [SerializeField] private float totalDuration = 1f;

    private Vector3 targetScale;

    private Vector3 currentScale = Vector3.zero;

    [SerializeField] private float scaleIncreaseRate = 0.25f;

    private int scaleDirection = 1;

    private float currentDuration = 0f;
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("New bark initialized.");
        targetScale = gameObject.transform.localScale;
        gameObject.transform.localScale = currentScale;
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log($"Text Displayed: {textDisplay.text}");
        updateScale();
        if(scaleDirection == 0)
        {
            currentDuration += Time.deltaTime;
            if(currentDuration >= totalDuration)
            {
                textDisplay.text = "";
                scaleDirection = -1;
            }
        }

        if(scaleDirection == 1)
        {
            if(transform.localScale.x > targetScale.x)
            {
                scaleDirection = 0;
                textDisplay.text = text;
            }
        }

        if(scaleDirection == -1)
        {
            
            if(transform.localScale.x <= 0)
            {
                Destroy(transform.parent.gameObject);
            }
        }
    }

    void updateScale()
    {
        Vector3 increase = new Vector3(
            scaleIncreaseRate * scaleDirection * Time.deltaTime, 
            scaleIncreaseRate * scaleDirection * Time.deltaTime, 
             0);
        
        transform.localScale += increase;
    }

    public void setText(string str) { 
        Debug.Log($"Bark text set to {str}");
        text = str; 
        }
    public void setDuration(float dur) { 
        Debug.Log($"Bark duration set to {dur}");

        totalDuration = dur; 
        }
}
