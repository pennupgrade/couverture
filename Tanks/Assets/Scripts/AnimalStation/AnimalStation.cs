using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalStation : MonoBehaviour
{
    [SerializeField] public SaveStateManager.CharacterOption characterToUnlock;

    private bool inRange = false;

    void Start()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            inRange = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            inRange = false;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("Clicked");
            Debug.Log(inRange);
        }
        
        if(inRange && Input.GetKeyDown(KeyCode.E))
        {
            SaveStateManagerGameObject.UnlockCharacter(characterToUnlock);
            UIManager.instance.Update_CatSelectionPanel_DuringGame();
            UIManager.instance.Open_CatSelectionPanel_DuringGame();
        }
    }
}
