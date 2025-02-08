using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BubbleChar : Character
{
    public float cooldown;
    public GameObject bubblePrefab;

    [HideInInspector]
    public bool active;

    [HideInInspector]
    public GameObject obj;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        active = false;
    }

    public override void Ability()
    {
        if (!active)
        {   
            active = true;
            obj = Instantiate(bubblePrefab);
            obj.GetComponent<Bubble>().targetTransform = transform;
        }
    }

    public override void AbilityUpdate()
    {
        if (obj == null) {
            active = false;
        }
    }
}
