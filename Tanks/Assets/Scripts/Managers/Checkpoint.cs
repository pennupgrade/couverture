// SPDX-FileCopyrightText: 2024 The Catanks Contributors
//
// SPDX-License-Identifier: MPL-2.0

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [SerializeField] private GameObject checkpointFlag;
    [SerializeField] private AudioManager audioManager;
    private MeshRenderer meshRenderer;
    private Material flagMaterial;
    private bool isActivated;

    public Animator animator;

    void OnTriggerEnter(Collider c) {
        Tank t = c.gameObject.GetComponent<Tank>();
        if (t is not null) {
            CheckpointFlagDing();
            CheckpointManagerV2.CheckpointActivated(this);
        }
    }

    void CheckpointFlagDing()
    {
        if (checkpointFlag == null || audioManager == null || isActivated) return;
        meshRenderer = checkpointFlag.GetComponent<MeshRenderer>();
        flagMaterial = meshRenderer.material;

        if (flagMaterial != null)
        {
            StartCoroutine(StatusAnimation());
            audioManager.Play("GoodSound");
        }

        isActivated = true;

        animator.SetBool("Stop", true);
    }

    private IEnumerator StatusAnimation()
    {
        float t = 0;
        float freq = 20.0f;

        // this magic number is -0.5pi
        // makes f(0) = 0
        float phase = -1.57079632679f;

        float limit = 3.0f * Mathf.PI / freq; 

        while (t < limit)
        {
            float status = 0.5f * Mathf.Sin(freq * t + phase) + 0.5f;
            flagMaterial.SetFloat("_Status", status);
            t += Time.deltaTime;

            yield return null;
        }

        flagMaterial.SetFloat("_Status", 1.0f);
        yield return null;
    }

}
