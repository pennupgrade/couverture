using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BossDeath : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private GameObject bossExplosion;
    [SerializeField] private Camera bossDeathCam;
    [SerializeField] private Camera doorOpenCam;
    [SerializeField] private SlidingWall exitWall;
    [SerializeField] private GameObject bossTank;

    public Vector3 offSet;
    private GameObject player;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        if (bossTank != null) {
            bossDeathCam.transform.position = bossTank.transform.position + offSet;
        }
    }

    public void StartDeathScene() {
        StartCoroutine(BossDeathCam());
    }

    private IEnumerator BossDeathCam() {
        Camera mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        player.GetComponent<Tank>().enableGod = true;
        bossDeathCam.enabled = true;
        mainCamera.enabled = false;
        doorOpenCam.enabled = false;
        bossTank.GetComponent<BossMovement>().enabled = false;
        bossTank.GetComponent<NavMeshAgent>().speed = 0;
        bossTank.GetComponent<BossStateMachine>().enabled = false;
        bossTank.GetComponent<Boss>().enabled = false;
        yield return new WaitForSeconds(.5f);

        bossTank.GetComponent<Animator>().SetTrigger("Stun");
        
        yield return new WaitForSeconds(1.5f);
        Instantiate(bossExplosion, bossTank.transform.position, Quaternion.identity);
        Destroy(bossTank);
        
        //Destroy(gameObject);
        yield return new WaitForSeconds(2f);

        doorOpenCam.enabled = true;
        bossDeathCam.enabled = false;
        mainCamera.enabled = false;
        
        yield return new WaitForSeconds(1f);

        exitWall.activate();

        yield return new WaitForSeconds(2f);

        doorOpenCam.enabled = false;
        bossDeathCam.enabled = false;
        mainCamera.enabled = true;
        player.GetComponent<Tank>().enabled = true;
    }
}
