using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Guard : MonoBehaviour
{

    // static 방식으로 선언을 하는 경우 다른 script에서 쉽게 불러오는 것이 가능.
    public static event System.Action OnGuardHasSpottedPlayer;

    public Transform pathHolder;

    // 속도 변수.
    public float speed = 5;
    // 대기 시간 변수.
    public float waitTime = .3f;
    // 1초에 90도를 회전하는 변수.
    public float turnSpeed = 90;

    public float timeToSpotPlayer = .5f;


    // 조명.
    public Light spotlight;

    // 반경 거리.
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

        // for(시작점, 참인 조건 : i가 waypoints 배열의 요소수 보다 작을 경우, 실행)
        for (int i = 0; i < waypoints.Length; i++)
        {
            waypoints[i] = pathHolder.GetChild(i).position;
            // 오브젝트가 바닥에 먹히고 있으므로, y의 높이를 지정한다.
            waypoints[i] = new Vector3(waypoints[i].x, transform.position.y, waypoints[i].z);
        }

        StartCoroutine(FollowPath(waypoints));
    }

    void Update()
    {
        if (CanSeePlayer())
        {
            // player가 감지될 시 타이머 발동.
            playerVisibleTimer += Time.deltaTime;
            // spotlight.color = Color.red;
        }
        else
        {
            playerVisibleTimer -= Time.deltaTime;
            // spotlight.color = originalSplotlightColour;
        }
        // Mathf.Clamp(해당값, 최소값, 최대값) : 해당값이 설정한 최소 최대 범위를 넘지 않도록 함.
        playerVisibleTimer = Mathf.Clamp(playerVisibleTimer, 0, timeToSpotPlayer);

        // Color.Lerp(시작점, 종점, 간격)
        // Player 감지 타임이 0일 때는 본 컬로를 유지하지만, 감지가 되면서 점차 색이 붉은색으로 변경됨.
        spotlight.color = Color.Lerp(originalSplotlightColour, Color.red, playerVisibleTimer / timeToSpotPlayer);

        if (playerVisibleTimer >= timeToSpotPlayer)
        {
            if (OnGuardHasSpottedPlayer != null)
                {
                    // 이벤트를 불러옴.
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

    // Coroutine 함수.
    IEnumerator FollowPath(Vector3[] waypoints)
    {
        // 우선 첫번째 waypoint 지점을 설정.
        transform.position = waypoints[0];

        // 이동할 지점의 waypoint 인덱스를 표기할 정수 하나를 생성.
        int targetWaypointIndex = 1;

        // 실제 위치.
        Vector3 targetWaypoint = waypoints[targetWaypointIndex];

        transform.LookAt(targetWaypoint);

        while (true)
        {

            // transform.position : 현재의 위치.
            // Vector3.MoveToward : Vector에는 절대적 위치 정보만이 아닌 상대적 위치 정보도 가짐. 현재 위치에서 목표 위치로 일정한 속도로 이동하는 함수.
            // Vector3.MoveToward(시작점 : 현재 위치, 종점(목표 위치) : targetWaypoint, 속도 : speed * Time.deltaTime )
            // Time.deltaTime : 성능에 상관 없이 인간이 인지할 수 있는 최소 단위로의 보정.
            transform.position = Vector3.MoveTowards(transform.position, targetWaypoint, speed * Time.deltaTime);

            // 조건 달기. (현재 위치와 목적지가 동일하다고 한다면)
            if (transform.position == targetWaypoint)
            {
                // waypoint를 이동시킨다.
                // 인덱스 0 지점으로 다시 되돌리기 위해 나머지 연산자를 이용.
                targetWaypointIndex = (targetWaypointIndex + 1) % waypoints.Length;
                targetWaypoint = waypoints[targetWaypointIndex];
                // 대기시간
                yield return new WaitForSeconds(waitTime);
                yield return StartCoroutine(TurnToFace(targetWaypoint));
            }
            // 조건이 참인 동안에는 다음 단계로 넘어가는 것을 방지.
            yield return null;

        }
    }

    // Coroutine 함수2.
    // Guard가 y축을 따라 대상을 보도록하는 각도를 계산.
    IEnumerator TurnToFace(Vector3 lookTarget)
    {
        // 타겟을 바라보는 벡터 방향을 계산.
        // 벡터 둘을 빼면 거리와 방향의 계산이 가능.
        Vector3 dirToLookTarget = (lookTarget - transform.position).normalized;
        // 타겟의 각도 (90 - arctan의 보정값)
        float targetAngle = 90 - Mathf.Atan2(dirToLookTarget.z, dirToLookTarget.x) * Mathf.Rad2Deg;

        // 오일러수 값이 정확하게 떨어지는 값이 아니므로, 0으로 설정보다는 조금 작은 값으로 안정장치를 만드는게 좋음.
        while (Mathf.Abs(Mathf.DeltaAngle(transform.eulerAngles.y, targetAngle)) > .05f)
        {

            float angle = Mathf.MoveTowardsAngle(transform.eulerAngles.y, targetAngle, turnSpeed * Time.deltaTime);
            transform.eulerAngles = Vector3.up * angle;
            // 다음 루틴까지 대기.
            yield return null;
        }
    }

    void OnDrawGizmos()
    {
        // 이동 크기나 위치 정보를 가지는 벡터로 pathHolder 자식 노드의 인덱스(0) 위치를 할당.
        Vector3 startPosition = pathHolder.GetChild(0).position;
        // 이전 위치에 시작 위치를 할당.
        Vector3 previousPositon = startPosition;
        foreach (Transform waypoint in pathHolder)
        {
            // Gizmos.DrawSphere(위치, 범위) : Gizmos로 구체를 그리기 위한 함수.    
            Gizmos.DrawSphere(waypoint.position, .3f);
            // Gizmos.DrawLine(시작점과, 종점 : 현재 지점)
            Gizmos.DrawLine(previousPositon, waypoint.position);
            previousPositon = waypoint.position;
        }
        Gizmos.DrawLine(previousPositon, startPosition);

        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, transform.forward * viewDistance);
    }
 }