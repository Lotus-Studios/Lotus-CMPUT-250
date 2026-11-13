using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public class Collectible : MonoBehaviour
{
    [Header("Following Settings")]
    [SerializeField] private float followDistance = 0.8f;
    [SerializeField] private float followSpeed = 6f;

    private bool isCollected = false;
    private Transform followTarget;
    private static Collectible lastCollected;
    private Vector3 originalPosition;

    private bool isBanked = false;
    private bool isBanking = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        // Only respond to the player
        if (!other.CompareTag("Player") &&
            !other.transform.root.CompareTag("Player"))
            return;

        Collect(other.transform);
    }
    private void Collect(Transform player)
    {
        isCollected = true;

        // Turn off the collider when collected
        GetComponent<Collider>().enabled = false;

        if (lastCollected == null)
        {
            followTarget = player;
        }
        else
        {
            followTarget = lastCollected.transform;
        }

        lastCollected = this;

    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (!isCollected || followTarget == null || isBanking)
            return;

        Vector3 direction = followTarget.position - transform.position; //direction towards the player

        if (direction.magnitude <= followDistance)return; //Don't move if it's already close enough

        Vector3 targetPosition = followTarget.position - direction.normalized * followDistance; //Calculate a position at the desired distance

        transform.position = Vector3.Lerp(transform.position,targetPosition,followSpeed * Time.deltaTime);
    }
    private void Awake()
    {
        originalPosition = transform.position; //So that collectibles can be reset after player dies
    }

    public void ResetCollectible()
    {
        if (!isCollected) //If not collected, no need to reset
            return;
        StopAllCoroutines();//Stop any ongoing animations
        isBanking = false;

        isCollected = false;

        followTarget = null;

        transform.position = originalPosition;

        GetComponent<Collider>().enabled = true; //Turn collider back on
    }

    public static void ResetAllCollectibles()
    {
        Collectible[] collectibles = FindObjectsByType<Collectible>(FindObjectsSortMode.None); //Grab all objects with a collectible component

        foreach (Collectible collectible in collectibles)
        {
            collectible.ResetCollectible(); //Reset every collectible
        }

        lastCollected = null;
    }

    public bool BankCollectible()
    {
        if (!isCollected || isBanked) return false;

        isBanked = true;
        isCollected = false;
        followTarget = null;

        gameObject.SetActive(false);

        return true;
    }

    public void StartBanking(PlayerController player, float delay = 0f) //Start the banking coroutine
    {
        if (!isCollected || isBanked || isBanking || player == null)
            return;

        StartCoroutine(BankAnimation(player, delay));
    }


    private IEnumerator BankAnimation(PlayerController player, float delay = 0f) //Animation that plays when the player begins banking collectibles
    {
        if (!isCollected || isBanked || isBanking || player == null)
            yield break;

        isBanking = true;

        if (delay > 0f)
        {
            //This section makes it so that the animations start at different times for each collectible
            yield return new WaitForSeconds(delay);

            if (!isCollected || isBanked || player == null)
            {
                isBanking = false;
                yield break;
            }
        }

        Vector3 startPosition = transform.position;
        float duration = 1f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            // Cancel banking if the player dies and resets the collectible.
            if (!isCollected || player == null)
            {
                isBanking = false;
                yield break;
            }

            elapsed += Time.deltaTime;

            float t = Mathf.Clamp01(elapsed / duration); //Calculate animation progress
            t = t * t; //Square it so that it starts slow and speds up

            transform.position = Vector3.Lerp(startPosition,player.transform.position,t);

            yield return null;
        }

        if (BankCollectible())
            player.AddBankedCollectibles(1);

        isBanking = false;
    }

    public static void BankAllCollectibles(PlayerController player)
    {

        List<Collectible> chain = new List<Collectible>();

        
        Collectible current = lastCollected;// Move backwards through currently held collectibles

        while (current != null)
        {
            chain.Add(current);

            if (current.followTarget != null) //Is the current object following something?
            {
                current = current.followTarget.GetComponent<Collectible>(); //Try and grab the collectible component
            }
            else
            {
                current = null;
            }
        }

        
        chain.Reverse(); // Reverse to get the original collection order, probably could have used a stack here but I'm lazy

        // Start each banking animation with a staggered delay
        for (int i = 0; i < chain.Count; i++)
        {
            chain[i].StartBanking(player, i * 0.1f);
        }

        lastCollected = null;
    }





}
