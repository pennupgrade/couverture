// SPDX-FileCopyrightText: 2024 The Catanks Contributors
//
// SPDX-License-Identifier: MPL-2.0

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointManagerV2 : MonoBehaviour
{
    public static CheckpointManagerV2 Instance = null;

    private readonly Dictionary<Checkpoint, int> checkpointDict = new();

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
        SetUpCheckpoints();

        if (GetCurrentCheckpointLocation() is Vector3 loc) {
            GameObject p = GameObject.FindWithTag("Player+Camera");
            p.transform.position = loc + p.transform.position - Tank.FindPlayer().transform.position;
        }
    }

    public static void CheckpointActivated(Checkpoint c) {
        if (!Instance.checkpointDict.ContainsKey(c)) {
            throw new InvalidOperationException();
        }
        SaveStateManagerGameObject.UnlockCheckpoint(Instance.checkpointDict[c]);
    }

    public static Vector3? GetCurrentCheckpointLocation() {
        if (SaveStateManagerGameObject.GetCurrentCheckpoint() is int checkpointIndex) {
            return Instance.transform.GetChild(checkpointIndex).gameObject.transform.position;
        }
        return null;
    }
}
