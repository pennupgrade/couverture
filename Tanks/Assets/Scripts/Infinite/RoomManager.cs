using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RoomManager : MonoBehaviour
{
    public static RoomManager Instance { get; private set;}
    public static int LevelNum { get; private set;}
    bool loading;
    private ClassicUIManager uiManager;
    private AudioManager audioManager;
    [SerializeField] int overrideLevel;


    //call at start
    public static void reset() {
        LevelNum = 1;
    }
    //call when exiting
    public static void destroyInstance() {
        Destroy(Instance);
        Instance = null;
    }
    void Awake()
    {
        // start debug save if required
        SaveStateManagerGameObject.DebugLoadSave();

        // load level save stuff
        SaveStateManagerGameObject.LoadLevel(SceneManager.GetActiveScene().name);
        if (overrideLevel > 0) {
            LevelNum = overrideLevel;
            overrideLevel = -1;
        }
        if (Instance == null) {
            Instance = this;
            uiManager = GetComponent<ClassicUIManager>();
            audioManager = GetComponent<AudioManager>();
            startScreen();
        } else if (Instance != this) {
            Instance.startScreen();
            Destroy(this.gameObject);
        }
        DontDestroyOnLoad(this);
    }

    public void startScreen() {
        //play opening sound effect
        StartCoroutine(startScreenCoroutine());
    }
    private IEnumerator startScreenCoroutine() {
        uiManager.reset();
        Tank pTank = Tank.FindPlayer();
        pTank.FreezeNoRotation();
        
        //fade in level number text
        uiManager.StartScreenTextFadeIn(LevelNum);
        yield return new WaitForSeconds(0.5f);
        audioManager.Play("StartSound");
        yield return new WaitForSeconds(1.5f);


        // remove start screen, play BG music
        audioManager.Play("BGM");
        uiManager.StartScreenFadeOut();
        yield return new WaitForSeconds(0.2f);
        pTank.UnfreezeNoRotation();

    } 

    public void roomTransition() {
        if (loading) return;

        SaveStateManagerGameObject.UpdateClassicModeHighScore(LevelNum);

        LevelNum++;
        Debug.Log("Loading Level" + LevelNum);

        //scene transition
        if (LevelNum == 1) {
            StartCoroutine(LoadAsyncScene("Classic1"));
        } else if (LevelNum == 2 || LevelNum == 18 || LevelNum == 36) {
            StartCoroutine(LoadAsyncScene("Classic2"));
        } else if (LevelNum == 3 || LevelNum == 20 || LevelNum == 23) {
            StartCoroutine(LoadAsyncScene("Classic3"));
        } else if (LevelNum == 4 || LevelNum == 17 || LevelNum == 39) {
            StartCoroutine(LoadAsyncScene("Classic4"));
        } else if (LevelNum == 5 || LevelNum == 19) {
            StartCoroutine(LoadAsyncScene("Classic5"));
        } else if (LevelNum == 6 || LevelNum == 35 || LevelNum == 40) {
            StartCoroutine(LoadAsyncScene("Classic6"));
        } else if (LevelNum == 10 || LevelNum == 60) {
            StartCoroutine(LoadAsyncScene("Classic10"));
        } else if (LevelNum == 16 || LevelNum == 37) {
            StartCoroutine(LoadAsyncScene("Classic16"));
        } else if (LevelNum == 21 || LevelNum == 41) {
            StartCoroutine(LoadAsyncScene("Classic21"));
        } else if (LevelNum == 22 || LevelNum == 42) {
            StartCoroutine(LoadAsyncScene("Classic22"));
        } else if (LevelNum == 30 || LevelNum == 38 || LevelNum == 50) {
            if ((LevelNum == 30 && Tank.FindPlayer().CharacterHasAbility()) || LevelNum == 50) {
                StartCoroutine(LoadAsyncScene("Classic30alt"));
            } else {
                StartCoroutine(LoadAsyncScene("Classic30"));
            }

        } else if (LevelNum == 43) {
            StartCoroutine(LoadAsyncScene("Classic43"));
        } else if (LevelNum == 55) {
            StartCoroutine(LoadAsyncScene("Classic55"));
        } else {
            if (LevelNum == 61) {
                //game end screen, displays time taken, button leads to main menu
                uiManager.LevelCompleteScreenFadeIn(true);
                //classic mode complete sound effect
                destroyInstance();
                EnemySpawner.reset(); // resets enemy counter
                audioManager.Stop("BGM");
                audioManager.Play("WinSound");
                
            } else {
                //randomized level
                /*
                int r = Random.Range(1, 12);
                if (r == 1) {
                    StartCoroutine(LoadAsyncScene("ClassicA"));
                } else if (r == 2) {
                    StartCoroutine(LoadAsyncScene("ClassicB"));
                } else if (r == 3) {
                    StartCoroutine(LoadAsyncScene("ClassicC"));
                } else if (r == 4) {
                    StartCoroutine(LoadAsyncScene("ClassicD"));
                } else if (r == 5) {
                    StartCoroutine(LoadAsyncScene("ClassicE"));
                } else if (r == 6) {
                    StartCoroutine(LoadAsyncScene("ClassicF"));
                } else if (r == 7) 
                    StartCoroutine(LoadAsyncScene("ClassicG"));
                } else if (r == 8) {
                    StartCoroutine(LoadAsyncScene("ClassicH"));
                } else if (r == 9) {
                    StartCoroutine(LoadAsyncScene("ClassicI"));
                } else if (r == 10) {
                    StartCoroutine(LoadAsyncScene("ClassicJ"));
                } else {
                    StartCoroutine(LoadAsyncScene("ClassicMaze"));
                }
                */
            }
        }
    }
    public void playerDeath() {
        // redirect to death screen showing level reached, button leads to main menu
        uiManager.DeathScreenFadeIn(LevelNum);
        //play sad sound
        audioManager.Stop("BGM");
        audioManager.Play("DeathSound");

        // call exit level
        SaveStateManagerGameObject.ExitLevel();
        SaveStateManagerGameObject.SaveToFile();

        destroyInstance();
        EnemySpawner.reset(); // resets enemy counter
    }

    IEnumerator LoadAsyncScene(string sceneName) {
        yield return new WaitForSeconds(1f);
        //level complete screen
        uiManager.LevelCompleteScreenFadeIn(false);
        Tank.FindPlayer().FreezeRotationAllowed();
        //play level complete sound
        audioManager.Stop("BGM");
        audioManager.Play("CompleteSound");
        yield return new WaitForSeconds(1.8f);
        //level complete screen fades out
        uiManager.LevelCompleteScreenTextFadeOut();
        yield return new WaitForSeconds(1f);

        SaveStateManagerGameObject.FinishLevel(sceneName, false);
        // reset stats
        SaveStateManagerGameObject.PlayerDied();
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        loading = true;

        while (!asyncLoad.isDone)
        {
            yield return null;
        }
        loading = false;
    }

    public void PauseGame() {
        Tank.FindPlayer().FreezeNoRotation();
    }

    public void ResumeGame() {
        Tank.FindPlayer().UnfreezeNoRotation();
    }
}
