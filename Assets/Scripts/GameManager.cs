using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
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
        
    }

    public void GoToGame()
    {
        ScreenFadeToBlack.Instance.BasicFade();
        ScreenFadeToBlack.Instance.beginWaitOnBlack.AddListener(() =>
        {
            hideMainMenu();
            setCursor(false);
            setCursorLockState(CursorLockMode.Locked);
        });
        ScreenFadeToBlack.Instance.endBasicFade.AddListener(() =>
        {
            if(player != null) player.canMove = true;
            GameStart.Invoke();
        });
    }

    public void GoToMainMenu()
    {
        ScreenFadeToBlack.Instance.BasicFade();

        if(player != null) player.canMove = false;
        
        ScreenFadeToBlack.Instance.beginWaitOnBlack.AddListener(() =>
        {
            showMainMenu();
            setCursor(true);
            setCursorLockState(CursorLockMode.None);
        });
        ScreenFadeToBlack.Instance.endBasicFade.AddListener(() =>
        {
            
            //Rising water should be stopped here, haven't done that yet.
            MainMenuStart.Invoke();
        });
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
    private void showGamenMenu()
    {
        gameMenu.gameObject.SetActive(true);
    }
    public void setCursor(bool State) { Cursor.visible = State; }
    public void setCursorLockState(UnityEngine.CursorLockMode State) { Cursor.lockState = State; }
}
