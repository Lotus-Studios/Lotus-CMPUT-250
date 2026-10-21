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

    private PlayerController player = null;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
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
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        //Show game menu only if not in main menu
        if(Keyboard.current.escapeKey.wasPressedThisFrame && !mainMenu.gameObject.activeSelf && !DialogueSystem.Instance.getIsActive())
        {
            if (gameMenu.gameObject.activeSelf)
            {
                setCursor(false);
                setCursorLockState(CursorLockMode.Locked);
                hideGameMenu();
            }
            else
            {
                setCursor(true);
                setCursorLockState(CursorLockMode.None);
                showGameMenu();
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

    private void hideMainMenu()
    {
        mainMenu.gameObject.SetActive(false);
    }
    private void showMainMenu()
    {
        mainMenu.gameObject.SetActive(true);
    }
    private void hideGameMenu()
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
