// SPDX-FileCopyrightText: 2024 The Catanks Contributors
//
// SPDX-License-Identifier: MPL-2.0

using System.Collections;
using UnityEngine;

public class SpikedWalls : MonoBehaviour
{
    private void OnTriggerEnter(Collider collision) {
        Debug.Log(collision.tag);
        if (collision.tag == "Player") {
            Debug.Log("Spiked");
            if (collision != null) {
                StartCoroutine(nameof(bounceBack), collision.gameObject);
            }
        }
    }

    private IEnumerator bounceBack(GameObject collision) {
        var bounceBack = collision.transform.position - transform.position;
        Debug.Log("Deals Damage.");
        var tank = collision.gameObject.GetComponent<Tank>();
        if (tank != null) {
            tank.takeDamage(5);
        }

        var ps = gameObject.GetComponentInChildren<ParticleSystem>();
        ps.gameObject.transform.position =
            gameObject.transform.position + bounceBack * 0.5f;
        ps.Play();
        var duration = 2f; // How long the bounce lasts
        var elapsedTime = 0f;

        while (elapsedTime <= duration) {
            var factor = Mathf.Lerp(1f, 0f, elapsedTime / duration);

            collision.gameObject.transform.position += bounceBack * (factor * 0.15f);
            yield return new WaitForSeconds(0.02f);
            elapsedTime += 0.02f;
        }
    }
}