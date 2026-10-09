using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class breakPlatform : MonoBehaviour
{
    [SerializeField] private float breakTime = 1f;
    [SerializeField] private float resetTime = 1f;

    [SerializeField] CrackBlockJiggler vfx;
    [SerializeField] private AudioClip breakBlockJiggleClip;
    [SerializeField] private AudioClip breakBlockShatterClip;

    private Collider platformCollider;

    private Coroutine breakCoroutine;


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
            breakCoroutine = StartCoroutine(BreakPlatform());
            AudioController.Instance.PlayPlatformSound(breakBlockJiggleClip, transform.position, 1f, breakTime);
        }
    }

    

    private IEnumerator BreakPlatform()
    {
        vfx.JigglePercentInfluence = 1.0f;
        yield return new WaitForSeconds(breakTime);

        vfx.Break();
        platformCollider.enabled = false;

        breakCoroutine = null;

        AudioController.Instance.PlayPlatformSound(breakBlockShatterClip, transform.position, 1f, breakBlockShatterClip.length);

        yield return new WaitForSeconds(resetTime);

        vfx.Reform();
        vfx.JigglePercentInfluence = 0.0f;

        platformCollider.enabled = true;

    }
}
