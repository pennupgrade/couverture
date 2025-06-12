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
