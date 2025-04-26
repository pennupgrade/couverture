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
        if (Camera.main == false) {
            return null;
        }
        if (Camera.main.enabled) {
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
        return null;
    }

    protected bool spawnInsideWallCheck()
    {
        Collider[] hitColliders = Physics.OverlapSphere(tank.gunShotPos.position, 0.1f, 1 << 8);
        foreach (var h in hitColliders) {
            if (h.gameObject.TryGetComponent<Enemy>(out Enemy e)) {
                e.takeDamage(100);
                return true;
            }
        }
        bool hit = Physics.Raycast(tank.gunShotPos.position - 0.2f * tank.gunShotPos.forward,
                            tank.gunShotPos.forward, out RaycastHit hitInfo,
                            Vector3.Distance(tank.gameObject.transform.position, tank.gunShotPos.position), 1 << 3);


        // return hit;
        return hit && hitInfo.collider.gameObject.tag != "Destructable";
    }

    public virtual TankState HandleShoot(Vector3 offsetVelocity)
    {
        if (tank.disableFire) return this;
        if (tank.numBullets <= 0) {
            tank.audioManager.Play("Blank");
            return this;
        }

        if (tank.cooldownCoroutine != null || spawnInsideWallCheck()) return this;
        tank.audioManager.Play("Fire");
        tank.numBullets--;
        tank.cooldownCoroutine = tank.StartCoroutine(Cooldown());
        var bullet = tank.SpawnBullet();
        bullet.transform.position = tank.gunShotPos.position;
        bullet.transform.rotation = Quaternion.identity;
        bullet.GetComponent<Rigidbody>().velocity = Quaternion.AngleAxis(0, Vector3.up)
                                                    * (tank.gun.transform.forward *
                                                       bullet.GetComponent<Projectile>().bulletSpeed);
        bullet.GetComponent<Bullet_Default>().addBounceChange();
        bullet.GetComponent<Projectile>().parent = tank.gameObject;
        bullet.transform.rotation = Quaternion.LookRotation(bullet.GetComponent<Rigidbody>().velocity);

        //offset velocity is helpful when the object firing the bullet is moving
        bullet.GetComponent<Rigidbody>().velocity += offsetVelocity;

        // Reload bullets if we're not already doing so
        if (tank.reloadCoroutine == null) ResetReload();
        return this;
    }

    public virtual IEnumerator Cooldown()
    {
        tank.cooldownProgress = 0f;
        //tank.cannonAnimator.SetTrigger("Fire");
        tank.StartCoroutine(AnimationCooldown());
        float cdTime = Tank.COOLDOWN_TIME +  (tank.numBullets <= 3 ? 0.16f : 0);

        while (tank.cooldownProgress <= cdTime)
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
            boomProg = 1.0f - Mathf.Max(0.0f, boomProg);
            boomProg = boomProg * 2.0f - 1.0f; // [-1, 1]

            meshMaterial.SetFloat("_T", boomProg);
            yield return null;
        }

        tank.cooldownCoroutine = null;
        yield return null;
    }

    public virtual IEnumerator Reload()
    {
        while (tank.numBullets < Tank.MAX_BULLETS)
        {
            tank.reloadProgress = 0f;

            while (tank.reloadProgress <= Tank.RELOAD_TIME)
            {
                tank.reloadProgress += Time.deltaTime;
                yield return null;
            }

            tank.numBullets++;
            tank.audioManager.Play("Reloaded");
        }

        tank.reloadCoroutine = null;
        yield return null;
    }
    
    public void ResetReload() {
        tank.reloadCoroutine = tank.StartCoroutine(Reload());
    }
}
