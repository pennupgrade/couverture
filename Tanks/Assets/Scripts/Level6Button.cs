// SPDX-FileCopyrightText: 2024 The Catanks Contributors
//
// SPDX-License-Identifier: MPL-2.0

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level6Button : ActivatorButton
{
    [SerializeField] int hitsToActivate = 0;
    [SerializeField] Boss b;
    private int hits = 0;
    public GameObject[] blinkers;
    public float[] angleDeltas;
    private float angleDelta;

    protected override void OnTriggerEnter(Collider other)
    {
        if (!onCooldown && other.transform.tag == "Projectile" && hits < hitsToActivate)
        {
            StartCoroutine(transformButton());
            ActivateBlinker(hits);
            hits++;
            Debug.Log("smacked");
        } else
        {
            //StartCoroutine(cooldownTimer());
        }
    }

    private void ActivateBlinker(int id)
    {
        if (id >= blinkers.Length) return;
        if (blinkers[id] == null) return;

        GameObject blinker = blinkers[id];
        AudioManager manager = blinker.GetComponent<AudioManager>();
        PitchShiftEffect pitchEffect = blinker.GetComponent<PitchShiftEffect>();

        if (pitchEffect)
        {
            pitchEffect.shiftDelta = 0.1875f * hits - 0.5f;
        }
        if (manager)
        {
            manager.Play("Ding");
        }

        blinker.GetComponent<BlinkerScript>().Activate();
    }

    protected override IEnumerator transformButton()
    {
        Debug.Log("activate " + hits);
        onCooldown = true;

        float timer = 0;
        float delta = hits < angleDeltas.Length ? angleDeltas[hits] : angleDelta;

        Vector3 startPos = transform.localPosition;
        Quaternion originalRot = transform.localRotation;
        Quaternion newRot = Quaternion.AngleAxis(delta, rotateAxis) * originalRot;

        while (timer <= 1)
        {
            if (hits >= hitsToActivate)
            {
                transform.localPosition = Vector3.Lerp(startPos,
                    startPos + vOffset, timer);
                b.unplug();
            } else
            {
                transform.localPosition = Vector3.Lerp(startPos,
                    startPos + vOffset, Mathf.Sin(Mathf.PI * timer));
            }
            transform.localRotation = Quaternion.Lerp(originalRot, newRot, Mathf.Pow(timer, 0.5f));

            timer += Time.deltaTime / 0.6f;
            yield return null;
        }

        onCooldown = false;
        Debug.Log("Done");
    }

    protected override IEnumerator cooldownTimer()
    {
        onCooldown = true;
        //change button appearance
        yield return new WaitForSeconds(0.6f);
        //revert button appearance
        onCooldown = false;
    }
}
