using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OingyBoingy : MonoBehaviour
{
    public float boinginess = 10.0f;
    [SerializeField] Animation bounceAnim;

    public void doBounceAnim()
    {
        bounceAnim.Play();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
