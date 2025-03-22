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
