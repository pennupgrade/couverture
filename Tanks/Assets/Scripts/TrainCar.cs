// SPDX-FileCopyrightText: 2024 The Catanks Contributors
//
// SPDX-License-Identifier: MPL-2.0

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainCar : MonoBehaviour
{
	public GameObject car;
	public GameObject turret; 

    // Update is called once per frame
    void Update()
    {
        if (turret)
        {
        	if (Vector3.Distance(car.transform.position, turret.transform.position) > 0.5f)
        	{
        		turret.transform.position = car.transform.position + Vector3.up * 0.3f;
        	}
        } else
        {
        	Destroy(car);
        	Destroy(gameObject);
        }
    }
}
