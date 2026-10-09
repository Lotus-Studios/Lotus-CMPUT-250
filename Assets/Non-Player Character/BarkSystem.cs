using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarkSystem : MonoBehaviour
{
    private static BarkSystem _bs;

    private BarkSystem() { }
    public static BarkSystem Instance { get { return _bs; }}
    [SerializeField] private GameObject barkInstance;

    //Setting up singleton
    void Awake()
    {
        if(_bs != null && _bs != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Debug.Log("Initialized BarkSystem.");
            _bs = this;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void bark(GameObject parent, string text, float duration, Vector3 offset)
    {
        Bark newBark = Instantiate(barkInstance, parent.transform).GetComponent<Bark>();
        newBark.setDuration(duration);
        newBark.setText(text);
        newBark.transform.position += offset;
        Debug.Log($"Instancing new bark at {parent.transform.position}");
    }
}
