using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using UnityEditor.UI;
using UnityEngine;

public class Bubble : MonoBehaviour, IDestroyable
{
    public Transform targetTransform;

    public void incapacitate(float time)
    {
        // Destroy(gameObject);
    }

    public void takeDamage(int dmg)
    {
        // Debug.Log("Taking Damage As Shield");
        // Destroy(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.position = targetTransform.position;
    }

    private void OnCollisionEnter(Collision other) {
        // if (other.gameObject.layer == LayerMask.NameToLayer("Enemy")) {
        //     Debug.Log("Hit other tank");
        //     Destroy(gameObject);
        // }
    }
}
