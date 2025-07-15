// SPDX-FileCopyrightText: 2024 The Catanks Contributors
//
// SPDX-License-Identifier: MPL-2.0

using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using UnityEngine;

public class Bubble : MonoBehaviour
{
    public Transform targetTransform;
    private Material shieldMat;
    void Start() {
        shieldMat = GetComponent<MeshRenderer>().material;
        shieldMat.SetFloat("_End_Alpha", -1);
        StartCoroutine(activateShield());
    }

    private IEnumerator activateShield() {
        Vector3 origScale = transform.localScale;
        float i = 0.1f;
        
        // Scale up
        while (i < 1) {
            float f = Mathf.SmoothStep(0.2f, 1.2f, i);
            transform.localScale = f * origScale;

            // Lerp _End_Alpha from -1 to 1
            float alpha = Mathf.Lerp(-1f, 1f, i);
            shieldMat.SetFloat("_End_Alpha", alpha);

            i += 8 * Time.deltaTime;
            yield return null;
        }

        i = 0.1f;

        // Scale down
        while (i < 1) {
            float f = Mathf.SmoothStep(1.2f, 1, i);
            transform.localScale = f * origScale;
            i += 25 * Time.deltaTime;
            yield return null;
        }

        transform.localScale = origScale;
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.position = targetTransform.position;
    }
 
    public void destroyShield() {
        StartCoroutine(deactivateShield());
    }
    
    private IEnumerator deactivateShield() {
        float i = 0;
        while (i < 1) {
            i += Time.deltaTime;

            // Lerp _End_Alpha from 1 back to -1
            float alpha = Mathf.Lerp(1f, -1f, i);
            shieldMat.SetFloat("_End_Alpha", alpha);


            yield return null;
        }

        Destroy(gameObject);
    }

}
