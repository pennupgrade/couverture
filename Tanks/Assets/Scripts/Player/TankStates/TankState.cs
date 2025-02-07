using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public abstract class TankState
{
    protected Tank tank;

    public TankState(Tank tank)
    {
        this.tank = tank;
    }

    public abstract TankState HandleMovement(Vector2 dir);

    public virtual TankState HandleGunRotation(Vector2 val)
    {
        // Debug.Log(Camera.main);
        var ray = Camera.main.ScreenPointToRay(val);

        var plane = new Plane(Vector3.up, tank.gun.transform.position);

        float dist;
        plane.Raycast(ray, out dist);

        var point = ray.GetPoint(dist);

        var offset = (point - tank.gun.transform.position).normalized;
        Vector3 dir = new(offset.x, 0, offset.z);

        var angle = Vector3.SignedAngle(tank.Body.transform.forward, dir, Vector3.up);

        Debug.DrawRay(tank.gun.transform.position, point - tank.gun.transform.position, Color.green);

        Quaternion s0 = tank.gun.transform.localRotation;
        //tank.gun.transform.localRotation = Quaternion.Slerp(s0, Quaternion.Euler(0, angle, 0), 12.8f * Time.deltaTime);
        tank.gun.transform.localRotation = Quaternion.Euler(0, angle, 0);
        return this;
    }

    protected bool spawnInsideWallCheck()
    {
        return
            Physics.Raycast(tank.gameObject.transform.position,
                            tank.gunShotPos.position - tank.gameObject.transform.position,
                            Vector3.Distance(tank.gameObject.transform.position, tank.gunShotPos.position), 1 << 3);
    }

    public virtual TankState HandleShoot(Vector3 offsetVelocity)
    {
        if (tank.numBullets <= 0 || tank.cooldownCoroutine != null || spawnInsideWallCheck()) return this;
        tank.numBullets--;
        tank.cooldownCoroutine = tank.StartCoroutine(Cooldown());
        var bullet = Object.Instantiate(tank.bulletPrefab, tank.gunShotPos.position, Quaternion.identity);
        bullet.GetComponent<Rigidbody>().velocity = Quaternion.AngleAxis(0, Vector3.up)
                                                    * (tank.gun.transform.forward *
                                                       bullet.GetComponent<Projectile>().bulletSpeed);
        bullet.GetComponent<Bullet_Default>().addBounceChange();
        bullet.GetComponent<Bullet_Default>().parent = tank.gameObject;
        bullet.transform.rotation = Quaternion.LookRotation(bullet.GetComponent<Rigidbody>().velocity);

        //offset velocity is helpful when the object firing the bullet is moving
        bullet.GetComponent<Rigidbody>().velocity += offsetVelocity;

        // Reload bullets if we're not already doing so
        if (tank.reloadCoroutine == null) tank.reloadCoroutine = tank.StartCoroutine(Reload());
        return this;
    }

    public virtual IEnumerator Cooldown()
    {
        tank.cannonAnimator.SetTrigger("Fire");
        tank.cooldownProgress = 0f;
        tank.StartCoroutine(AnimationCooldown());

        while (tank.cooldownProgress <= Tank.COOLDOWN_TIME)
        {
            tank.cooldownProgress += Time.deltaTime;
            yield return null;
        }

        tank.cooldownCoroutine = null;
        yield return null;
    }

    public virtual IEnumerator AnimationCooldown()
    {
        tank.animationProgress = 0f;
        var meshMaterial = tank.cannonAnimator.gameObject.GetComponent<MeshRenderer>().material;



        while (tank.animationProgress <= Tank.COOLDOWN_TIME)
        {
            tank.animationProgress += Time.deltaTime / 1.5f;
            float boomProg = Mathf.Min(0.99f, tank.animationProgress / (Tank.COOLDOWN_TIME)); // 0-1

            meshMaterial.SetFloat("_Boom", Mathf.Max(0.0f, boomProg));
            yield return null;
        }

        tank.cooldownCoroutine = null;
        yield return null;
    }

    public virtual IEnumerator Reload()
    {
        while (tank.numBullets < 5)
        {
            tank.reloadProgress = 0f;

            while (tank.reloadProgress <= Tank.RELOAD_TIME)
            {
                tank.reloadProgress += Time.deltaTime;
                yield return null;
            }

            tank.numBullets++;
        }

        tank.reloadCoroutine = null;
        yield return null;
    }
}
