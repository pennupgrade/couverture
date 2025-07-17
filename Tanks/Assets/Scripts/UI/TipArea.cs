// SPDX-FileCopyrightText: 2024 The Catanks Contributors
//
// SPDX-License-Identifier: MPL-2.0

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TipArea : MonoBehaviour
{
	[SerializeField] [TextArea(2, 20)] private string tipDisplay;

	public string GetTipText()
	{
		return tipDisplay;
	}
}
