using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{
    public GameObject bulletPrefab;
    private GameObject player;
    [SerializeField] private GameObject gun;
    private Transform shotgunBarrel1;
    private Transform shotgunBarrel2;
    private float bulletSpeed;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        shotgunBarrel1 = gun.transform.GetChild(0);
        shotgunBarrel2 = gun.transform.GetChild(1);
    }

    public void shootBullet(int barrel = 0)
    {
        Debug.Log("Bullet Shot");
        Transform startPos;
        if (barrel == 0)
            startPos = gun.transform;
        else if (barrel == 1)
            startPos = shotgunBarrel1;
        else
            startPos = shotgunBarrel2;

        bool random = true;
        float dispersion = 0.5f;
        GameObject bullet = Object.Instantiate(bulletPrefab, startPos.position, Quaternion.identity);
        //bullet.GetComponent<Rigidbody>().velocity = (Quaternion.AngleAxis(dispersion * ((random) ? (Random.value - 0.5f) : 1), Vector3.up)
        // * (new Vector3(target.x - startPos.x, 0, target.z - startPos.z)).normalized * bullet.GetComponent<Projectile>().bulletSpeed);
        //bullet.transform.rotation = Quaternion.LookRotation(bullet.GetComponent<Rigidbody>().velocity);
        bullet.GetComponent<Rigidbody>().velocity = (Quaternion.AngleAxis(dispersion * ((random) ? (Random.value - 0.5f) : 1), Vector3.up)
         * startPos.forward * bullet.GetComponent<Projectile>().bulletSpeed);
        bullet.transform.rotation = Quaternion.LookRotation(bullet.GetComponent<Rigidbody>().velocity);
    }

    public void shootShotgun()
    {
        Debug.Log("Shotgun Shot");
        shootBullet();
        shootBullet(1);
        shootBullet(2);
    }

    private void Update()
    {
        float GUN_DISTANCE = 1f;
        Vector3 movePos = (player.transform.position - transform.position).normalized * GUN_DISTANCE;
        gun.transform.position = transform.position + movePos;
        gun.transform.rotation = Quaternion.LookRotation(player.transform.position - gun.transform.position);
    }
}
