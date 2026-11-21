using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
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
    [SerializeField] private TextAsset goodEndingPoolDialogue;
    [SerializeField] private TextAsset badEndingPoolDialogue;

    //Used to hide the player rudimentarily
    private SpriteRenderer playerSprite;
    private RisingWater water;

    [Header("Good Ending Assets")]
    
    [SerializeField] private GameObject lotusBloomAnimationHandler;
    [SerializeField] private GoodEndingCutsceneCamera goodCam;
    [SerializeField] private GameObject lotusling;
    [SerializeField] private Vector3 lotuslingCameraBillboardOffset;

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
            //pool.poolAlarm.AddListener(waterPoolLotus.doFirstDialogue);
            pool.poolAlarm.AddListener(endCutscene);
            //waterPoolLotus.finishedFirstDialogue.AddListener(endCutscene);
        }

        //Retrieving the players sprite renderer
        if (player)
        {
            var body = player.gameObject.transform.Find("Body");
            playerSprite = body.GetComponentInChildren<SpriteRenderer>();
        }
    }

    void LateUpdate()
    {
        if (lotusling)
        {
            Debug.Log("Before: " + lotusling.transform.localRotation);
            lotusling.transform.LookAt(goodCam.cam.transform);
            lotusling.transform.Rotate(lotuslingCameraBillboardOffset);
            Debug.Log("After: " + lotusling.transform.localRotation);

        }
        else
        {
            Debug.Log("Lotusling not set!");
        }
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
        var dia = (DialogueSystem.Instance.hasGoodEnding())? goodEndingPoolDialogue : badEndingPoolDialogue;
        GameManager.Instance.hideHUD();
        DialogueSystem.Instance.startDialogue(dia.ToString());
        DialogueSystem.Instance.dialogueFinished.AddListener(transition);

        void transition()
        {
            DialogueSystem.Instance.dialogueFinished.RemoveListener(transition);

            if (DialogueSystem.Instance.hasGoodEnding())
            {
                goodEndingCutscene();
            }
            else
            {
                badEndingCutscene();
            }
        }
    }

    private void goodEndingCutscene()
    {
        scrBlack.beginWaitOnBlack.AddListener(startCameraOrbit);
        scrBlack.BasicFade();
        if (AudioLayering.Instance != null) {
            AudioLayering.Instance.PlayGoodEndingMusic(0);
        }
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

        if (AudioLayering.Instance != null) {
            AudioLayering.Instance.PlayBadEndingMusic(0);
        }

        void startDrownAnim()
        {
            scrBlack.beginWaitOnBlack.RemoveListener(startDrownAnim);
            Debug.Log("Starting animation");

            player.canMove = false;
            playerSprite.enabled = false;

            player.ResetAll(false);
            if (water != null)
            {
                // tried with lerping and a huge number but that doesnt work as fade to black pauses the timer, so its instant now and this seems to work
                Vector3 currentPos = water.transform.position;
                water.transform.position = new Vector3(currentPos.x, player.startWaterLevel, currentPos.z);
                water.respawnLevel = player.startWaterLevel;

                // Forces UpdateSubmergedObjects() to run based on the new position
                water.ResetMe(player.startWaterLevel);
            }
            player.teleport(player.currentCheckpointPosition);
            drownAnim.activate();
            drownAnim.finishedAnimation.AddListener(onDrownFinished);
        }

        void onDrownFinished()
        {
            drownAnim.finishedAnimation.RemoveListener(onDrownFinished);

            scrBlack.beginWaitOnBlack.AddListener(swapToRespawnAnim);
            scrBlack.BasicFade();
        }

        void swapToRespawnAnim()
        {
            scrBlack.beginWaitOnBlack.RemoveListener(swapToRespawnAnim);

            drownAnim.deactivate();
            respawnAnim.activate();
            respawnAnim.finishedAnimation.AddListener(onRespawnFinished);
        }

        void onRespawnFinished()
        {
            respawnAnim.finishedAnimation.RemoveListener(onRespawnFinished);

            scrBlack.beginWaitOnBlack.AddListener(finishCutscene);
            scrBlack.BasicFade();
        }

        void finishCutscene()
        {
            scrBlack.beginWaitOnBlack.RemoveListener(finishCutscene);

            respawnAnim.deactivate();

            player.teleport(player.currentCheckpointPosition);

            playerSprite.enabled = true;
            player.canMove = true;
            GameManager.Instance.showHUD();
            GameManager.Instance.resetGame();
        }
    }

    private void transitionToCredits()
    {
        goodCam.deactivate();
        drownAnim.deactivate();
        playerSprite.enabled = true;
        player.canMove = true;
        GameManager.Instance.showHUD();
        GameManager.Instance.resetGame();
        GameManager.Instance.GoToMainMenu();
    }

    public bool inCutscene() { return _inCutscene; }
}
