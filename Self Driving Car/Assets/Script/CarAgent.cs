using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using UnityEngine;
using Unity.MLAgents.Actuators;

public class CarAgent : Agent
{
    public float speed = 10f;
    public float torque = 1f;
    public float turnspeed = 8f;

    public int score = 0;
    public bool resetOnCollision = true;

    private Transform _track;

    


    public override void Initialize()
    {
        GetTrackIncrement();
    }

    private void MoveCar(float horizontal, float vertical, float dt)
    {
        float distance = speed * vertical;
        transform.Translate(distance * dt * Vector3.forward);

        float rotation = horizontal * torque * 90f;
        transform.Rotate(0f, rotation * dt, 0f);
    }

    public static float GetAngle(Vector3 vStart, Vector3 vEnd)
    {
        Vector3 v = vEnd - vStart;

        return Mathf.Atan2(v.y, v.x) * Mathf.Rad2Deg;
    }



    public override void OnActionReceived(float[] vectorAction)
    {
        float horizontal = vectorAction[0];
        float vertical = vectorAction[1];

        var lastPos = transform.position;
        MoveCar(horizontal, vertical, Time.fixedDeltaTime);

        int reward = GetTrackIncrement();

        var moveVec = transform.position - lastPos;

        // angle = Vector3.Angle(moveVec, _track.forward);
        float angle = GetAngle(moveVec, _track.position);
        float bonus = (1f - angle / 90f) * Mathf.Clamp01(vertical) * Time.fixedDeltaTime;
        AddReward(bonus);

        score += reward;
    }

    // 직접 조종 부분.
    public override void Heuristic(float[] action)
    {
        action[0] = Input.GetAxis("Horizontal");
        action[1] = Input.GetAxis("Vertical");
    }

    // 관찰 부분.
    public override void CollectObservations(VectorSensor vectorSensor)
    {
        float angle = Vector3.SignedAngle(_track.forward, transform.forward, Vector3.up);

        vectorSensor.AddObservation(angle / 180f);
        vectorSensor.AddObservation(ObserveRay(1f, .5f, 26f));
        vectorSensor.AddObservation(ObserveRay(1f, 0f, 0f));
        vectorSensor.AddObservation(ObserveRay(1f, -.5f, -26f));
        vectorSensor.AddObservation(ObserveRay(-1f, 0, 180f));
    }


    // 레이 관찰.
    private float ObserveRay(float z, float x, float angle)
    {
        var tf = transform;

        // Get the start position of the ray
        var raySource = tf.position + Vector3.up / 2f;
        const float rayDist = 3f;
        var position = raySource + tf.forward * z + tf.right * x;

        // 레이 각도 얻기.
        // Quaternion을 Euler angle로 바꾸어줌.
        var eulerAngle = Quaternion.Euler(0, angle, 0f);
       
        // 현재 방향 : 오일러 각 * 현재 위치의 앞.
        var dir = eulerAngle * tf.forward;

        // 만약 주어진 방향 하에 광선이 닿는 곳이 있다면 초록색 라인으로 표시.
        RaycastHit hit;
        if (Physics.Raycast(position, dir, out hit, rayDist))
        {
            Debug.DrawRay(position, dir * 10, Color.green);
        }
       
        return hit.distance >= 0 ? hit.distance / rayDist : -1f;
    }

    // 각 트랙에 대한 보상값 설정이 필요.
    private int GetTrackIncrement()
    {
        // 초기 보상값 설정.
        int reward = 0;

        // 차의 중심축 : ray 변수 : 광선의 시작의 좌표 설정.
        var carCenter = transform.position + Vector3.up;

        // 방향 설정 : 아래 :Vector3.down 
        // 변수 hit : 충돌체의 충돌의 정보를 저장하는 변수.
        // 2f : 광선의 길이를 정의.
        if (Physics.Raycast(carCenter, Vector3.down, out var hit, 1.5f))
        {
            // transform : 오브젝트의 위치, 회전, 크기를 나타내는 컴포넌트.
            // 충돌 정보를 저장한 변수의 오브젝트 위치, 회전, 크기를 새로운 충돌 정보로 지정.
            var newHit = hit.transform;
            
            // 만약 다른 트랙 부분으로 이동한다면.
            if (_track != null && newHit != _track)
            {
                // 두 벡터의 각을 구함.
                float angle = Vector3.Angle(_track.forward, newHit.position - _track.position);
                // 만약 각이 90도 이하의 값을 갖는다면 보상을 1을 주고, 그렇지 않다면 벌칙값을 부여.
                reward = (angle < 90f) ? 1 : -1;
            }


            _track = newHit;
        }

        return reward;
    }

    public override void OnEpisodeBegin()
    {
        // 만약 충돌시 리셋.
        if (resetOnCollision)
        {
            transform.localPosition = new Vector3(60,0,-280);
            transform.localRotation = Quaternion.identity;
        }
    }

    // 벽에 부딪힌다면 처벌값을 부여하고 한 에피소드를 종료시킨다.
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("wall"))
        {
            SetReward(-1f);
            EndEpisode();
        }
    }
}

