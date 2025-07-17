// SPDX-FileCopyrightText: 2024 The Catanks Contributors
//
// SPDX-License-Identifier: MPL-2.0

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargeIndicator : MonoBehaviour
{
    [SerializeField] float windUpTime;
    [SerializeField] GameObject indicatorObject;
    [SerializeField] float holdDur;
    [SerializeField] SpriteRenderer sr;

    private float spawnTime;
    private float thisLength;
    
    // Start is called before the first frame update
    void Start()
    {
        spawnTime = Time.time;
        thisLength = this.transform.localScale.y;
    }

    // Update is called once per frame
    void Update()
    {
        float timeElapsed = Time.time - spawnTime;
        if (Time.time < spawnTime + windUpTime) {
            indicatorObject.transform.localScale = new Vector3(1, -1 * timeElapsed * (1/windUpTime), 1); 
            //indicatorObject.transform.position = new Vector3(0, 0, thisLength * (0.5f - (timeElapsed /(2*windUpTime)))) + this.transform.position;
        } else { 
            Color og = sr.color;
            print(og);
            sr.color = new Color(og.r, og.g, og.b, 0.8f);
        }
        if (Time.time > spawnTime + windUpTime + holdDur) {
            Destroy(this.gameObject);
        }
    }
}
