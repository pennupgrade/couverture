using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Classic1Spawner : MonoBehaviour
{
    public int section;
    public GameObject enemy;
    void Start() {
        StartCoroutine(spawn());
    }

    IEnumerator spawn() {
        EnemySpawner.EnemiesRemaining++;
        yield return new WaitForSeconds(2.8f);
        enemy.gameObject.SetActive(true);
        enemy.GetComponent<Enemy>().onDeath += enemyDestroyed;
    }

    public void enemyDestroyed() {
        EnemySpawner.EnemiesRemaining -= 1;
        Tank pTank = Tank.FindPlayer();
        if (pTank != null && pTank.health > 0) {
            pTank.setInvincible(true);
            //change level
            RoomManager.Instance.roomTransition(section);
        }
    }

}
