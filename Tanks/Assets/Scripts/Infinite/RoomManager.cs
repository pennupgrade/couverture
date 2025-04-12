using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RoomManager : MonoBehaviour
{
    public static RoomManager Instance { get; private set;}
    public static int LevelNum { get; private set;}
    bool loading;
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
        if (overrideLevel > 0) {
            LevelNum = overrideLevel;
            overrideLevel = -1;
        }
        if (Instance == null) {
            Instance = this;
        } else if (Instance != this) {
            Destroy(this);
        }
        DontDestroyOnLoad(this);
    }

    public void roomTransition() {
        if (loading) return;

        LevelNum++;
        Debug.Log("Loading Level" + LevelNum);

        //shader transition
        //level transition

        if (LevelNum == 1) {
            StartCoroutine(LoadAsyncScene("Classic1"));
        } else if (LevelNum == 2) {
            StartCoroutine(LoadAsyncScene("Classic2"));
        } else {
            if (LevelNum == 3) {
                //game end screen, displays time taken, leads to main menu

                //temporary
                playerDeath();
            } else {
                //randomized level
            }
        }
    }
    public void playerDeath() {
        // redirect to death screen showing level reached, leads to main menu

        //temporary
        LevelNum = 0;
        roomTransition();
    }

    IEnumerator LoadAsyncScene(string sceneName) {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        loading = true;

        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
        loading = false;
    }
}
