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
    [SerializeField] private TextAsset tallyThresholdDialogue;

    [SerializeField] private int tallyThresholdValue = 0;

    [SerializeField] private List<string> BarkList;
    private int barkIndex = -1;
    private bool saidBarkThisLife = true;
    [SerializeField] RisingWater water;
    private LotusGuruFloatingDummy dummy;
    [SerializeField] private GameObject dummyPrefab;
    // Start is called before the first frame update
    void Start()
    {
        //Setting up dummy
        dummy = Instantiate(dummyPrefab, transform).GetComponent<LotusGuruFloatingDummy>();
        dummy.water = water;
        dummy.setResetTransform(transform);

        defaultColor = sprite.color;
        //Getting reference to player.
        foreach(GameObject pl in GameObject.FindGameObjectsWithTag("Player"))
        {
            if(pl.GetComponent<PlayerController>() != null)
            {
                player = pl;
            }
        }
        
        
        if(player == null)
        {
            Debug.Log("NPC unable to find reference to player.");
        }
        hide();
        playerDrowning.playerDied.AddListener(setNewDeathBark);
    }
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

        if(DialogueSystem.Instance.getTallyScore() >= tallyThresholdValue && tallyThresholdValue != 0)
        {
            DialogueSystem.Instance.startDialogue(tallyThresholdDialogue.ToString());
        }
        else
        {
            DialogueSystem.Instance.startDialogue(firstDialogue.ToString());
        }
        
        doneFirstDialogue = true;

        //Manage re enabling player movement
        void reenablePlayerMovement()
        { 
            playerController.canMove = true;
            DialogueSystem.Instance.dialogueFinished.RemoveListener(reenablePlayerMovement);
        }

        DialogueSystem.Instance.dialogueFinished.AddListener(reenablePlayerMovement);
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

    public void resetDoneFirstDialogue()
    {
        doneFirstDialogue = false;
    }
    public LotusGuruFloatingDummy getDummy()
    {
        return dummy;
    }
}
