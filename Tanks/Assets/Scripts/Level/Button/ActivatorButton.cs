using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivatorButton : MonoBehaviour
{
    [SerializeField] protected Vector3 vOffset = new Vector3(0, -2, 0);
    [SerializeField] protected float rotateDelta = 0;
    [SerializeField] protected Vector3 rotateAxis = new Vector3(0, 0, 0);
    [SerializeField] protected float pressSpeed = 0.8f;
    [SerializeField] protected bool disableRotation;

    public bool reusable;
    protected bool onCooldown;
    public GameObject[] toChange;
    public AudioManager audioManager;

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (!onCooldown && other.transform.tag == "Player")
        {
            buttonPressed();
            audioManager?.Play("Press");
        }
    }
    protected void buttonPressed() {
        foreach (GameObject g in toChange) {
            if (g.TryGetComponent<Activatable>(out Activatable aObj)) {
                aObj.activate();
            } else {
                //for spawning enemies
                g.gameObject.SetActive(!g.activeSelf);
            }
        }

        if (!reusable) {
            StartCoroutine(transformButton());
        } else {
            StartCoroutine(cooldownTimer());
        }
    }

    protected virtual IEnumerator transformButton() {
        onCooldown = true;
        float timer = 0;
        Vector3 startPos = transform.localPosition;
        Quaternion originalRot = transform.localRotation;
        Quaternion newRot = Quaternion.AngleAxis(rotateDelta * Mathf.PI / 180.0f, rotateAxis);

        while (timer <= 1) {
            transform.localPosition = Vector3.Lerp(startPos, 
                startPos + vOffset, timer);
            if (!disableRotation) transform.localRotation = Quaternion.Lerp(originalRot, newRot, timer);

            timer += Time.deltaTime * pressSpeed;
            yield return null;
        }

        // gameObject.SetActive(false);
    }


    protected virtual IEnumerator cooldownTimer() {
        onCooldown = true;
        //change button appearance
        yield return new WaitForSeconds(0.6f);
        //revert button appearance
        onCooldown = false;
    }

}
