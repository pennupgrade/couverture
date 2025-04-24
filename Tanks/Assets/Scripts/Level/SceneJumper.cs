using System.Collections;
using UnityEngine;
using TransitionType = SceneTransition.TransitionType;

public class SceneJumper : MonoBehaviour
{
    [SerializeField] private Vector3 vOffset = new(0, -0.4f, 0);
    [SerializeField] private TransitionType transitionTypeToUse = TransitionType.Level;

    private bool onCooldown;

    public float transitionTime;
    public string nextScene;

    protected void OnTriggerEnter(Collider other) {
        if (!onCooldown && other.transform.CompareTag("Player")) {
            ButtonPressed(other);
        }
    }

    private void ButtonPressed(Collider playerCollider) {
        var tank = playerCollider.gameObject.GetComponent<Tank>();
        if (tank) {
            tank.FreezeEndOfLevel();
        }

        GameManager.Instance.GoToNextLevel(transitionTime, nextScene, transitionTypeToUse);

        StartCoroutine(SlideButtonDown());
        onCooldown = true; // debounce
    }

    private IEnumerator SlideButtonDown() {
        onCooldown = true;
        float timer = 0;
        var startPos = transform.localPosition;
        while (timer <= transitionTime) {
            transform.localPosition = Vector3.Lerp(startPos,
                                                   startPos + vOffset, timer);
            timer += Time.deltaTime * 0.3f;
            yield return null;
        }

        gameObject.SetActive(false);
    }
}