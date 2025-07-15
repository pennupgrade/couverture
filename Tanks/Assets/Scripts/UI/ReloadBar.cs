// SPDX-FileCopyrightText: 2024 The Catanks Contributors
//
// SPDX-License-Identifier: MPL-2.0

using UnityEngine;
using UnityEngine.UI;

public class ReloadBar : MonoBehaviour
{
    [SerializeField] private Tank tank;
    [SerializeField] private Slider slider;

    // Update is called once per frame
    private void Update() {
        slider.value = tank.reloadProgress / Tank.RELOAD_TIME;
    }
}