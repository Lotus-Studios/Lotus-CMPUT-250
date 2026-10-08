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

    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("123");
        if(collision.gameObject == player && !doneFirstDialogue)
        {
            
            Debug.Log("Hit player");
        }
    }

    void doFirstDialogue()
    {
        OrbitCamera cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<OrbitCamera>();
        cam.SetDialogueTarget(transform);

        PlayerController playerController = player.GetComponent<PlayerController>();
        if (playerController != null)
        {
            playerController.canMove = false;
        }

        DialogueSystem.Instance.startDialogue(firstDialogue.ToString());
        doneFirstDialogue = true;
    }
}
