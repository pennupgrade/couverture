using PathCreation;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainSpawner : MonoBehaviour
{
	public VertexPath track;
	public GameObject trainCar; 
	public float speed;
	public float delay;
	public int count;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Spawn());
    }

    IEnumerator Spawn()
    {
    	for (int i = 0; i < count; i++)
    	{
            GameObject trainCarCopy = Instantiate(trainCar);
            trainCarCopy.SetActive(true);

            MovingPlatformRotating plat = trainCarCopy.GetComponentInChildren<MovingPlatformRotating>();
    		plat.speed = speed;
    		plat.vertexPath = track;
    		yield return new WaitForSeconds(delay);
    	}
    }


}
