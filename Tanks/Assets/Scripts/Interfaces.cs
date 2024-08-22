using System.Collections;
using System.Collections.Generic;
using UnityEngine;

interface IDestroyable {
    void takeDamage(int dmg);
    void incapacitate(float time);
}

interface IAlertableEnemy {
    void alert();
}