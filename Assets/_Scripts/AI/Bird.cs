using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Bird : MonoBehaviour
{
    [SerializeField] private float mass = 1f;
    [SerializeField] private float forwardDistance = 10f;
    [SerializeField] private float safeDistance = 12f;
    [SerializeField] private float flySpeed = 1f;
    [SerializeField] private float flapCooldown = 0.4f;
    [SerializeField] private float flapModifier = 2f;
    [SerializeField] private float flapVelocity = 5f;
    [SerializeField] private float rotationSpeed = 5f;


    // Lift up
    // Weight down
    // Drag back
    // Thrust forward


    public Vector3 velocity;

    private void Start()
    {
        velocity = new Vector3(0f, 0f, 1f);
        StartCoroutine(Flap());
    }
    private void Update()
    {
        velocity.y -= 9.81f * mass * Time.deltaTime; // weight

        transform.position += velocity * Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.V))
        {
            Quaternion rotation = Quaternion.AngleAxis(90f, new Vector3(0, 1, 0));
            Vector3 rotatedVector = rotation * velocity;
            velocity = rotatedVector;
        }
    }
    private void FixedUpdate()
    {
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(velocity.normalized, Vector3.up), rotationSpeed * Time.fixedDeltaTime);
        if (!Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, forwardDistance)) return;
        
    }
    private IEnumerator Flap()
    {
        while (true)
        {
            float distanceToGround;
            if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit))
            {
                if(hit.point.y < 0f) // water check
                {
                    distanceToGround = Vector3.Distance(transform.position, new Vector3(transform.position.x, 0f, transform.position.z));
                }
                else distanceToGround = hit.distance;
            }
            else
            {
                distanceToGround = 0f;
            }
            float perc = distanceToGround / safeDistance;
            yield return new WaitForSeconds(flapCooldown * perc);

            //velocity += transform.forward * flapModifier;
            if(velocity.y < -1f) velocity.y += flapVelocity;
        }
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * forwardDistance);
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * safeDistance);
    }
}
