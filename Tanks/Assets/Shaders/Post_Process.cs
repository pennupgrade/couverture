using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Post_Process : MonoBehaviour
{
    public Material fullscreenmaterial;
    private int invertedon = 0;
    void Update () {
        if (Input.GetKeyDown(KeyCode.Space)){
            invertedon = 1- invertedon;
            fullscreenmaterial.SetInt("_InvertedOn", invertedon);
            print("InvertedOn: " + invertedon);
        }
    }

}