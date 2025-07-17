// SPDX-FileCopyrightText: 2024 The Catanks Contributors
//
// SPDX-License-Identifier: MPL-2.0

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleScript : MonoBehaviour
{
    ParticleSystem particle;
    Vector3 initRot;
    // Start is called before the first frame update
    void Start()
    {
        particle = GetComponent<ParticleSystem>();
        initRot = transform.rotation.eulerAngles;
        Debug.Log(initRot);

    }

    // Update is called once per frame
    void Update()
    {
        ParticleSystem.MainModule test = particle.main;
        //test.startRotationX = (((transform.rotation.eulerAngles[0] - initRot[0])*Mathf.PI/180));
        test.startRotationZ = (((transform.rotation.eulerAngles[1] - initRot[1]) * Mathf.PI / 180));
        //test.startRotationY = (((transform.rotation.eulerAngles[2] - initRot[2]) * Mathf.PI / 180));

    }
}
