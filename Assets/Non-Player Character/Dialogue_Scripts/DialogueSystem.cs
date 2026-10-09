using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Narrative;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.UI;
using UnityEngine.Events;

public class DialogueSystem : MonoBehaviour
{
    private static DialogueSystem _ds;

    private DialogueSystem() { }
    public static DialogueSystem Instance { get { return _ds; }}

    //Setting up singleton
    void Awake()
    {
        if(_ds != null && _ds != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Debug.Log("Initialized dialogueSystem.");
            _ds = this;
        }
    }
    //Is the dialogue UI visible?
    private bool isActive = false;
    //Are the choice buttons visible?
    private bool buttonsActive = false;

    private float portraitFadeInSpeed = 0.5f;
    private float portraitFadeOutSpeed = 0.5f;
    //Current chunk being displayed
    private int chunkIndex = 0;

    private int tallyScore = 0;
    //Current dialogue being worked through
    private Dialogue currentDialogue = null;
    #region References to Visual Components
    [SerializeField] private DialogueBox dialogueBox;
    [SerializeField] private DialoguePortrait portraitLeft;
    [SerializeField] private DialoguePortrait portraitRight;
    [SerializeField] private GameObject ChoiceButton; //This is a prefab instanced multiple times
    [SerializeField] private GameObject choiceButtonContainer;
    private OrbitCamera cameraOrbit;
    #endregion
    #region Data Containers
    //Used to make the dictionary below
    [SerializeField] private List<Sprite> rawSprites = new List<Sprite>();
    //portrait ID -> portrait Sprite
    [SerializeField] private Dictionary<string, Sprite> portraitIDSprite = new Dictionary<string, Sprite>();
    //Raw dialogues as TextAssets, used to make the dictionary below
    [SerializeField] private List<TextAsset> rawDialogues = new List<TextAsset>();
    //dialogueID -> Dialogue
    [SerializeField] private Dictionary<string, Dialogue> idDialogue = new Dictionary<string, Dialogue>();
    //SFXID -> Sound Effect
    [SerializeField] private List<AudioClip> rawAudio = new List<AudioClip>();
    [SerializeField] private Dictionary<string, AudioClip> idSFX = new Dictionary<string, AudioClip>();
    //TODO: Make SFX database after the sfx system has been implimented.
    #endregion
    
    #region Events
    public UnityEvent dialogueFinished;
    #endregion
    //Current line in a dialogue that we are at
    
    void Start()
    {
        //TODO: Make this less contrived, these should all just be grabbing the reference set in the editor.
        //Grab box for dialogue from scene by type
        dialogueBox = GameObject.FindAnyObjectByType<DialogueBox>();
        //Grab container for choice buttons from container
        choiceButtonContainer = GameObject.FindGameObjectWithTag("ChoiceButtonContainer");
        //add all dialogues to dictionary
        loadDialogues();
        //add all sprites to the dictionary
        loadSprites();

        loadAudio();

        cameraOrbit = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<OrbitCamera>();
    }

    //Starts a dialogue based in the input string
    public void startDialogue(string CSV)
    {
        if(currentDialogue != null) return;

        //Resetting everything
        //Could be function, but only used here.
        isActive = true;
        dialogueBox.ClearName();
        chunkIndex = -1;
        currentDialogue = DialogueParser.ParseDialogue(CSV);

        portraitLeft.EnterFade(portraitFadeInSpeed);
        portraitRight.EnterFade(portraitFadeInSpeed);

        progressDialogue();
        //Debug.Log($"Added dialogue with {currentDialogue.getText(chunkIndex)} as its first dialogue.");
        //Debug.Log($"Added dialogue with {currentDialogue.getText(chunkIndex + 1)} as its second dialogue.");
        
    }
    //Version with a Dialogue passed instead of a string to parse
    public void startDialogue(Dialogue dia)
    {
        //Debug.Log("Running start dialogue...");
        if(currentDialogue != null) return;
        isActive = true;
        dialogueBox.ClearName();
        chunkIndex = -1;
        currentDialogue = dia;

        portraitLeft.EnterFade(portraitFadeInSpeed);
        portraitRight.EnterFade(portraitFadeInSpeed);
        //Debug.Log($"Added dialogue with {currentDialogue.getText(chunkIndex)} as its first dialogue.");
        //Debug.Log($"Added dialogue with {currentDialogue.getText(chunkIndex + 1)} as its second dialogue.");
        progressDialogue();
    }

    private void updatePortraits()
    {
        portraitLeft.SetSprite(portraitIDSprite[currentDialogue.getPortraitLeftID(chunkIndex)]);
        portraitRight.SetSprite(portraitIDSprite[currentDialogue.getPortraitRightID(chunkIndex)]);
    }

    //Creates the choice buttons
    private void startChoiceScreen(Dialogue dlg)
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        //Create buttons for each option
        List<DialogueChoice> choices = dlg.choices;
        foreach(DialogueChoice choice in choices)
        {
            Debug.Log($"Creating button for {choice.buttonText}.");
            DialogueButton newChoiceButton = Instantiate(ChoiceButton, choiceButtonContainer.transform).GetComponent<DialogueButton>();
            newChoiceButton.setChoice(choice);
            //Adding listener to each button for unique choice
            newChoiceButton.GetComponent<Button>().onClick.AddListener(() => makeChoice(choice));
        }
    }
    //Called on button press, starts the next dialogue based on the passed in choice
    public void makeChoice(DialogueChoice choice)
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        Debug.Log($"Made choice {choice.buttonText}");

        tallyScore += choice.tallyValue;

        //Clear all old dialogue buttons
        foreach(Transform child in choiceButtonContainer.transform)
        {
            Destroy(child.gameObject);
        }

        //Initiate chosen Dialogue
        if (idDialogue.Keys.Contains(choice.nextDialogueID))
        {
            startDialogue(idDialogue[choice.nextDialogueID]);
        }
        else
        {
            Debug.Log($"Attempted to start dialogue with ID {choice.nextDialogueID} when no dialogue has such id.");
        }
        
    }

    public int getTallyScore() { return tallyScore; }

    //Input detection
    //TODO: Make the key changeable
    void Update()
    {
        updateDialogueBox();
        if (isActive)
        {
            if (Input.GetKeyDown(KeyCode.Space)) //TODO: Confirm if we r using space
            {
                progressDialogue();
            }
        }
    }

    //Updating the dialogue after every space press
    void progressDialogue()
    {
        //Reached the end of the current dialogue
        if (currentDialogue.isEndofDialogue(chunkIndex))
        {
            isActive = false;
            portraitLeft.ExitFade(0.5f);
            portraitRight.ExitFade(0.5f);
            //Start choices if we have any
            if (currentDialogue.hasChoices())
            {
                startChoiceScreen(currentDialogue);
            }
            else
            {
                if (cameraOrbit == null)
                {
                    cameraOrbit = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<OrbitCamera>();

                }

                if (cameraOrbit != null)
                {
                    cameraOrbit.ClearDialogueTarget();
                }

                PlayerController playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
                if (playerController != null)
                {
                    playerController.canMove = true;
                }
                dialogueFinished?.Invoke();
            }
            currentDialogue = null;

        }
        else //We aren't at the end of the current dialogue
        {
            //Move to next chunk and update
            chunkIndex++;
            setDialogueBox();
            
            
        }
    }
    //Updates the dialogue boxes open or closed state
    void updateDialogueBox()
    {
        if (!dialogueBox.IsOpen && isActive) dialogueBox.OpenTextbox();
        if(dialogueBox.IsOpen && !isActive) dialogueBox.CloseTextbox();
    }
    //sets the dialogue boxes line and name safely
    void setDialogueBox()
    {
        dialogueBox.SetLine(currentDialogue.getText(chunkIndex));
        dialogueBox.SetName(currentDialogue.getTextboxTitle(chunkIndex));
    }

    void setPortraitSprites()
    {
        if(portraitIDSprite.Keys.Contains<string>(currentDialogue.getPortraitLeftID(chunkIndex)))
        portraitLeft.SetSprite(portraitIDSprite[currentDialogue.getPortraitLeftID(chunkIndex)]);
        
        if(portraitIDSprite.Keys.Contains<string>(currentDialogue.getPortraitRightID(chunkIndex)))
        portraitRight.SetSprite(portraitIDSprite[currentDialogue.getPortraitRightID(chunkIndex)]);
    }

    void playAudio()
    {
        if (idSFX.Keys.Contains<string>(currentDialogue.getSoundID(chunkIndex)))
        {
            AudioController.Instance.PlayDialogueAudio(idSFX[currentDialogue.getSoundID(chunkIndex)]);
            Debug.Log($"Added {currentDialogue.getSoundID(chunkIndex)}");
        }
    }

    //Adds all sprites to the dictionary with their keys being their names in files
    void loadSprites()
    {
        foreach(Sprite spr in rawSprites)
        {
            portraitIDSprite[spr.name] = spr;
            Debug.Log($"Added {spr.name} to spriteID.");
        }
        portraitIDSprite["EMPTY"] = null;
    }

    void loadAudio()
    {
        foreach(AudioClip audio in rawAudio)
        {
            idSFX[audio.name] = audio;
            Debug.Log($"Added {audio.name} to spriteID.");
        }
        portraitIDSprite["EMPTY"] = null;
    }

    void loadDialogues()
    {
      foreach(TextAsset rawTxt in rawDialogues)
        {
            Dialogue newDia = DialogueParser.ParseDialogue(rawTxt.ToString());
            idDialogue[newDia.getID()] = newDia;
        }  
    }
}
