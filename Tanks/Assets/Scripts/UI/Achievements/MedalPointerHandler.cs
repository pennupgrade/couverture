// SPDX-FileCopyrightText: 2024 The Catanks Contributors
//
// SPDX-License-Identifier: MPL-2.0

using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class MedalPointerHandler : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public Action HandlePointerClick { private get; set; }
    public Action HandlePointerEnter { private get; set; }
    public Action HandlePointerExit { private get; set; }

    public void OnPointerClick(PointerEventData eventData) => HandlePointerClick();
    public void OnPointerEnter(PointerEventData eventData) => HandlePointerEnter();
    public void OnPointerExit(PointerEventData eventData) => HandlePointerExit();
}
