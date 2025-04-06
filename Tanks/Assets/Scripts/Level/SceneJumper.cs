using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneJumper : MonoBehaviour
{
    [SerializeField] private Vector3 vOffset = new Vector3(0, -0.4f, 0);
    private bool onCooldown;

    public float transitionTime;
    public string nextScene;

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (!onCooldown && other.transform.tag == "Player")
        {
            ButtonPressed(other);
        }
    }
    protected void ButtonPressed(Collider playerCollider)
    {
        Tank tank = playerCollider.gameObject.GetComponent<Tank>();
        if (tank)
        {
            tank.Freeze();
        }
        
        GameManager.Instance.GoToNextLevel(transitionTime, nextScene);

        StartCoroutine(SlideButtonDown());
        onCooldown = true; // debounce
    }

    private IEnumerator SlideButtonDown()
    {
        onCooldown = true;
        float timer = 0;
        Vector3 startPos = transform.localPosition;
        while (timer <= transitionTime)
        {
            transform.localPosition = Vector3.Lerp(startPos,
                startPos + vOffset, timer);
            timer += Time.deltaTime * 0.3f;
            yield return null;
        }
        gameObject.SetActive(false);
    }
}
