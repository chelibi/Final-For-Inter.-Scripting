using System.Collections;
using UnityEngine;

public class SecurityCamera : MonoBehaviour
{
    [SerializeField] private float lookInterval = 0.1f;
    [Range(30,110)]
    [SerializeField] private float fieldOfView = 75;
    [SerializeField] private float rotationSpeed = 5f;
    private Transform rayEmitter;
    private GameObject[] playerObj;
    

    void Start()
    {
        rayEmitter = this.transform.GetChild(0);
        playerObj = GameObject.FindGameObjectsWithTag("Player");
        StartCoroutine(CheckForPlayerObj());
    }


    void Update()
    {
        
    }

     IEnumerator CheckForPlayerObj()
    {
        while (true)
        {
            yield return new WaitForSeconds(lookInterval);

            foreach (GameObject user in playerObj)
            {
                Ray ray = new Ray(rayEmitter.position, user.transform.position - rayEmitter.position);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, 100))
                {
                    if (hit.transform.gameObject.CompareTag("Player"))
                    {
                        Vector3 targetDir = user.transform.position - rayEmitter.position;
                        float angle = Vector3.Angle(targetDir, rayEmitter.forward);

                        if (angle < fieldOfView)
                        {
                            Debug.Log("Found player.");
                            Debug.DrawRay(rayEmitter.position, targetDir, Color.green, 4);

                            Quaternion targetRotation = Quaternion.LookRotation(targetDir);
                            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
                        }
                        else
                        {
                            Debug.DrawRay(rayEmitter.position, targetDir, Color.yellow, 4);
                        }
                    }
                    else
                    {
                        Debug.DrawRay(rayEmitter.position, user.transform.position - rayEmitter.position, Color.red, 4);
                    }
                }
            }
        }
    }
}