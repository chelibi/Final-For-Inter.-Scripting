using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleporter : MonoBehaviour, IItem
{
    private Rigidbody rb;
    private Transform playerTransform;
    private Vector3 teleportDestination;
    private GameObject teleportMarker;
    private bool isMarkerSet = false; //Needed to add this to stop the prefab from spawning on Start
    private Camera mainCamera; //added reference to camera for raycast
    private float teleportHeightOffset = 1.0f; //added a height offset because I was falling through the floor on teleport

    private Animator animator; //added for animation assignment

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        teleportDestination = transform.position;
        mainCamera = Camera.main; //gets camera for raycast behavior

        animator = GetComponentInChildren<Animator>(); //added for animation assignment
    }

    public void Pickup(Transform hand)
    {
        Debug.Log("Picking up Teleporter");
        // make kinematic rigidbody
        rb.isKinematic = true;
        // move to hand and match rotation
        transform.SetParent(hand);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        // turn off collision so it doesn't push the player off the map
    }

    public void Drop()
    {
        Debug.Log("Dropping Teleporter");
        // make dynamic rigidbody
        rb.isKinematic = false;
        // throw it away from the player
        rb.AddRelativeForce(Vector3.forward * 10, ForceMode.Impulse);
        // set this parent to null
        transform.SetParent(null);
    }

    public void PrimaryAction()
    {
        Debug.Log("Teleporting to destination");
        // Primary Action: Teleport to marked location
        //added math n stuff to add a height offset so I would stop teleporting through the floor.
        Vector3 adjustedDestination = new Vector3 (teleportDestination.x, teleportDestination.y + teleportHeightOffset, teleportDestination.z);
        
        playerTransform.position = adjustedDestination; //changed teleportDestination to adjustedDestination

        animator.SetTrigger("Teleport"); //added for animation assignment
    }

    public void SecondaryAction()
    {
        Debug.Log("Setting new teleport destination");
        RaycastHit hit; //new raycast stuff
        if (Physics.Raycast(mainCamera.transform.position, mainCamera.transform.forward, out hit))
        {
            SetTeleportDestination(hit.point);
        }

        animator.SetTrigger("SetMarker"); //added for animation assignment
    }

    //polish
    private void SetTeleportDestination(Vector3 newDestination)
    {
        teleportDestination = newDestination;

        if (!isMarkerSet)
        {
            //Changed logic to find the prefab instead of drag and drop
            GameObject Booth = GameObject.Find("Booth");
            if (Booth != null)
            {
                teleportMarker = Instantiate(Booth, newDestination, Quaternion.identity);
                isMarkerSet = true;
            }
            else
            {
                Debug.LogError("Booth object not found in the scene.");
            }
        }
        else if (teleportMarker != null)
        {
            teleportMarker.transform.position = newDestination;
        }
    }
}
