using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CrackBlockJiggler : MonoBehaviour
{
    [Header("Per piece")]
    [SerializeField] BrokenPlatformVisualFragment[] crackPieces;

    [SerializeField] Vector3 pieceDirectionalAmplitudes = new Vector3(0.05f, 0.05f, 0.05f);
    [SerializeField] float pieceNoiseFrequency = 10f;
    //set on start
    float _jigglePercentInfluence = 0.0f;
    //Clamps from 0-100% range (0.0 to 1.0f)
    public float JigglePercentInfluence { get { return _jigglePercentInfluence; } set { _jigglePercentInfluence = Mathf.Clamp(value, 0.0f, 1.0f); } }

    [SerializeField]
    breakPlatform platform;

    [Header("Per entire block")]
    Vector3 blockRestPoint;
    [SerializeField] float blockNoiseFrequency = 5f;
    [SerializeField] Vector3 blockNoiseAmplitudes = new Vector3(0.1f, 0.05f, 0.1f);


    [SerializeField] float outwardSplitForce = 2.0f;
    [SerializeField] float forwardSplitForce = 4.0f;
    [SerializeField] float maxSplitTorque = 50f;

    [SerializeField] ParticleSystem particles;

    [Header("Debris")]
    [SerializeField] private bool enableShrink = false;
    [SerializeField] private float shrinkDelay = 3f;
    [SerializeField] private float shrinkDuration = 3f;
    private Coroutine shrinkCoroutine;

    // Start is called before the first frame update
    void Start()
    {
        Vector3[] restPoints = new Vector3[crackPieces.Length]; 
        for(int i = 0; i < crackPieces.Length; i++)
        {
            crackPieces[i].restPosition = crackPieces[i].transform.position;
            crackPieces[i].restRotation = crackPieces[i].transform.rotation;
            crackPieces[i].GetComponent<Collider>().enabled = false;
            
            crackPieces[i].Rigidbody.isKinematic = true;
        }

        blockRestPoint = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        
        if(JigglePercentInfluence == 0.0 || platform.broken) return;
        if(!particles.isPlaying) particles.Play();
        for(int i = 0; i < crackPieces.Length; i++)
        {
            float pieceNoiseValue = Mathf.PerlinNoise(
                crackPieces[i].transform.position.x + Time.time*pieceNoiseFrequency, 
                crackPieces[i].transform.position.z +Time.time*pieceNoiseFrequency);
            crackPieces[i].transform.position = crackPieces[i].restPosition + pieceDirectionalAmplitudes * pieceNoiseValue * JigglePercentInfluence;
        }


        float noiseValue = Mathf.PerlinNoise(
                transform.position.x + Time.time*blockNoiseFrequency, 
                transform.position.z +Time.time*blockNoiseFrequency);
        transform.position = blockRestPoint + blockNoiseAmplitudes * noiseValue * JigglePercentInfluence;

    }

    public void Break()
    {
        particles.Stop();
        for(int i = 0; i < crackPieces.Length; i++)
        {
            crackPieces[i].Rigidbody.isKinematic = false;
            crackPieces[i].GetComponent<Collider>().enabled = true;
            // Vector3 forceDir = -(transform.position - crackPieces[i].transform.position).normalized;
            // crackPieces[i].Rigidbody.AddForce(forceDir*randomSplitForce + -transform.right * forwardSplitForce);
            crackPieces[i].Rigidbody.AddForce(-transform.right * forwardSplitForce, ForceMode.Impulse);

            crackPieces[i].Rigidbody.AddTorque(new Vector3(
                UnityEngine.Random.Range(-maxSplitTorque, maxSplitTorque), 
            UnityEngine.Random.Range(-maxSplitTorque, maxSplitTorque), 
            UnityEngine.Random.Range(-maxSplitTorque, maxSplitTorque))
            );

        }

        if (enableShrink) 
        {
            if (shrinkCoroutine != null) StopCoroutine(shrinkCoroutine);
            shrinkCoroutine = StartCoroutine(fadeBlocks());
        }
        
    }

    private IEnumerator fadeBlocks()
    {
        yield return new WaitForSeconds(shrinkDelay);

        if (enableShrink)
        {
            // disable collider
            for (int i = 0; i < crackPieces.Length; i++)
            {
                crackPieces[i].GetComponent<Collider>().enabled = false;
                crackPieces[i].Rigidbody.isKinematic = true;
            }

            float shrinkTimer = 0f;
            while (shrinkTimer < shrinkDuration)
            {
                shrinkTimer += Time.deltaTime;
                float t = shrinkTimer / shrinkDuration; // percentage of time

                for (int i = 0; i < crackPieces.Length; i++)
                {
                    crackPieces[i].transform.localScale = Vector3.Lerp(crackPieces[i].restScale, Vector3.zero, t); // lerp restScale to 0 at rate of t
                }

                yield return null;
            }

            for (int i = 0; i < crackPieces.Length; i++)
            {
                if (crackPieces[i].Renderer != null)
                {
                    crackPieces[i].Renderer.enabled = false;
                }
            }
        }

        shrinkCoroutine = null;
    }

    public void Reform()
    {
        if (shrinkCoroutine != null && enableShrink) 
        {
            StopCoroutine(shrinkCoroutine);
            shrinkCoroutine = null;
        }

        for(int i = 0; i < crackPieces.Length; i++)
        {
            crackPieces[i].Rigidbody.isKinematic = true;
            crackPieces[i].GetComponent<Collider>().enabled = false;
            crackPieces[i].Rigidbody.position = crackPieces[i].restPosition;
            crackPieces[i].Rigidbody.rotation = crackPieces[i].restRotation;
            crackPieces[i].ResetScale();
        }
    }
}
