using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : Activatable
{
    
    [SerializeField] bool easyMode;
    [SerializeField] bool hardMode;

    public static int EnemiesRemaining {
        get {
            return enemiesRemaining;
        }
        set {
            enemiesRemaining = value;
            //update UI
            RoomManager.Instance.changeEnemyCountUI();
        }
    }
    private static int enemiesRemaining; // = 0;
    [SerializeField] int section;
    [SerializeField] int levelNumber;
    public GameObject[] enemies;
    private float spawnDelay = 2.8f;

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
        Tank pTank = Tank.FindPlayer();
        bool rocket = pTank.CharacterHasAbility();
        bool bubble = pTank.character.GetType() == typeof(BubbleChar);
        if (easyMode && (rocket || (bubble && RoomManager.LevelNum % 5 == 0))) return;
        if (hardMode) {
            if (bubble && RoomManager.LevelNum % 5 != 0) return;
            if (!rocket && !bubble) return;
        }
        if (levelNumber > 0) {
            if (RoomManager.LevelNum != levelNumber) return;
        } else {
            if (section == 1 && RoomManager.LevelNum > 12) return;
            if (section == 3 && RoomManager.LevelNum < 31) return;
            if (section == 2 && (RoomManager.LevelNum < 13 || RoomManager.LevelNum > 30)) return;
        }

        StartCoroutine(spawn());
    }

    private IEnumerator spawn() {
        int r = (int) Mathf.Floor(enemies.Length * Random.value);
        if (enemies[r].TryGetComponent<Enemy>(out Enemy e)) {
            EnemiesRemaining++;
            yield return new WaitForSeconds(spawnDelay);
            enemies[r].SetActive(true);
            if (e is Sentry) {
                e.gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
            }
            e.onDeath += enemyDestroyed;
        } else {
            EnemiesRemaining += enemies[r].transform.childCount;
            yield return new WaitForSeconds(spawnDelay);
            for (int i = 0; i < enemies[r].transform.childCount; ++i) {
                if (enemies[r].transform.GetChild(i).gameObject.TryGetComponent<Enemy>(out Enemy enemy)) {
                    enemy.gameObject.SetActive(true);
                    if (enemy is Sentry) {
                        enemy.gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
                    }
                    enemy.onDeath += enemyDestroyed;
                }
            }
        }
    }

    public void enemyDestroyed() {
        EnemiesRemaining--;
        if (EnemiesRemaining <= 0) {
            EnemiesRemaining = 0;

            Tank pTank = Tank.FindPlayer();
            if (pTank != null && pTank.health > 0) {
                pTank.setInvincible(true);
                //change level
                RoomManager.Instance.roomTransition();
            }

        }
    }

}
