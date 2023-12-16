using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : MonoBehaviour
{
    [SerializeField] private float lookInterval = 0.01f;
    [SerializeField] private float fireInterval = 0.5f; // Interval between shots
    [Range(30, 110)]
    [SerializeField] private float fieldOfView = 60;
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private float projectileSpeed = 20f;

    private Transform rayEmitter;
    public GameObject[] playerObj;
    private Rigidbody playerRigidbody;
    private bool playerInSight = false;
    private GameObject currentTarget;
    private Animator turretAnimator; // Reference to the Animator component

    void Start()
    {
        turretAnimator = GetComponent<Animator>(); // Assign the Animator component
        rayEmitter = this.transform.GetChild(0);
        playerObj = GameObject.FindGameObjectsWithTag("Player");

        if (playerObj.Length > 0)
        {
            playerRigidbody = playerObj[0].GetComponent<Rigidbody>();
        }

        StartCoroutine(CheckForPlayerObj());
        StartCoroutine(FireAtIntervals());
    }

    IEnumerator CheckForPlayerObj()
    {
        while (true)
        {
            yield return new WaitForSeconds(lookInterval);
            playerInSight = false;

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

                            playerInSight = true;
                            currentTarget = user;

                            if (!turretAnimator.GetBool("IsEmerged"))
                            {
                                turretAnimator.SetBool("IsEmerged", true); // Trigger emerge animation
                            }
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

            // Set to idle or active state based on whether it's firing or not
            turretAnimator.SetBool("IsIdle", !playerInSight);
        }
    }

    IEnumerator FireAtIntervals()
    {
        while (true)
        {
            yield return new WaitForSeconds(fireInterval);
            if (playerInSight)
            {
                FireAtPredictedPosition(currentTarget);
            }
        }
    }

    void FireAtPredictedPosition(GameObject target)
    {
        Vector3 predictedPosition = CalculatePredictedPosition(target);
        GameObject projectile = TurretPool.Instance.GetPooledObject(); // Make sure TurretPool is also attached
        if (projectile != null)
        {
            projectile.transform.position = rayEmitter.position;
            projectile.transform.rotation = Quaternion.LookRotation(predictedPosition - rayEmitter.position);
            projectile.SetActive(true);

            Rigidbody projectileRigidbody = projectile.GetComponent<Rigidbody>();
            if (projectileRigidbody == null)
            {
                projectileRigidbody = projectile.AddComponent<Rigidbody>();
                projectileRigidbody.useGravity = false;
            }
            projectileRigidbody.velocity = (predictedPosition - rayEmitter.position).normalized * projectileSpeed;

            StartCoroutine(DeactivateProjectileAfterDelay(projectile, 3f));

            // Trigger recoil animation
            turretAnimator.SetTrigger("Recoil");
        }
    }

    IEnumerator DeactivateProjectileAfterDelay(GameObject projectile, float delay)
    {
        yield return new WaitForSeconds(delay);
        projectile.SetActive(false);
    }

    Vector3 CalculatePredictedPosition(GameObject target)
    {
        float distance = Vector3.Distance(target.transform.position, rayEmitter.position);
        float travelTime = distance / projectileSpeed;
        Vector3 predictedPosition = target.transform.position + playerRigidbody.velocity * travelTime;
        return predictedPosition;
    }
}
