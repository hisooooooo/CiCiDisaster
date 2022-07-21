using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCameraController : MonoBehaviour
{

    [SerializeField] Camera cam;
    [SerializeField] private Transform player;
    [SerializeField] private float aheadDistance;
    [SerializeField] private float cameraSpeed;
    private float lookAhead;
    public float yAdjust;
    private float zoomDistance;

    
    [SerializeField] InputHandler _input;

    private void Awake()
    {
        cam = GetComponent<Camera>();
        zoomDistance = cam.orthographicSize;
    }
    private void Update()
    {


        transform.position = new Vector3(player.position.x + lookAhead, player.position.y + yAdjust, transform.position.z);
        lookAhead = Mathf.Lerp(lookAhead, (aheadDistance * player.localScale.x), Time.deltaTime * cameraSpeed);
    
        if (_input.MouseWheelInput > 0 && cam.orthographicSize > 1)
        {
            zoomDistance -= 1;
        }
        if (_input.MouseWheelInput < 0)
        {
            zoomDistance += 1;
        }

        cam.orthographicSize = zoomDistance;
       
    }


}
