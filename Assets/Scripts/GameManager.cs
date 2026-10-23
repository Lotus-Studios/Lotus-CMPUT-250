using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager Instance { get { return _instance; } }
    public UnityEvent MainMenuToGame;
    public UnityEvent GameStart;
    public UnityEvent MainMenuStart;

    //Skip having the main menu on game start for debuggin purposes
    [SerializeField] private bool mainMenuBypass = false;
    [SerializeField] private MainMenu mainMenu;
    [SerializeField] private GameMenu gameMenu;

    private PlayerController player;
    // Start is called before the first frame update

    void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
    }

    void Start()
    {
        foreach (GameObject pl in GameObject.FindGameObjectsWithTag("Player"))
        {
             if(pl.GetComponent<PlayerController>() != null)
            {
                player = pl.GetComponent<PlayerController>();
            }
        }
       
        if(player != null)
        {
            if (mainMenuBypass)
            {
                player.canMove = true;
                setCursor(false);
                setCursorLockState(CursorLockMode.Locked);
                hideMainMenu();
            }
            else
            {
                player.canMove = false;
                showMainMenu();
            }
        }
        else
        {
            Debug.Log($"Game manager couldn't find playerController! Found player as {GameObject.FindGameObjectWithTag("Player").name}");
        }
    }

    public void resetGame()
    {
        //Wait for the player to teleport before resetting the NPC floaters
        UnityEvent listenedEvent = player.playerTeleported;

        void resetNPCs()
        {
            foreach (GameObject npc in GameObject.FindGameObjectsWithTag("NPC"))
            {
                LotusNPC lotus = npc.GetComponent<LotusNPC>();
                
                lotus.resetDoneFirstDialogue();
                lotus.getDummy().ResetMe();
            }
            //Self unsubscribe
            listenedEvent.RemoveListener(resetNPCs);
        }

        player.ResetAll();
        //Add the npc reset to listen to the player teleport
        listenedEvent.AddListener(resetNPCs);
        DialogueSystem.Instance.resetTallyScore();
    }


    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current == null) return;
        if (mainMenu == null || gameMenu == null) return;

        bool tabPressed = Keyboard.current.tabKey.wasPressedThisFrame;
        bool isDialogueActive = DialogueSystem.Instance != null && DialogueSystem.Instance.getIsActive();

        if (tabPressed && !mainMenu.gameObject.activeSelf && !isDialogueActive)
        {
            if (gameMenu.gameObject.activeSelf)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    public void GoToGame()
    {
        Debug.Log("Beginning GoToGame");
        //Reference to fade class
        ScreenFadeToBlack fade = ScreenFadeToBlack.Instance;

        //behaviour when the screen is fully black
        void onBlack()
        {   
            hideMainMenu();
            setCursor(false);
            setCursorLockState(CursorLockMode.Locked);
            fade.beginWaitOnBlack.RemoveListener(onBlack); //self unsubcribe
        }
        //Behaviour when the animation is complete
        void onEnd()
        {
            if(player != null) player.canMove = true;
            GameStart.Invoke();
            
            fade.endBasicFade.RemoveListener(onEnd); //self unsubcribe
        }
        //Add both as listeners to their respective events in ScreenFadeToBlack
        ScreenFadeToBlack.Instance.beginWaitOnBlack.AddListener(onBlack);
        ScreenFadeToBlack.Instance.endBasicFade.AddListener(onEnd);
        //Call the function
        fade.BasicFade();
    }

    public void GoToMainMenu()
    {
        if(DialogueSystem.Instance.getIsActive()) return;
        Debug.Log("GoToMainMenu starting...");

        Time.timeScale = 1f;

        ScreenFadeToBlack fade = ScreenFadeToBlack.Instance;

        if(player != null) player.canMove = false;
        
        void onBlack()
        {
            hideGameMenu();
            showMainMenu();
            setCursor(true);
            setCursorLockState(CursorLockMode.None);
            fade.beginWaitOnBlack.RemoveListener(onBlack);
        }

        void onEnd()
        {
            fade.endBasicFade.RemoveListener(onEnd);
            MainMenuStart.Invoke();
        }

        fade.beginWaitOnBlack.AddListener(onBlack);
        fade.endBasicFade.AddListener(onEnd);

        player.ResetToCurrentCheckpoint();
    }

    public void PauseGame()
    {
        showGameMenu();
        setCursor(true);
        setCursorLockState(CursorLockMode.None);
        //Time.timeScale = 0f;
    }

    public void ResumeGame()
    {
        hideGameMenu();
        setCursor(false);
        setCursorLockState(CursorLockMode.Locked);
        //Time.timeScale = 1f;
    }

    private void hideMainMenu()
    {
        mainMenu.gameObject.SetActive(false);
    }
    private void showMainMenu()
    {
        mainMenu.gameObject.SetActive(true);
    }
    public void hideGameMenu()
    {
        gameMenu.gameObject.SetActive(false);
    }
    private void showGameMenu()
    {
        gameMenu.gameObject.SetActive(true);
    }
    public void setCursor(bool State) { Cursor.visible = State; }
    public void setCursorLockState(UnityEngine.CursorLockMode State) { Cursor.lockState = State; }
}
