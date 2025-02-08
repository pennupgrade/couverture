using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BubbleChar : Character
{
    public float cooldown;
    private float currCD = 0;
    public GameObject bubblePrefab = Resources.Load<GameObject>("BubbleShield");

    [HideInInspector]
    public bool active;

    [HideInInspector]
    public GameObject obj;

    public override void Ability(Tank tank)
    {
        if (!active)
        {   
            active = true;
            obj = Object.Instantiate(bubblePrefab);
            obj.GetComponent<Bubble>().targetTransform = tank.transform;
        }
    }

    public override void AbilityUpdate(Tank tank)
    {
        if (obj == null) {
            active = false;
        }
    }
}
