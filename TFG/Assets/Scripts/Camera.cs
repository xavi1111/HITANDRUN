using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera : MonoBehaviour
{
    [SerializeField] Player player;
    Transform cameraTransform;
    // Start is called before the first frame update
    void Start()
    {
        cameraTransform = GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        if (player != null) { 
            Vector3 newCameraPosition = new Vector3(player.GetComponent<Transform>().position.x, player.GetComponent<Transform>().position.y + 1f, cameraTransform.position.z); 
            cameraTransform.position = newCameraPosition;
        }
    }
}
