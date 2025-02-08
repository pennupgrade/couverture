using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum EffectsOnDestroy
{
    SpeedBoost,
    DamageBoost,
    ReloadSpeed,
    AddHealth,
    None
};

public class destructableScript : MonoBehaviour
{
    public Material red;
    public Material green;
    private DamageFlash damageFlash;
    public GameObject Body; // IK i told you that you can just put this.gameObject into DamageFlash, but decide if you're gonna keep this cause rn it's unused - Anthony
    // maybe add a counter in inspector to denote # of bullets the obj can take before it breaks - Anthony
    [SerializeField]
    public EffectsOnDestroy effectsOnDestroy = EffectsOnDestroy.None;
    [SerializeField]
    float speedBoostCoolDown = 5;
    [SerializeField]
    float speedBoostMagnitude = 5;
    [SerializeField]
    float damageBoostCoolDown = 5;
    [SerializeField]
    float damageBoostMagnitude = 5;
    [SerializeField]
    float reloadSpeedCoolDown = 5;
    [SerializeField]
    float reloadSpeedScale = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        damageFlash = new DamageFlash(this.gameObject);
    }

    void destroy(GameObject parent)
    {
        Debug.Log("Destroy");
        addEffectOnDestruction(parent);
        Destroy(this.gameObject, 0.25f);
    }

    private void addEffectOnDestruction(GameObject parent)
    {
        TimedEffect effectActions = null;
        switch (effectsOnDestroy)
        {
            case EffectsOnDestroy.SpeedBoost:
                effectActions =
                    new TimedEffect(speedBoostCoolDown,
                    (tank) => { tank.moveSpeed += speedBoostMagnitude;
                        Debug.Log("Speed Boost Start");
                    },
                    (tank) => { tank.moveSpeed -= speedBoostMagnitude;
                        Debug.Log("Speed Boost End");
                    }  );
                break;
            case EffectsOnDestroy.DamageBoost:
                effectActions =
                    new TimedEffect(damageBoostCoolDown,
                    (tank) => {
                        tank.isDamageBoosting = true;
                        Debug.Log("Damage Boost Start"); },
                    (tank) => {
                        tank.isDamageBoosting = false;
                        Debug.Log("Damage Boost End"); }); //don't have access to damage yet 
                break;
            case EffectsOnDestroy.ReloadSpeed:
                effectActions =
                                   new TimedEffect(reloadSpeedCoolDown,
                                   (tank) => { Tank.RELOAD_TIME *= reloadSpeedScale;
                                       Debug.Log("Reload Speed Start");
                                   },
                                   (tank) => { Tank.RELOAD_TIME /= reloadSpeedScale;
                                       Debug.Log("Reload Speed End");
                                   });
                break;
            case EffectsOnDestroy.AddHealth:
                effectActions =
                                   new TimedEffect(1,
                                   (tank) => { tank.health += 100;
                                       Debug.Log("Add Health");
                                   }, //not sure if we're doing it like this?
                                   (tank) => {
                                       Debug.Log("Health End (No Effect)");
                                   }); // not deducting the added health back
                break;
            case EffectsOnDestroy.None:
                break;
        }
        if(effectsOnDestroy != EffectsOnDestroy.None && effectActions != null)
        {
            Tank player = parent.GetComponent<Tank>();
            if (player != null)
                player.addEffect(effectActions);
        }
    }
    
    private void OnCollisionEnter(Collision collision)
    {
        Projectile bullet = collision.gameObject.GetComponent<Projectile>(); // how can we handle other bullets differently too

        if(bullet != null)
        {
            // what if parent don't exist, or like say parent is an enemy tank but you kill that tank - Anthony
            // just make sure you cover that if you still want the below functionalities in the if branches
            GameObject parent = bullet.parent; 
            if(parent.tag == "Player")
            {
                Debug.Log("here");
                damageFlash.CallDamageFlash(this);
                destroy(parent);
            }
            else if (parent.tag == "Tank")
            {
                Debug.Log("red");
                this.GetComponent<Renderer>().material = red;
            }
        }
    }
}
