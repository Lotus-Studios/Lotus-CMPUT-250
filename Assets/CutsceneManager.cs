using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Playables;
using UnityEngine.Splines;

public class CutsceneManager : MonoBehaviour
{
    private static CutsceneManager _instance;
    public static CutsceneManager Instance { get { return _instance; } }
    private bool _inCutscene = false;
    
    [Header("Both Ending Assets")]
    [SerializeField] private PlayerController player;
    [SerializeField] private ScreenFadeToBlack scrBlack = ScreenFadeToBlack.Instance;
    [SerializeField] private LotusNPC waterPoolLotus;
    //[SerializeField] private Vector3 playerEndCutsceneStartPosition;
    [SerializeField] private TextAsset onPoolEnterDialogue;
    [SerializeField] private TextAsset badEndingDialogue;
    [SerializeField] private EndingPool pool;

    //Used to hide the player rudimentarily
    private SpriteRenderer playerSprite;

    [Header("Good Ending Assets")]
    
    [SerializeField] private GameObject lotusBloomAnimationHandler;
    [SerializeField] private GoodEndingCutsceneCamera goodCam;

    [Header("Bad Ending Assets")]
    [SerializeField] private LotusBadEndingRespawnAnimationHandler respawnAnim;
    [SerializeField] private LotusDrownAnimationHandler drownAnim;

    
    // Start is called before the first frame update
    void Start()
    {
        //Initialize singleton
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
            return;
        } 
        _instance = this;
        //Disable the bad ending respawn camera.
        //badEndRespawnCamera.enabled = false;

        //Making lotus do his final dialogue after the player has been in the pool for a bit.
        //Also linking the end cutscene to the lotus NPC finishing his dialogue.
        if (pool)
        {
            pool.poolAlarm.AddListener(waterPoolLotus.doFirstDialogue);
            waterPoolLotus.finishedFirstDialogue.AddListener(endCutscene);
        }

        //Retrieving the players sprite renderer
        if (player)
        {
            var body = player.gameObject.transform.Find("Body");
            playerSprite = body.GetComponentInChildren<SpriteRenderer>();
        }
    }

    void Update()
    {

    }
    //Ending cutscene
    /*
    Events:
    1. Fade to black
    2. Play dialogue1
    3. Play separate animations based on tally.
    4a. Play good ending animation -> go to credits.
    4b. Play bad ending animation -> play seedlin back to beginning animation.*/
    private void endCutscene()
    {
        if (DialogueSystem.Instance.hasGoodEnding())
        {
            goodEndingCutscene();
        }
        else
        {
            badEndingCutscene();
        }
    }

    private void goodEndingCutscene()
    {
        scrBlack.beginWaitOnBlack.AddListener(startCameraOrbit);
        scrBlack.BasicFade();
        void startCameraOrbit()
        {
            player.canMove = false;
            playerSprite.enabled = false;
            goodCam.activate();
            scrBlack.beginWaitOnBlack.RemoveListener(startCameraOrbit);
            lotusBloomAnimationHandler.SetActive(true);
            lotusBloomAnimationHandler.GetComponentInChildren<EndingTriggerZone>().finishedAnimation.AddListener(transitionToCredits);
        }
    }

    private void badEndingCutscene()
    {
        scrBlack.beginWaitOnBlack.AddListener(startDrownAnim);
        scrBlack.BasicFade();
        void startDrownAnim()
        {
            Debug.Log("Starting animation");
            drownAnim.activate();
            player.canMove = false;
            playerSprite.enabled = false;
            scrBlack.beginWaitOnBlack.RemoveListener(startDrownAnim);

            drownAnim.finishedAnimation.AddListener(() => {player.ResetAll(true);});
            scrBlack.beginWaitOnBlack.AddListener(respawnAtStart);
        }
        void respawnAtStart()
        {
            respawnAnim.activate();
            respawnAnim.finishedAnimation.AddListener(() => {
                player.ResetToCurrentCheckpoint();
                respawnAnim.deactivate();
                drownAnim.deactivate();
                player.canMove = true;
                playerSprite.enabled = true;
        });
            scrBlack.beginWaitOnBlack.RemoveListener(respawnAtStart);
        }
    }

    private void transitionToCredits()
    {
        goodCam.deactivate();
        drownAnim.deactivate();
        GameManager.Instance.GoToMainMenu();
    }

    public bool inCutscene() { return _inCutscene; }
}
