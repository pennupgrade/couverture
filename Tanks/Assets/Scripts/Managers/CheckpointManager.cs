using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CheckpointManager : MonoBehaviour
{
    public static CheckpointManager Instance = null;

    private readonly Dictionary<Checkpoint, int> checkpointDict = new();

    private Checkpoint[] checkpointArr;

    private Checkpoint currCheckpoint;

    private void GetCheckpointList() {
        for (int i = 0; i < transform.childCount; i++) {
            checkpointArr = new Checkpoint[transform.childCount];
            Checkpoint nextCheckpoint = transform.GetChild(i).gameObject.GetComponent<Checkpoint>();
            if (nextCheckpoint is null) { // null check
                throw new InvalidOperationException();
            }
            checkpointDict[nextCheckpoint] = i;
            checkpointArr[i] = nextCheckpoint;
        }
    }

    void Awake() {
        Instance = this;
        // is this the right place to put this?
        GetCheckpointList();
    }

    // Start is called before the first frame update
    void Start()
    {
        SaveStateManagerGameObject.SetupCheckpointManager();
        Respawn();
    }

    public static void ForceSetCurrentCheckpoint(int newCheckpoint) {
        Instance.currCheckpoint = Instance.checkpointArr[newCheckpoint];
    }

    public static void SetCurrentCheckpoint(int newCheckpoint) {
        
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