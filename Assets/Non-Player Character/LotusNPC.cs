using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LotusNPC : MonoBehaviour
{
    private GameObject player = null;
    [SerializeField] private Collider forcedDialogueCollider;

    public bool doneFirstDialogue = false;

    [SerializeField] private TextAsset firstDialogue;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        if(player == null)
        {
            Debug.Log("NPC unable to find reference to player.");
        }
        doFirstDialogue();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("123");
        if(collision.gameObject == player)
        {
            Debug.Log("Hit player");
        }
    }

    void doFirstDialogue()
    {
        DialogueSystem.Instance.startDialogue(firstDialogue.ToString());
        doneFirstDialogue = true;
        OrbitCamera cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<OrbitCamera>();
        cam.SetDialogueTarget(transform);
    }
}
