using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BubbleChar : Character
{
    private float cooldown = 3;
    private float currCD = 0;
    public GameObject bubblePrefab = Resources.Load<GameObject>("BubbleShield");

    [HideInInspector]
    public bool active;

    [HideInInspector]
    public GameObject obj;

    public override bool Ability(Tank tank)
    {
        if (!active && currCD <= 0)
        {   
            active = true;
            obj = Object.Instantiate(bubblePrefab);
            obj.GetComponent<Bubble>().targetTransform = tank.transform;
            return true;
        }
        else
        {
            return false;
        }
    }

    public override void AbilityUpdate(Tank tank)
    {
        if (obj == null) {
            active = false;
        }
        currCD -= Time.deltaTime;
    }

    public override float getCoolDown()
    {
        return cooldown;
    }
}
