using UnityEngine;

public class MovementScript : MonoBehaviour
{
    public Vector3 moveVector;

    // Update is called once per frame
    void Update()
    {
        transform.position += moveVector * Time.deltaTime;
    }
}
