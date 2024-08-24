using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FadeOutSequence : MonoBehaviour
{
    [SerializeField] private Vector3 vOffset = new Vector3(0, -0.4f, 0);
    [SerializeField] private GameObject FadeOutObject;

    private Image FadeOutPanel;
    private bool onCooldown;
    private bool canReset;

    public float transitionTime;
    public string nextScene;

    private void Awake()
    {
        if (FadeOutObject == null) return;

        GameObject fadePanelObject = FadeOutObject.transform.Find("Panel").gameObject;

        if (fadePanelObject == null) return;

        FadeOutPanel = fadePanelObject.GetComponent<Image>();
    }

    private void Update()
    {
        if (canReset && Input.GetKeyDown(KeyCode.Space))
        {
            RestartGame();
        }
    }

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
            tank.Dissolve(transitionTime);
            tank.moveSpeed = 0;
            tank.health = 1000000; // i am just too lazy to make you invincible when you transition levels
        }
        
        //GameManager.Instance.GoToSubLevel(transitionTime, nextScene);
        if (!FadeOutObject.activeSelf)
        {
            FadeOutObject.SetActive(true);
        }

        StartCoroutine(SlideButtonDown());
        onCooldown = true; // debounce
    }

    public void RestartGame()
    {
        GameManager.Instance.RestartLevel();
    }

    private IEnumerator SlideButtonDown()
    {
        onCooldown = true;
        float timer = 0;
        Vector3 startPos = transform.localPosition;

        bool doesFadeOutExist = (FadeOutPanel != null);

        while (timer <= transitionTime + 0.75f)
        {
            transform.localPosition = Vector3.Lerp(startPos,
                startPos + vOffset, timer);
            timer += Time.deltaTime;

            if (doesFadeOutExist)
            {
                float alpha = timer / (transitionTime);
                FadeOutPanel.color = new Color(0, 0, 0, alpha);
            }

            yield return null;
        }

        if (doesFadeOutExist)
        {
            FadeOutPanel.color = new Color(0, 0, 0, 1);

            GameObject ThankYouObject = FadeOutObject.transform.Find("Thank you").gameObject;

            
            if (ThankYouObject != null)
            {
                ThankYouObject.SetActive(true);

                string thankYouMessage = "Thanks for playing! - UPGRADE";
                string nextMessage = "Press spacebar to play again";
                string text = "";

                float time = 0;
                int i = 0;

                TextMeshProUGUI textMesh = ThankYouObject.transform.Find("Text").gameObject.GetComponent<TextMeshProUGUI>();
                TextMeshProUGUI textMesh2 = ThankYouObject.transform.Find("Text2").gameObject.GetComponent<TextMeshProUGUI>();

                while (text.Length != thankYouMessage.Length)
                {
                    Debug.Log("jiwubyfuweif");
                    if (time > 0.05f)
                    {
                        text += thankYouMessage[i];
                        textMesh.text = text;
                        i++;
                        time = 0;
                    }

                    time += Time.deltaTime;
                    yield return null;
                }

                textMesh.text = text;
                text = "";

                // 3am code hitting hard

                time = -0.50f;
                i = 0;

                while (text.Length != nextMessage.Length)
                {
                    Debug.Log("jiwubyfuweif");
                    if (time > 0.05f)
                    {
                        text += nextMessage[i];
                        textMesh2.text = text;
                        i++;
                        time = 0;
                    }

                    time += Time.deltaTime;
                    yield return null;
                }

                textMesh2.text = text;

                canReset = true;
            }
        }       
    }
}
