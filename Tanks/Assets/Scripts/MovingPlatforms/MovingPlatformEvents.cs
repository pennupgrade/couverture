// SPDX-FileCopyrightText: 2024 The Catanks Contributors
//
// SPDX-License-Identifier: MPL-2.0

using PathCreation;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class MovingPlatformEvents : MonoBehaviour
{
    public MovingPlatform movingPlatform;

    public VertexPath vertexPath;

    public UnityEvent OnNextIndex;

    public int eventIndex;

    public bool canEvent;

    private void OnEnable()
    {
        canEvent = true;
    }

    private void Update()
    {
        if (eventIndex != movingPlatform.nextIndex)
        {
            return;
        }

        if (Vector3.Distance(movingPlatform.transform.position, vertexPath.GetMainPos(eventIndex)) < 0.02f)
        {
            OnNextIndex.Invoke();

            canEvent = false;
        }
    }
}
