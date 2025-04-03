using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FanColliderScript : MonoBehaviour
{
    FanScript fanScript;
    
    // Start is called before the first frame update
    void Start()
    {
        fanScript = transform.parent.GetComponent<FanScript>();
    }

    private void OnTriggerStay(Collider collider)
    {
        bool isEnemyTank = collider.tag == "Tank" && fanScript.canEffectEnemy && !collider.gameObject.name.Equals("Boss");
        bool isPlayer = collider.tag == "Player";

        if ((isPlayer || isEnemyTank) && fanScript.active)
        {
            fanScript.pushPlayer(collider);
        }
    }
}
