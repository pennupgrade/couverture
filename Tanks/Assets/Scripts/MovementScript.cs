// SPDX-FileCopyrightText: 2024 The Catanks Contributors
//
// SPDX-License-Identifier: MPL-2.0

using UnityEngine;

public class MovementScript : MonoBehaviour
{
    public Vector3 moveVector;

    // Update is called once per frame
    void Update()
    {
        transform.position += moveVector * Time.deltaTime;
    }
}
