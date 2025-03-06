using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAndCamera : MonoBehaviour
{
    public static PlayerAndCamera Instance = null;
    [SerializeField] private Camera camera;
    [SerializeField] private Tank tank;

    void Awake() {
        if (Instance is null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Instance.transform.position = transform.position;
            Instance.transform.rotation = transform.rotation;
            Instance.tank.transform.position = tank.transform.position;
            Instance.tank.transform.rotation = tank.transform.rotation;
            Instance.camera.transform.position = camera.transform.position;
            Instance.camera.transform.rotation = camera.transform.rotation;
            Instance.tank.Unfreeze();
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
