using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Boss : MonoBehaviour, IDestroyable
{
    [HideInInspector] public float health = 3000f;

    public GameObject bulletPrefab;
    private GameObject player;
    [SerializeField] private GameObject gun;
    private Transform shotgunBarrel1;
    private Transform shotgunBarrel2;
    private Transform shotgunBarrel3;
    private float bulletSpeed;
    public GameObject[] enemyPrefabs;
    public Transform[] summonLocations;
    public WireDeath[] wires;
    private bool unplugged;
    private int ind; 
    [SerializeField] private GameObject wirePlug;
    private List<Rigidbody> enemySpawned;
    [SerializeField] private SlidingWall exitWall;
    private float timer = 0;
    private const float MOVE_TIME = 1.25f;
   [SerializeField] private GameObject shield;
    [SerializeField] private BossMovement bm;
    protected DamageFlash df;
    [SerializeField] private GameObject missilePrefab;
    private List<List<GameObject>> enemies;
    
    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        shotgunBarrel1 = gun.transform.GetChild(0);
        shotgunBarrel2 = gun.transform.GetChild(1);
        shotgunBarrel3 = gun.transform.GetChild(2);
        enemySpawned = null;
        df = new DamageFlash(this.gameObject);
        ind = 0;
    }

    public void setEnemies(List<List<GameObject>> enemies)
    {
        this.enemies = enemies;
    }

    public void shootBullet(int barrel = 0)
    {
        // Choose where to shoot for shotgun bullets vs normal bullets
        Transform startPos;
        if (barrel == 0)
            startPos = shotgunBarrel1;
        else if (barrel == 1)
            startPos = shotgunBarrel2;
        else
            startPos = shotgunBarrel3;

        // Shoot the bullet forwards
        bool random = false;
        float dispersion = 0.5f;
        GameObject bullet = PoolManager.bulletPool.Get().gameObject;
        bullet.transform.position = new Vector3(startPos.position.x, -1.055195f, startPos.position.z);
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        rb.velocity = (Quaternion.AngleAxis(dispersion * ((random) ? (Random.value - 0.5f) : 1), Vector3.up)
         * startPos.forward * bullet.GetComponent<Projectile>().bulletSpeed);
        rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        bullet.transform.rotation = Quaternion.LookRotation(rb.velocity);
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
        gun.transform.LookAt(player.transform);

        if (!unplugged) {
            foreach (WireDeath wd in wires) {
                if (wd.gameObject.TryGetComponent<CharacterJoint>(out CharacterJoint c)) {
                } else {
                    unplug();
                }
            }
        }

        if (unplugged && ind < wires.Length) {
            wires[ind].Kill();
            ind++;
        }

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

    public void unplug() {
        unplugged = true;
        shield.SetActive(false);
    }

    public bool isUnplugged() {
        return unplugged;
    }

    public void takeDamage(int dmg)
    {
        health -= dmg;
        df.CallDamageFlash(this);
        if (health < 0)
        {
            player.GetComponent<Tank>().enabled = true;
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
        for (int i = 0; i < enemies.Count; i++)
        {
            for (int j = 0; j < enemies[i].Count; j++)
            {
                enemies[i][j].GetComponent<Enemy>().takeDamage(9999999);
            }
        }
        Destroy(gameObject);
    }
    public void ShootMissile()
    {
        Vector3 shootPosition = transform.position + new Vector3(0, 2, 0);
        GameObject missile = Instantiate(missilePrefab, shootPosition, Quaternion.identity);
        MissileScript missileScript = missile.GetComponent<MissileScript>();
        missileScript.initialize(shootPosition, player.transform.position);
    }
}
