using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BubbleChar : Character
{
    public float cooldown = 8;
    public float stayTime = 3;
    public float selfDestroyCD = 3;
    public float currCD = 0;
    public GameObject bubblePrefab = Resources.Load<GameObject>("BubbleShield");

    [HideInInspector]
    public bool active;

    [HideInInspector]
    public GameObject obj;

    public override bool Ability(Tank tank)
    {
        Debug.Log("Curr CD: " + currCD);
        Debug.Log("Is active: " + active);
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
        if (active)
        {
            selfDestroyCD -= Time.deltaTime;
            if (selfDestroyCD <= 0)
            {
                DestroyBubble();
            }
        }
        else
        {
            currCD -= Time.deltaTime;
        }
    }

    public override float getCoolDown()
    {
        return cooldown;
    }

    public float getStayTime()
    {
        return stayTime;
    }

    public override bool isActive()
    {
        return active;
    }

    public void DestroyBubble()
    {
        currCD = cooldown;
        selfDestroyCD = stayTime;
        GameObject.Destroy(obj);
        UIManager.instance.Gameplay_Panel.
                        GetComponentInChildren<GameplayHUDManager>()
                        .StartFillAbilityBar(getCoolDown());
    }
}
