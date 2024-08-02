using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Charles you LOuusssyy mf get your dork Ass down to the test chamber  or else i will shove the Sample up your a
// you STOP fuking with ethe microwave
// https://www.youtube.com/watch?v=vuhOfzUCL7A

public class TempCameraUntilCharlesDoesHisCrap : MonoBehaviour
{
    public GameObject Player;
    public float speed = 1.0f;

    [Range(-30.0f, 30.0f)] public float offsetValueX;
    [Range(-30.0f, 30.0f)] public float offsetValueZ;
    [Range(0.0f, 60.0f)] public float distanceThreshold;

    // Update is called once per frame
    void Update()
    {
        GameObject Body = Player.gameObject.transform.Find("Body").gameObject;

        if (Body == null)
        {
            return;
        }

        Vector3 playerPosition = Body.transform.position;
        Vector3 cameraPosition = Camera.main.transform.position;

        Vector3 offset = new Vector3(offsetValueX, 7.0f, offsetValueZ);
        Vector3 targetPosition = playerPosition + offset;

        float distance = Vector3.Distance(cameraPosition, targetPosition);
        if (distance > distanceThreshold )
        {
            //targetPosition.x = playerPosition.x;
            //targetPosition.z = playerPosition.z;
            Camera.main.transform.position = Vector3.Lerp(cameraPosition, targetPosition, speed * Time.deltaTime);
        }

        Debug.Log(distance);

    }
}
