using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketChar : Character
{
    private float cooldown = 5;
    private float currCD = 0;
    public GameObject rocketPrefab = Resources.Load<GameObject>("Rocket");

    public override bool Ability(Tank tank)
    {
        if (currCD <= 0)
        {
            var bullet = Object.Instantiate(rocketPrefab, tank.gunShotPos.position, Quaternion.identity);
            bullet.GetComponent<Rigidbody>().velocity = Quaternion.AngleAxis(0, Vector3.up)
                                                        * (tank.gun.transform.forward *
                                                           bullet.GetComponent<Projectile>().bulletSpeed);
            bullet.transform.rotation = Quaternion.LookRotation(bullet.GetComponent<Rigidbody>().velocity);
            bullet.GetComponent<Projectile>().parent = tank.gameObject;
            currCD = cooldown;
            return true;
        }
        else
        {
            return false;
        }
    }

    public override void AbilityUpdate(Tank tank)
    {
        currCD -= Time.deltaTime;
    }

    public override float getCoolDown()
    {
        return cooldown;
    }

    public override bool isActive()
    {
        return false;
    }
}
