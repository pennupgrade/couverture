using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Character : MonoBehaviour
{
    protected Tank tank;

    protected virtual void Start() 
    // i still think you should use a constructor :( idk if this will be bad if we have many objects in a scene - Anthony
    {
        tank = GameObject.FindObjectOfType<Tank>();
    }
    public abstract void Ability();

    public abstract void AbilityUpdate();
}
