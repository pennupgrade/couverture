using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveSpawner : Activatable
{
    public float spawnDelay = 0.5f;
    public GameObject[][] enemies;
    public GameObject[] endUnlock; 
    [SerializeField] private int waveNum, enemiesRemaining;
    public override void activate() {
        if (activated) return;
        activated = true;
        waveNum = 0;
        StartCoroutine(SpawnDelay());
    }
    private IEnumerator SpawnDelay() {
        yield return new WaitForSeconds(spawnDelay);
        spawnEnemies();
    }

    private void spawnEnemies() {
        enemiesRemaining = enemies[waveNum].Length;
        foreach (GameObject g in enemies[waveNum]) {
            g.SetActive(!g.activeSelf);
            g.GetComponent<Enemy>().onDeath += enemyDestroyed;
        }
    }

    public void enemyDestroyed() {
        enemiesRemaining -= 1;
        if (enemiesRemaining <= 0) {
            waveNum++;
            if (waveNum >= enemies.Length) {
                wavesCleared();
            } else {
                StartCoroutine(SpawnDelay());
            }
        }
    }
    private void wavesCleared() {
        foreach (GameObject g in endUnlock) {
            if (g.TryGetComponent<Activatable>(out Activatable aObj)) {
                aObj.activate();
            }
        }
    }
}
