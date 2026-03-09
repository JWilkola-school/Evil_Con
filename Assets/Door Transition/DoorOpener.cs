using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorOpener : MonoBehaviour
{
    public float distanceToOpen = 2;
    public Transform cam;

    RaycastHit hit;
    Door activeDoor;

    void Update()
    {
        bool isHit = Physics.Raycast(transform.position, transform.forward, out hit, distanceToOpen);

        if(isHit && hit.transform.tag == "Door")
        {
            Door doorHit = hit.transform.GetComponent<Door>();

            if(doorHit != activeDoor)
            {
                if(activeDoor != null)
                {
                    activeDoor.Deemphasize();
                }

                activeDoor = doorHit;
                activeDoor.Emphasize();
            }

            if (Input.GetButtonDown("Fire1") && Vector3.Dot(cam.forward, activeDoor.transform.forward) > 0)
            {
                activeDoor.Open();
            }
        }
        else if(activeDoor != null)
        {
            activeDoor.Deemphasize();
            activeDoor = null;
        }
    }
}
