using UnityEngine;
using UnityEngine.EventSystems;

public class MedalPointerHandler : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log($"clicked: {gameObject.name}");
    }
}
