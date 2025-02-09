using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CheckpointManager : MonoBehaviour
{
    public static CheckpointManager Instance = null;

    private readonly Dictionary<Checkpoint, int> checkpointDict = new();

    private Checkpoint currCheckpoint;

    private void SetUpCheckpoints() {
        for (int i = 0; i < transform.childCount; i++) {
            Checkpoint nextCheckpoint = transform.GetChild(i).gameObject.GetComponent<Checkpoint>();
            if (nextCheckpoint is null) { // null check
                throw new InvalidOperationException();
            }
            checkpointDict[nextCheckpoint] = i;
        }
    }

    void Awake() {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        // is this the right place to put this?
        SetUpCheckpoints();
        SaveStateManagerGameObject.SetupCheckpointManager();
        Respawn();
    }

    public static void ForceSetCurrentCheckpoint(int newCheckpoint) {
        Instance.currCheckpoint = Instance.transform.GetChild(newCheckpoint).GetComponent<Checkpoint>();
    }

    public static void CheckpointActivated(Checkpoint checkpoint) {
        if (!Instance.checkpointDict.ContainsKey(checkpoint)) {
            throw new InvalidOperationException();
        }
        int checkpointNum = Instance.checkpointDict[checkpoint];
        if (Instance.currCheckpoint is null || checkpointNum > Instance.checkpointDict[Instance.currCheckpoint]) {
            Instance.currCheckpoint = checkpoint;
        }
        SaveStateManagerGameObject.SetCurrentCheckpoint(SceneManager.GetActiveScene().name, checkpointNum);
    }

    public static void Respawn() {
        if (Instance.currCheckpoint is not null) {
            GameObject.FindWithTag("Player+Camera").transform.position = Instance.currCheckpoint.transform.position;
        }
    }

    public static Checkpoint GetCurrCheckpoint() {
        return Instance.currCheckpoint;
    }
}