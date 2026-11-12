using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;



public class breakPlatform : MonoBehaviour
{
    [SerializeField] private float breakTime = 1f;
    [SerializeField] private float resetTime = 1f;

    [SerializeField] CrackBlockJiggler vfx;
    [SerializeField] private AudioClip breakBlockJiggleClip;
    [SerializeField] private AudioClip breakBlockShatterClip;
    [SerializeField] private float breakBlockVolume = 1f;

    private Collider platformCollider;

    private Coroutine breakCoroutine;

    [HideInInspector]
    public bool broken;


    void Start()
    {


        // Get the collider that is NOT a trigger
        Collider[] colliders = GetComponents<Collider>();

        foreach (Collider col in colliders)
        {
            if (!col.isTrigger)
            {
                platformCollider = col;
                break;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && breakCoroutine == null)
        {   
            if(!broken){
                breakCoroutine = StartCoroutine(BreakPlatform());
                if (AudioController.Instance != null) {
                    AudioController.Instance.PlayPlatformSound(breakBlockJiggleClip, transform.position, breakBlockVolume, breakTime);
                }
            }
        }
    }

    

    private IEnumerator BreakPlatform()
    {

        vfx.JigglePercentInfluence = 1.0f;
        yield return new WaitForSeconds(breakTime);

        vfx.Break();
        platformCollider.enabled = false;

        breakCoroutine = null;

        if(!broken) {

            if (AudioController.Instance != null) {
                AudioController.Instance.PlayPlatformSound(breakBlockShatterClip, transform.position, breakBlockVolume, breakBlockShatterClip.length);
            }

            broken = true;
            yield return new WaitForSeconds(resetTime);
            broken = false;

            vfx.Reform();
            vfx.JigglePercentInfluence = 0.0f;

            platformCollider.enabled = true;
        }

    }
}
