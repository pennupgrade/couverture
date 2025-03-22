using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTipTrigger : MonoBehaviour
{
    private TipArea currentTip;
    private string currentTipText;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<TipArea>(out TipArea tip))
        {
            if (currentTip == null || !currentTip.Equals(tip))
            {
                currentTip = tip;
                currentTipText = currentTip.GetTipText();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        currentTip = null;
        currentTipText = "";
    }

    public bool CheckTipActive()
    {
        return currentTip != null;
    }

    public string GetTipText()
    {
        if (!CheckTipActive())
            return null;

        return currentTipText;
    }

    public Vector3 GetTipWorldPosition()
    {
        if (!CheckTipActive())
            return Vector3.zero;
        
        return currentTip.transform.position;
    }
}
