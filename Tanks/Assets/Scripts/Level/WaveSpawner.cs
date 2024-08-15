using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveSpawner : Activatable
{
    public GameObject[] wave1;
    public GameObject[] wave2; 
    public GameObject[] wave3; 
    public GameObject[] wave4; 
    public GameObject[] wave5; 
    public float spawnDelay = 0.5f;
    private ArrayList enemies = new ArrayList();
    public GameObject[] endUnlock; 
    [SerializeField] private int waveNum, enemiesRemaining;
    void Start() {
        enemies.Add(wave1);
        if (wave2.Length > 0) {
            enemies.Add(wave2);
        }
        if (wave3.Length > 0) {
            enemies.Add(wave3);
        }
        if (wave4.Length > 0) {
            enemies.Add(wave4);
        }
        if (wave5.Length > 0) {
            enemies.Add(wave4);
        }
    }
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
        enemiesRemaining = ((GameObject[])enemies[waveNum]).Length;
        foreach (GameObject g in (GameObject[]) enemies[waveNum]) {
            g.SetActive(!g.activeSelf);
            g.GetComponent<Enemy>().onDeath += enemyDestroyed;
        }
    }

    public void enemyDestroyed() {
        enemiesRemaining -= 1;
        if (enemiesRemaining <= 0) {
            waveNum++;
            if (waveNum >= enemies.Count) {
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
            } else {
                g.SetActive(!g.activeSelf);
            }
        }
    }
}
