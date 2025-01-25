using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Character : MonoBehaviour
{
    protected Tank tank;

    protected virtual void Start()
    {
        tank = GameObject.FindObjectOfType<Tank>();
    }
    public abstract void Ability();

    public abstract void AbilityUpdate();
}
