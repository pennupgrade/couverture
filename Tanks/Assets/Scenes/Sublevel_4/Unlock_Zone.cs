using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unlock_Zone : MonoBehaviour
{

    [Header("Write BUBBLE_CAT or ROCKET_CAT.")]
    [Header("It's probably case sensitive.")]
    [Header("Make sure you spell the thing corectly")]
    [Header("Otherwise I think it's going to creash.")]
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
            SaveStateManagerGameObject.UnlockCharacter(characterToUnlock);
        }
        // if (other.gameObject.tag == "Tank")
        // {
        //     other.GetComponent<Enemy>().takeDamage(300000);
        //     Debug.Log("tank taking damage");
        // }
    }
}
