using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Guard : MonoBehaviour
{
    public static event System.Action OnGuardHasSpottedPlayer;
    public Transform pathHolder;
    public LayerMask viewMask;

    float spotTimer;
    public float timeToSpotPlayer = 0.5f;
    public float timeToSpotCloserPlayer = 0.15f;
    public float inFrontOfGuard = 0.05f;
   
    float playerVisibleTimer;
    public float speed = 5f;
    float waitTime = 0.5f;
    public float turnSpeed = 90f;
    public Light spotlight;
    public float viewDistance;
    public float closerVD;
    public float inFrontOfGuardVD;
    float viewAngle;
    Transform player;
    Color originalSpotlightColour;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        viewAngle = spotlight.spotAngle;
        originalSpotlightColour = spotlight.color;

        Vector3[] waypoints = new Vector3[pathHolder.childCount];

        for (int i = 0; i < waypoints.Length; i++)
        {
            waypoints[i] = pathHolder.GetChild(i).position;
            waypoints[i] = new Vector3(waypoints[i].x, transform.position.y, waypoints[i].z);
        }

        StartCoroutine(FollowPath(waypoints));
    }
    private void Update() {
        if(CanSeePlayer()){
            playerVisibleTimer += Time.deltaTime;
         
        } else{
            playerVisibleTimer -= Time.deltaTime;
        }
        playerVisibleTimer = Mathf.Clamp(playerVisibleTimer, 0, spotTimer);
        spotlight.color = Color.Lerp(originalSpotlightColour, Color.red, playerVisibleTimer / spotTimer);

        if (Vector3.Distance(transform.position, player.position) < closerVD)
        {
            spotTimer = timeToSpotCloserPlayer;
        }
        else 
        {
            spotTimer = timeToSpotPlayer;
        }
        if (Vector3.Distance(transform.position, player.position) < inFrontOfGuard)
        {
            spotTimer = inFrontOfGuard;
        }

        if (playerVisibleTimer >= spotTimer)
        {
            if(OnGuardHasSpottedPlayer != null)
            {
                OnGuardHasSpottedPlayer();
            }
        }
    }
    bool CanSeePlayer()
    {
        if (Vector3.Distance(transform.position,player.position) < viewDistance)
        {
            Vector3 dirToPlayer = (player.position - transform.position).normalized;
            float angleBetweenGuardAndPlayer = Vector3.Angle(transform.forward, dirToPlayer);
            if(angleBetweenGuardAndPlayer < viewAngle / 2f){
                if(!Physics.Linecast(transform.position, player.position, viewMask)){
                    return true;
                }
            }
        }
        return false;
    }

    IEnumerator FollowPath(Vector3[] waypoints)
    {
        transform.position = waypoints[0];

        int targetWaypointIndex = 1;
        Vector3 targetWaypoint = waypoints[targetWaypointIndex];
        transform.LookAt (targetWaypoint);

        while (true)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetWaypoint, speed * Time.deltaTime);
            if(transform.position == targetWaypoint)
            {
                targetWaypointIndex = (targetWaypointIndex + 1) % waypoints.Length;
                targetWaypoint = waypoints[targetWaypointIndex];
                yield return new WaitForSeconds(waitTime);
                yield return StartCoroutine(TurnToFace(targetWaypoint));

            }
            yield return null;
        }
    }

    IEnumerator TurnToFace(Vector3 lookTarget)
    {
        
        Vector3 dirToLookTarget = (lookTarget - transform.position).normalized;
        float targetAngle = 90 - Mathf.Atan2(dirToLookTarget.z, dirToLookTarget.x) * Mathf.Rad2Deg;

        while (Mathf.Abs(Mathf.DeltaAngle(transform.eulerAngles.y, targetAngle))>0.05f)
        {
            float angle = Mathf.MoveTowardsAngle(transform.eulerAngles.y, targetAngle, turnSpeed * Time.deltaTime);
            transform.eulerAngles = Vector3.up * angle;
            yield return null;
        }
    }

    void OnDrawGizmos()
    {
        Vector3 startPos = pathHolder.GetChild(0).position;
        Vector3 previousPos = startPos;


        foreach (Transform waypoint in pathHolder)
        {
            Gizmos.DrawSphere(waypoint.position, 0.3f);
            Gizmos.DrawLine(previousPos, waypoint.position);
            previousPos = waypoint.position;
        }
        Gizmos.DrawLine(previousPos, startPos);

        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position,transform.forward * viewDistance);

        Gizmos.color = Color.green;
        Gizmos.DrawRay(transform.position, transform.forward * closerVD);

        Gizmos.color = Color.magenta;
        Gizmos.DrawRay(transform.position, transform.forward * inFrontOfGuardVD);
    }

    // Update is called once per frame
    
}
