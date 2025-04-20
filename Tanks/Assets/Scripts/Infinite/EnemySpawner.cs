using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : Activatable
{
    
    [SerializeField] bool easyMode;
    [SerializeField] bool hardMode;
    static int enemiesRemaining; // = 0;
    [SerializeField] int section;
    [SerializeField] int levelNumber;
    public GameObject[] enemies;
    private float spawnDelay = 2f;

    //call at start
    public static void reset() {
        enemiesRemaining = 0;
    }

    public override void activate() {
        if (!activated && enemiesRemaining > 0) {
            
            spawn();
            activated = true;
        }
    }

    void Start()
    {
        if (enemies.Length == 0) return;
        bool rocket = Tank.FindPlayer().CharacterHasAbility();
        if (easyMode && rocket) return;
        if (hardMode && !rocket) return;
        if (levelNumber > 0) {
            if (RoomManager.LevelNum != levelNumber) return;
        } else {
            if (section == 1 && RoomManager.LevelNum > 15) return;
            if (section == 3 && RoomManager.LevelNum < 36) return;
            if (section == 2 && (RoomManager.LevelNum < 16 || RoomManager.LevelNum > 35)) return;
        }

        StartCoroutine(spawn());
    }

    private IEnumerator spawn() {
        yield return new WaitForSeconds(spawnDelay);
        int r = (int) Mathf.Floor(enemies.Length * Random.value);
        if (enemies[r].TryGetComponent<Enemy>(out Enemy e)) {
            enemiesRemaining++;
            enemies[r].SetActive(true);
            e.onDeath += enemyDestroyed;
        } else {
            for (int i = 0; i < enemies[r].transform.childCount; ++i) {
                if (enemies[r].transform.GetChild(i).gameObject.TryGetComponent<Enemy>(out Enemy enemy)) {
                    enemiesRemaining++;
                    enemies[r].transform.GetChild(i).gameObject.SetActive(true);
                    enemy.onDeath += enemyDestroyed;
                }
            }
        }
    }

    public void enemyDestroyed() {
        enemiesRemaining--;
        if (enemiesRemaining <= 0) {
            enemiesRemaining = 0;

            Tank pTank = Tank.FindPlayer();
            if (pTank != null && pTank.health > 0) {
                pTank.setInvincible(true);
                //change level
                RoomManager.Instance.roomTransition();
            }

        }
    }

}
