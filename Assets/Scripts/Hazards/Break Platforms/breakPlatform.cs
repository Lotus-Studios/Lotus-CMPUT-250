using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;



public class breakPlatform : MonoBehaviour
{
    [SerializeField] private float breakTime = 1f;
    [SerializeField] private float resetTime = 1f;
    [SerializeField] private bool permanentDestruction = false;

    [SerializeField] CrackBlockJiggler vfx;
    [SerializeField] private AudioClip breakBlockJiggleClip;
    [SerializeField] private AudioClip breakBlockShatterClip;
    [SerializeField] private float breakBlockVolume = 1f;

    private List<Collider> platformColliders = new List<Collider>();

    private Coroutine breakCoroutine;

    [HideInInspector]
    public bool broken;
    public bool IsPermanentDestruction => permanentDestruction;

    void Start()
    {
        // Get the collider that is NOT a trigger
        Collider[] colliders = GetComponents<Collider>();

        foreach (Collider col in colliders)
        {
            if (!col.isTrigger)
            {
                platformColliders.Add(col);
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
        foreach (Collider col in platformColliders)
        {
            col.enabled = false;
        }

        breakCoroutine = null;

        if(!broken) {

            if (AudioController.Instance != null) {
                AudioController.Instance.PlayPlatformSound(breakBlockShatterClip, transform.position, breakBlockVolume, breakBlockShatterClip.length);
            }

            broken = true;

            if (permanentDestruction)
            {
                breakCoroutine = null;
                yield break;
            }

            yield return new WaitForSeconds(resetTime);
            broken = false;

            vfx.Reform();
            vfx.JigglePercentInfluence = 0.0f;

            foreach (Collider col in platformColliders)
            {
                col.enabled = true;
            }
        }
       
        breakCoroutine = null;
    }
}
