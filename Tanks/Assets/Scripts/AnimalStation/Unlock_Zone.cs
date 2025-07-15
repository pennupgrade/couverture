// SPDX-FileCopyrightText: 2024 The Catanks Contributors
//
// SPDX-License-Identifier: MPL-2.0

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unlock_Zone : MonoBehaviour
{
    [Header("Choose which cat you want to unlock.")]
    [SerializeField] public SaveStateManager.CharacterOption characterToUnlock;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other) {
        Debug.Log(other.name);
        if(other.gameObject.tag == "Player")
        {
            // other.GetComponent<Tank>().takeDamage(300000);
            // Debug.Log("taking damage");
            // if (characterToUnlock == SaveStateManager.CharacterOption.BUBBLE_CAT) {
            //     Debug.Log("SaveStateManagerGameObject.UnlockCharacter(SaveStateManager.CharacterOption.BUBBLE_CAT);");
            //     SaveStateManagerGameObject.UnlockCharacter(SaveStateManager.CharacterOption.BUBBLE_CAT);
            // } else if (characterToUnlock == SaveStateManager.CharacterOption.ROCKET_CAT) {
            //     Debug.Log("SaveStateManagerGameObject.UnlockCharacter(SaveStateManager.CharacterOption.ROCKET_CAT);");
            //     SaveStateManagerGameObject.UnlockCharacter(SaveStateManager.CharacterOption.ROCKET_CAT);
            // } else {
            //     Debug.Log("SaveStateManagerGameObject.UnlockCharacter(SaveStateManager.CharacterOption.DEFAULT_CAT);");
            //     SaveStateManagerGameObject.UnlockCharacter(SaveStateManager.CharacterOption.DEFAULT_CAT);
            // }
            SaveStateManagerGameObject.UnlockCharacter(characterToUnlock);
            
        }
        // if (other.gameObject.tag == "Tank")
        // {
        //     other.GetComponent<Enemy>().takeDamage(300000);
        //     Debug.Log("tank taking damage");
        // }
    }
}
