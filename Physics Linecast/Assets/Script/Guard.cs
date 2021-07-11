using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Guard : MonoBehaviour
{

    // static ������� ������ �ϴ� ��� �ٸ� script���� ���� �ҷ����� ���� ����.
    public static event System.Action OnGuardHasSpottedPlayer;

    public Transform pathHolder;

    // �ӵ� ����.
    public float speed = 5;
    // ��� �ð� ����.
    public float waitTime = .3f;
    // 1�ʿ� 90���� ȸ���ϴ� ����.
    public float turnSpeed = 90;

    public float timeToSpotPlayer = .5f;


    // ����.
    public Light spotlight;

    // �ݰ� �Ÿ�.
    public float viewDistance;
    public LayerMask viewMask;
    float viewAngle;

    float playerVisibleTimer;

    Transform player;
    Color originalSplotlightColour;


    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        viewAngle = spotlight.spotAngle;
        originalSplotlightColour = spotlight.color;

        Vector3[] waypoints = new Vector3[pathHolder.childCount];

        // for(������, ���� ���� : i�� waypoints �迭�� ��Ҽ� ���� ���� ���, ����)
        for (int i = 0; i < waypoints.Length; i++)
        {
            waypoints[i] = pathHolder.GetChild(i).position;
            // ������Ʈ�� �ٴڿ� ������ �����Ƿ�, y�� ���̸� �����Ѵ�.
            waypoints[i] = new Vector3(waypoints[i].x, transform.position.y, waypoints[i].z);
        }

        StartCoroutine(FollowPath(waypoints));
    }

    void Update()
    {
        if (CanSeePlayer())
        {
            // player�� ������ �� Ÿ�̸� �ߵ�.
            playerVisibleTimer += Time.deltaTime;
            // spotlight.color = Color.red;
        }
        else
        {
            playerVisibleTimer -= Time.deltaTime;
            // spotlight.color = originalSplotlightColour;
        }
        // Mathf.Clamp(�ش簪, �ּҰ�, �ִ밪) : �ش簪�� ������ �ּ� �ִ� ������ ���� �ʵ��� ��.
        playerVisibleTimer = Mathf.Clamp(playerVisibleTimer, 0, timeToSpotPlayer);

        // Color.Lerp(������, ����, ����)
        // Player ���� Ÿ���� 0�� ���� �� �÷θ� ����������, ������ �Ǹ鼭 ���� ���� ���������� �����.
        spotlight.color = Color.Lerp(originalSplotlightColour, Color.red, playerVisibleTimer / timeToSpotPlayer);

        if (playerVisibleTimer >= timeToSpotPlayer)
        {
            if (OnGuardHasSpottedPlayer != null)
                {
                    // �̺�Ʈ�� �ҷ���.
                    OnGuardHasSpottedPlayer();
                }
        }
    }

    bool CanSeePlayer()
    {
        if (Vector3.Distance(transform.position, player.position) < viewDistance)
        {
            Vector3 dirToPlayer = (player.position - transform.position).normalized;
            float angleBetweenGuardAndPlayer = Vector3.Angle(transform.forward, dirToPlayer);
            if (angleBetweenGuardAndPlayer < viewAngle / 2f)
            {
                if (!Physics.Linecast(transform.position, player.position, viewMask))
                {
                    return true;
                }
            }
        }
        return false;
    }

    // Coroutine �Լ�.
    IEnumerator FollowPath(Vector3[] waypoints)
    {
        // �켱 ù��° waypoint ������ ����.
        transform.position = waypoints[0];

        // �̵��� ������ waypoint �ε����� ǥ���� ���� �ϳ��� ����.
        int targetWaypointIndex = 1;

        // ���� ��ġ.
        Vector3 targetWaypoint = waypoints[targetWaypointIndex];

        transform.LookAt(targetWaypoint);

        while (true)
        {

            // transform.position : ������ ��ġ.
            // Vector3.MoveToward : Vector���� ������ ��ġ �������� �ƴ� ����� ��ġ ������ ����. ���� ��ġ���� ��ǥ ��ġ�� ������ �ӵ��� �̵��ϴ� �Լ�.
            // Vector3.MoveToward(������ : ���� ��ġ, ����(��ǥ ��ġ) : targetWaypoint, �ӵ� : speed * Time.deltaTime )
            // Time.deltaTime : ���ɿ� ��� ���� �ΰ��� ������ �� �ִ� �ּ� �������� ����.
            transform.position = Vector3.MoveTowards(transform.position, targetWaypoint, speed * Time.deltaTime);

            // ���� �ޱ�. (���� ��ġ�� �������� �����ϴٰ� �Ѵٸ�)
            if (transform.position == targetWaypoint)
            {
                // waypoint�� �̵���Ų��.
                // �ε��� 0 �������� �ٽ� �ǵ����� ���� ������ �����ڸ� �̿�.
                targetWaypointIndex = (targetWaypointIndex + 1) % waypoints.Length;
                targetWaypoint = waypoints[targetWaypointIndex];
                // ���ð�
                yield return new WaitForSeconds(waitTime);
                yield return StartCoroutine(TurnToFace(targetWaypoint));
            }
            // ������ ���� ���ȿ��� ���� �ܰ�� �Ѿ�� ���� ����.
            yield return null;

        }
    }

    // Coroutine �Լ�2.
    // Guard�� y���� ���� ����� �������ϴ� ������ ���.
    IEnumerator TurnToFace(Vector3 lookTarget)
    {
        // Ÿ���� �ٶ󺸴� ���� ������ ���.
        // ���� ���� ���� �Ÿ��� ������ ����� ����.
        Vector3 dirToLookTarget = (lookTarget - transform.position).normalized;
        // Ÿ���� ���� (90 - arctan�� ������)
        float targetAngle = 90 - Mathf.Atan2(dirToLookTarget.z, dirToLookTarget.x) * Mathf.Rad2Deg;

        // ���Ϸ��� ���� ��Ȯ�ϰ� �������� ���� �ƴϹǷ�, 0���� �������ٴ� ���� ���� ������ ������ġ�� ����°� ����.
        while (Mathf.Abs(Mathf.DeltaAngle(transform.eulerAngles.y, targetAngle)) > .05f)
        {

            float angle = Mathf.MoveTowardsAngle(transform.eulerAngles.y, targetAngle, turnSpeed * Time.deltaTime);
            transform.eulerAngles = Vector3.up * angle;
            // ���� ��ƾ���� ���.
            yield return null;
        }
    }

    void OnDrawGizmos()
    {
        // �̵� ũ�⳪ ��ġ ������ ������ ���ͷ� pathHolder �ڽ� ����� �ε���(0) ��ġ�� �Ҵ�.
        Vector3 startPosition = pathHolder.GetChild(0).position;
        // ���� ��ġ�� ���� ��ġ�� �Ҵ�.
        Vector3 previousPositon = startPosition;
        foreach (Transform waypoint in pathHolder)
        {
            // Gizmos.DrawSphere(��ġ, ����) : Gizmos�� ��ü�� �׸��� ���� �Լ�.    
            Gizmos.DrawSphere(waypoint.position, .3f);
            // Gizmos.DrawLine(��������, ���� : ���� ����)
            Gizmos.DrawLine(previousPositon, waypoint.position);
            previousPositon = waypoint.position;
        }
        Gizmos.DrawLine(previousPositon, startPosition);

        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, transform.forward * viewDistance);
    }
 }