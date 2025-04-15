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
            Destroy(this);
        }
        DontDestroyOnLoad(this);
    }

    public void startScreen() {
        //play opening sound effect
        StartCoroutine(startScreenCoroutine());
    }
    private IEnumerator startScreenCoroutine() {
        Tank pTank = FindPlayer();
        pTank.FreezeNoRotation();
        
        //fade in level number text
        uiManager.StartScreenTextFadeIn(LevelNum);
        yield return new WaitForSeconds(3);
        pTank.UnfreezeNoRotation();

        // remove start screen, play BG music
        uiManager.StartScreenFadeOut();
    } 

    public void roomTransition() {
        if (loading) return;

        SaveStateManagerGameObject.UpdateClassicModeHighScore(LevelNum);

        LevelNum++;
        Debug.Log("Loading Level" + LevelNum);

        //scene transition
        if (LevelNum == 1) {
            StartCoroutine(LoadAsyncScene("Classic1"));
        } else if (LevelNum == 2) {
            StartCoroutine(LoadAsyncScene("Classic2"));
        } else if (LevelNum == 3) {
            StartCoroutine(LoadAsyncScene("Classic3"));
        } else if (LevelNum == 4) {
            StartCoroutine(LoadAsyncScene("Classic4"));
        } else if (LevelNum == 5) {
            StartCoroutine(LoadAsyncScene("Classic5"));
        } else if (LevelNum == 6) {
            StartCoroutine(LoadAsyncScene("Classic6"));
        } else {
            if (LevelNum == 60) {
                //game end screen, displays time taken, button leads to main menu
                uiManager.LevelCompleteScreenFadeIn(true);
                //classic mode complete sound effect

                
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
        uiManager.DeathScreenFadeIn();
        //play sad sound

        // call exit level
        SaveStateManagerGameObject.ExitLevel();
        SaveStateManagerGameObject.SaveToFile();

        destroyInstance();
        EnemySpawner.reset(); // resets enemy counter
    }

    IEnumerator LoadAsyncScene(string sceneName) {
        //level complete screen
        uiManager.LevelCompleteScreenFadeIn(false);
        //play level complete sound
        yield return new WaitForSeconds(2f);
        //level complete screen fades out
        uiManager.LevelCompleteScreenTextFadeOut();

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

    private Tank FindPlayer() {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) {
            Debug.Log("RoomManager: Could not find player");
        }
        return player.GetComponent<Tank>();
    }

    public void PauseGame() {
        FindPlayer().FreezeNoRotation();
    }

    public void ResumeGame() {
        FindPlayer().UnfreezeNoRotation();
    }
}
