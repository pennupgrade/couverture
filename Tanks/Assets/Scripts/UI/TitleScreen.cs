using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScreen : MonoBehaviour
{
    public void StartGame() {
        SceneManager.LoadScene("Save Select");
    }

    public void OpenOptions() {
        throw new NotImplementedException();
    }

    public void Quit() => Application.Quit();
}
