using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityHandler : MonoBehaviour
{
    public Character player;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        player.AbilityUpdate();
        if (Input.GetKeyDown(KeyCode.Q))
        {
            player.Ability();
        }
    }
}
