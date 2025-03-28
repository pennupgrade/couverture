using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unlock_Zone : MonoBehaviour
{
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
            Debug.Log("SaveStateManagerGameObject.UnlockCharacter(SaveStateManager.CharacterOption.BUBBLE_CAT);");
            SaveStateManagerGameObject.UnlockCharacter(SaveStateManager.CharacterOption.BUBBLE_CAT);
        }
        // if (other.gameObject.tag == "Tank")
        // {
        //     other.GetComponent<Enemy>().takeDamage(300000);
        //     Debug.Log("tank taking damage");
        // }
    }
}
