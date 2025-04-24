using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTracks : MonoBehaviour
{
    public GameObject tracksDecal;
    public float trackOffset;
    void Start() {
        StartCoroutine(SpawnTracks());
    }

    private IEnumerator SpawnTracks()
    {
        while (true)
        {
            yield return new WaitForSeconds(trackOffset);
            RaycastHit info;
            if (Physics.Raycast(transform.position, Vector3.down, out info, 1f, 1 << 3))
            {
                Instantiate(tracksDecal, transform.position, transform.rotation, info.transform);

            }
        }
    }
}
