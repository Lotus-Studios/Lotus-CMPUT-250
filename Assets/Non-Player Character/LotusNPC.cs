using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class LotusNPC : MonoBehaviour
{
    private GameObject player = null;

    [SerializeField] private PlayerDrowning playerDrowning;
    [SerializeField] private Collider forcedDialogueCollider;
    [SerializeField] private SpriteRenderer sprite;
    public bool doneFirstDialogue = false;

    private Color CLEAR = new Color(1, 1, 1, 0);

    private Color defaultColor;
    [SerializeField] private TextAsset firstDialogue;

    [SerializeField] private List<string> BarkList;
    private int barkIndex = -1;
    private bool saidBarkThisLife = true;

    [SerializeField] private LotusGuruFloatingDummy dummy;
    [SerializeField] private GameObject dummyPrefab;
    // Start is called before the first frame update
    void Start()
    {
        //dummy = Instantiate(dummyPrefab, transform).GetComponent<LotusGuruFloatingDummy>();
        //dummy.setResetTransform(transform);
        defaultColor = sprite.color;
        
        player = GameObject.FindGameObjectWithTag("Player");
        
        if(player == null)
        {
            Debug.Log("NPC unable to find reference to player.");
        }
        hide();
        //doFirstDialogue();
        playerDrowning.playerDied.AddListener(setNewDeathBark);
        //BarkSystem.Instance.bark(gameObject, "This is a bark.", 30f, new Vector3(0, 1.25f, -0.25f));
    }

    // private void onTrigger(Collision collision)
    // {
    //     Debug.Log("123");
    //     if(collision.gameObject == player.gameObject && !doneFirstDialogue)
    //     {
    //         Debug.Log("Hit player");
    //     }
    // }
    void Update()
    {
        if (!saidBarkThisLife && Vector3.Distance(player.transform.position, transform.position) < 4)
        {
            afterDeathBark();
        }
    }
    private void afterDeathBark()
    {
        BarkSystem.Instance.bark(gameObject, BarkList[barkIndex], 6, new Vector3(0, 1.25f, -0.25f));
        saidBarkThisLife = true;
    }

    private void setNewDeathBark()
    {
        //Dont change barks if dying before hearing the first dielaogue
        if(!doneFirstDialogue) return;

        saidBarkThisLife = false;
        if(barkIndex < BarkList.Count - 1)
        {
            barkIndex++;
        }
    }
    public void doFirstDialogue()
    {
        if(doneFirstDialogue) return;

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

    void hide()
    {
        defaultColor = sprite.color;
        sprite.color = CLEAR;
    }

    void show()
    {
        sprite.color = defaultColor;
    }
}
