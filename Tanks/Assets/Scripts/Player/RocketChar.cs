using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketChar : Character
{
    public float cooldown;
    private float currCD;
    public GameObject rocketPrefab;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        currCD = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void Ability()
    {
        if (currCD <= 0)
        {
            var bullet = Object.Instantiate(rocketPrefab, tank.gunShotPos.position, Quaternion.identity);
            bullet.GetComponent<Rigidbody>().velocity = Quaternion.AngleAxis(0, Vector3.up)
                                                        * (tank.gun.transform.forward *
                                                           bullet.GetComponent<Projectile>().bulletSpeed);
            bullet.transform.rotation = Quaternion.LookRotation(bullet.GetComponent<Rigidbody>().velocity);
            currCD = cooldown;
        }
    }

    public override void AbilityUpdate()
    {
        currCD -= Time.deltaTime;
    }
}
