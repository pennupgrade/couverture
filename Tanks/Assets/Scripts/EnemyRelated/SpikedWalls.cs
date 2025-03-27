using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikedWalls : MonoBehaviour
{
    bool reDealDamage = true;
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("START");
    }

    private void OnTriggerEnter(Collider collision)
    {
        Debug.Log(collision.tag);
        if(collision.tag == "Player")
        {
            Vector3 bounceBack = collision.transform.position - transform.position;
            //collision.transform.position += bounceBack;
            Debug.Log("Spiked");
            if(collision != null)
            {
                StartCoroutine("bounceBack", collision.gameObject);
            }
        }
    }

    private IEnumerator bounceBack(GameObject collision)
    {
        Vector3 bounceBack = collision.transform.position - transform.position;
        Debug.Log("Deals Damage.");
        Tank tank = collision.gameObject.GetComponent<Tank>();
        if(tank != null)
        {
            tank.takeDamage(5);
        }
        ParticleSystem ps = gameObject.GetComponentInChildren<ParticleSystem>();
        ps.gameObject.transform.position =
            gameObject.transform.position + bounceBack * 0.5f;
        ps.Play();
        float duration = 2f; // How long the bounce lasts
        float elapsedTime = 0f;

        while (elapsedTime <= duration)
        {
            float factor = Mathf.Lerp(1f, 0f, elapsedTime / duration);

            collision.gameObject.transform.position +=
            bounceBack * factor * 0.15f;
            yield return new WaitForSeconds(0.02f);
            elapsedTime += 0.02f;
        }
    }
}
