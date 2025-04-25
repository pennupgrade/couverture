using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = System.Random;

public class RoomManager : MonoBehaviour
{
    [HideInInspector] public bool paused, noPause;
    public static RoomManager Instance { get; private set; }
    public static int LevelNum { get; private set; }
    private bool loading;
    private ClassicUIManager uiManager;
    private AudioManager audioManager;
    [SerializeField] private int overrideLevel;

    //call when exiting
    public void DestroyIt() {
        LevelNum = 1;
        Destroy(gameObject);
        Instance = null;
    }

    private void Awake() {
        // start debug save if required
        SaveStateManagerGameObject.LoadClassicModeSave();

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
            randomLevelIndex = 10;
            StartMission();
        }
        else if (Instance != this) {
            Instance.StartMission();
            Destroy(gameObject);
        }

        DontDestroyOnLoad(this);
    }

    private void Update() {
        if (noPause || UIManager.Instance.pauseMenu.IsAnimating) return;

        // Only enable toggling pause if the cat selection panel isn't open
        if (UIManager.Instance.Cat_Selection_Panel.activeInHierarchy) return;

        if (Input.GetKeyDown(KeyCode.Escape)) {
            paused = !paused;

            if (paused) {
                PauseGame();
                UIManager.Instance.pauseMenu.ShowPanel();
            }
            else {
                ResumeGame();
                UIManager.Instance.pauseMenu.HidePanel();
            }
        }
    }

    private void StartMission() => StartCoroutine(_StartMission());

    private IEnumerator _StartMission() {
        noPause = true;

        uiManager.Reset();
        var pTank = Tank.FindPlayer();
        pTank.Freeze(false);

        uiManager.StartScreenEnter(LevelNum);
        yield return new WaitForSeconds(0.3f);

        // Play opening sound effect
        audioManager.Play("StartSound");
        yield return new WaitForSeconds(1.7f);

        // Remove start screen, play BG music
        uiManager.StartScreenLeave();
        yield return new WaitForSeconds(0.2f);

        changeBGM(true);
        pTank.Unfreeze();

        yield return new WaitWhile(() => uiManager.IsAnimating);

        noPause = false;
    }

    public void roomTransition() {
        if (loading) return;

        SaveStateManagerGameObject.UpdateClassicModeHighScore(LevelNum);

        LevelNum++;
        Debug.Log("Loading Level" + LevelNum);

        //scene transition
        if (LevelNum == 1) {
            StartCoroutine(LoadAsyncScene("Classic1"));
        }
        else if (LevelNum == 2 || LevelNum == 15 || LevelNum == 31) {
            StartCoroutine(LoadAsyncScene("Classic2"));
        }
        else if (LevelNum == 3 || LevelNum == 17 || LevelNum == 20) {
            StartCoroutine(LoadAsyncScene("Classic3"));
        }
        else if (LevelNum == 4 || LevelNum == 14 || LevelNum == 34) {
            StartCoroutine(LoadAsyncScene("Classic4"));
        }
        else if (LevelNum == 5 || LevelNum == 16) {
            StartCoroutine(LoadAsyncScene("Classic5"));
        }
        else if (LevelNum == 6 || LevelNum == 30 || LevelNum == 35) {
            if (LevelNum == 30) {
                randomizeSceneArray();
            }

            StartCoroutine(LoadAsyncScene("Classic6"));
        }
        else if (LevelNum == 12 || LevelNum == 50) {
            StartCoroutine(LoadAsyncScene("Classic12"));
        }
        else if (LevelNum == 13 || LevelNum == 32) {
            StartCoroutine(LoadAsyncScene("Classic13"));
        }
        else if (LevelNum == 18 || LevelNum == 36) {
            StartCoroutine(LoadAsyncScene("Classic18"));
        }
        else if (LevelNum == 19 || LevelNum == 37) {
            StartCoroutine(LoadAsyncScene("Classic19"));
        }
        else if (LevelNum == 25 || LevelNum == 33 || LevelNum == 40) {
            var pTank = Tank.FindPlayer();
            if ((LevelNum == 25 && (pTank.CharacterHasAbility() || pTank.character.GetType() == typeof(BubbleChar))) ||
                LevelNum == 40) {
                StartCoroutine(LoadAsyncScene("Classic25alt"));
            }
            else {
                StartCoroutine(LoadAsyncScene("Classic25"));
            }
        }
        else if (LevelNum == 38) {
            StartCoroutine(LoadAsyncScene("Classic38"));
        }
        else if (LevelNum == 45) {
            StartCoroutine(LoadAsyncScene("Classic45"));
        }
        else {
            if (LevelNum == 51) {
                //game end screen, button leads to main menu
                uiManager.MissionCompleteEnter(true);
                SaveStateManagerGameObject.UpdateClassicModeHighScore(LevelNum);

                //classic mode complete sound effect
                changeBGM(false);
                audioManager.Play("WinSound");
            }
            else {
                //randomized level

                randomizedSceneLoader();
            }
        }
    }

    public void PlayerDeath() => StartCoroutine(_PlayerDeath());

    private IEnumerator _PlayerDeath() {
        noPause = true;

        // redirect to death screen showing level reached, button leads to main menu
        uiManager.DeathScreenEnter(LevelNum);

        //play sad sound
        changeBGM(false);
        audioManager.Play("DeathSound");

        yield return new WaitWhile(() => uiManager.IsAnimating);

        // call exit level
        SaveStateManagerGameObject.ExitLevel();
        SaveStateManagerGameObject.SaveToFile();
    }

    public void ResetToLevelOne() => StartCoroutine(_ResetToLevelOne());

    private IEnumerator _ResetToLevelOne() {
        noPause = true;
        loading = true;

        UIManager.Instance.pauseMenu.HidePanel();

        Time.timeScale = 1;
        changeBGM(false);

        var restartQuit = uiManager.Restart();

        SaveStateManagerGameObject.ExitLevel();
        SaveStateManagerGameObject.SaveToFile();

        yield return new WaitWhile(() => restartQuit.IsAnimating);

        DestroyIt();
        EnemySpawner.reset();
        SceneManager.LoadScene("Classic1");
    }

    private IEnumerator LoadAsyncScene(string sceneName) {
        noPause = true;
        loading = true;

        var operation = SceneManager.LoadSceneAsync(sceneName)!;
        operation.allowSceneActivation = false;

        yield return new WaitForSeconds(0.7f);

        //level complete screen
        uiManager.MissionCompleteEnter(false);
        Tank.FindPlayer().FreezeEndOfLevel();

        //play level complete sound
        changeBGM(false);
        audioManager.Play("CompleteSound");
        yield return new WaitForSeconds(1.4f);

        //level complete screen fades out
        uiManager.MissionCompleteLeave();
        yield return new WaitForSeconds(1f);

        SaveStateManagerGameObject.FinishLevel(sceneName, false);

        // reset stats
        SaveStateManagerGameObject.PlayerDied();

        yield return new WaitWhile(() => uiManager.IsAnimating);
        yield return new WaitForSecondsRealtime(0.15f);

        operation.allowSceneActivation = true;
        loading = false;
    }

    private void changeBGM(bool play) {
        if (play) {
            audioManager.Play("BGM");
        }
        else {
            audioManager.Stop("BGM");
        }
    }

    public void changeEnemyCountUI() {
        if (LevelNum != 50) {
            uiManager.UpdateEnemyCount(EnemySpawner.EnemiesRemaining);
        }
    }

    public void PauseGame() {
        Tank.FindPlayer().Freeze(false);
        Time.timeScale = 0;
    }

    public void ResumeGame() {
        Tank.FindPlayer().Unfreeze();
        Time.timeScale = 1;
    }

    /// <summary>
    ///     Create random permutation of numbers 1 to 10 to select levels from
    /// </summary>
    private int[] randomNumbers;
    private int randomLevelIndex;

    private void randomizeSceneArray() {
        randomLevelIndex = 0;
        var rnd = new Random();
        var numbers = new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
        randomNumbers = numbers.OrderBy(x => rnd.Next()).ToArray();
    }

    private void randomizedSceneLoader() {
        if (randomLevelIndex > 9) {
            randomizeSceneArray();
        }

        var r = randomNumbers[randomLevelIndex];
        randomLevelIndex++;
        if (r == 1) {
            StartCoroutine(LoadAsyncScene("ClassicA"));
        }
        else if (r == 2) {
            StartCoroutine(LoadAsyncScene("ClassicB"));
        }
        else if (r == 3) {
            StartCoroutine(LoadAsyncScene("ClassicC"));
        }
        else if (r == 4) {
            StartCoroutine(LoadAsyncScene("ClassicD"));
        }
        else if (r == 5) {
            StartCoroutine(LoadAsyncScene("ClassicE"));
        }
        else if (r == 6) {
            StartCoroutine(LoadAsyncScene("ClassicF"));
        }
        else if (r == 7) {
            StartCoroutine(LoadAsyncScene("ClassicG"));
        }
        else if (r == 8) {
            StartCoroutine(LoadAsyncScene("ClassicH"));
        }
        else if (r == 9) {
            StartCoroutine(LoadAsyncScene("ClassicI"));
        }
        else if (r == 10) {
            StartCoroutine(LoadAsyncScene("ClassicJ"));
        }
    }

    public void ReturnToMainMenu() => StartCoroutine(_ReturnToMainMenu());

    private IEnumerator _ReturnToMainMenu() {
        noPause = true;
        var restartQuit = uiManager.Quit();

        yield return new WaitForSeconds(0.1f);
        yield return new WaitWhile(() => restartQuit.IsAnimating);

        DestroyIt();
        EnemySpawner.reset();
        SceneManager.LoadScene("TitleScreen");
    }
}