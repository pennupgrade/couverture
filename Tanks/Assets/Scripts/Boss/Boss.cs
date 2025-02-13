using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour, IDestroyable
{
    [HideInInspector] public float health = 5000f;

    public GameObject bulletPrefab;
    private GameObject player;
    [SerializeField] private GameObject gun;
    private Transform shotgunBarrel1;
    private Transform shotgunBarrel2;
    private float bulletSpeed;
    public GameObject[] enemyPrefabs;
    public Transform[] summonLocations;
    private List<Rigidbody> enemySpawned;
    [SerializeField] private SlidingWall exitWall;

    private float timer = 0;
    private const float MOVE_TIME = 1.25f;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        shotgunBarrel1 = gun.transform.GetChild(0);
        shotgunBarrel2 = gun.transform.GetChild(1);
        enemySpawned = null;
    }

    public void shootBullet(int barrel = 0)
    {
        // Choose where to shoot for shotgun bullets vs normal bullets
        Transform startPos;
        if (barrel == 0)
            startPos = gun.transform;
        else if (barrel == 1)
            startPos = shotgunBarrel1;
        else
            startPos = shotgunBarrel2;

        // Shoot the bullet forwards
        bool random = true;
        float dispersion = 0.5f;
        GameObject bullet = Object.Instantiate(bulletPrefab, startPos.position, Quaternion.identity);
        bullet.GetComponent<Rigidbody>().velocity = (Quaternion.AngleAxis(dispersion * ((random) ? (Random.value - 0.5f) : 1), Vector3.up)
         * startPos.forward * bullet.GetComponent<Projectile>().bulletSpeed);
        bullet.transform.rotation = Quaternion.LookRotation(bullet.GetComponent<Rigidbody>().velocity);
    }

    // Shoots a bullet through all 3 barrels
    public void shootShotgun()
    {
        shootBullet();
        shootBullet(1);
        shootBullet(2);
    }


    public List<GameObject> Summon()
    {
        // Choose which enemy to spawn
        int random = Random.Range(0, enemyPrefabs.Length);
        
        List<GameObject> enemyList = new List<GameObject>();
        enemySpawned = new List<Rigidbody>();

        // Summon an enemy at all spawn locations
        for (int i = 0; i < summonLocations.Length; i++)
        {
            GameObject enemy = Instantiate(enemyPrefabs[random], summonLocations[i].position, summonLocations[i].rotation);
            enemyList.Add(enemy);
            enemySpawned.Add(enemy.GetComponent<Rigidbody>());
        }

        timer = 0;
        return enemyList;
    }

    private void Update()
    {
        // Move gun to point towards the player
        float GUN_DISTANCE = 1f;
        Vector3 movePos = (player.transform.position - transform.position).normalized * GUN_DISTANCE;
        gun.transform.position = transform.position + movePos;
        gun.transform.rotation = Quaternion.LookRotation(player.transform.position - gun.transform.position);

        // Moves enemies forwards if just spawned
        const float ENEMY_MOVE_SPEED = 1f;
        if (enemySpawned != null)
        {
            timer += Time.deltaTime;
            if (timer >= MOVE_TIME)
            {
                foreach (var enemy in enemySpawned)
                {
                    enemy.velocity = Vector3.zero;
                }
                enemySpawned = null;
            }
            else
            {
                for (int i = 0; i < enemySpawned.Count; i++)
                {
                    enemySpawned[i].velocity = summonLocations[i].forward * ENEMY_MOVE_SPEED;
                }
            }
        }
    }

    public void takeDamage(int dmg)
    {
        health -= dmg;

        if (health < 0)
        {
            Die();
        }
    }

    public void incapacitate(float time)
    {
        // Not Implemented
    }

    public void Die()
    {
        exitWall.activate();
        Destroy(gameObject);
    }
}
