using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public event System.Action OnReachedEndOfLevel;

    public float moveSpeed = 7;
    public float smoothMoveTime = .1f;
    public float turnspeed = 8;


    float angle;
    float smoothInputMagnitude;
    float smoothMoveVelocity;
    Vector3 velocity;

    new Rigidbody rigidbody;
    // boll 기본값은 false.
    bool disabled;

    void Start()
    {
        // 오브젝트 내 구성요소를 가져옴.
        rigidbody = GetComponent<Rigidbody>();
        Guard.OnGuardHasSpottedPlayer += Disable;
    }
    void Update()
    {
        Vector3 inputDirection = Vector3.zero;
        // Vector3 inputDirection = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;
        if (!disabled)
        {
           inputDirection = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;
        }
        // 입력 강도. : 입력값의 벡터 방향의 길이를 알기 위해 필수.
        // magnitude : 벡터의 길이를 반환.
        float inputMagnitude = inputDirection.magnitude;

        // ref : 이미 초기화가 끝난 상태의 객체. 함수 외부 -> 함수 내부로 값을 전달할 경우 이용.
        smoothInputMagnitude = Mathf.SmoothDamp(smoothInputMagnitude, inputMagnitude, ref smoothMoveVelocity, smoothMoveTime);


        // Mathf.Rad2Deg : 각도의 단위를 라디안(radian)에서 도(degree) 단위로 변환.
        float targetAngle = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg;
         
        // LerpAngle(해당값, 목표값, 간격) : t시간 동안 a부터 b까지 변경되는 각도를 반환함.부드러운 회전을 구현할 때 쓰는 함수.

        angle = Mathf.LerpAngle(angle, targetAngle, turnspeed * Time.deltaTime * inputMagnitude);

        velocity = transform.forward * moveSpeed * smoothInputMagnitude;
        
        // eulerAngles : 오일러 앵글은 Vector3로 표현되는 회전값.
        // transform.eulerAngles = Vector3.up * targetAngle;

        // Transform.Translate는 게임오브젝트를 이동시키기 위한 함수
        // Space.World : 절대 좌표 기준으로 움직일 경우.
        // Time.deltaTime : 곱하는 이유는 초당 호출되는 프레임이 디바이스마다 다르기 때문에 시간에 따라 동일한 거리를 움직이게 하기 위함.
        // transform.Translate(transform.forward * moveSpeed * Time.deltaTime * inputMagnitude, Space.World);
    }

    // 도착 지점에서 도달하면 끝.
    void OnTriggerEnter(Collider hitCollider)
    {
        if (hitCollider.tag == "Finish")
        {
            Disable();
            if (OnReachedEndOfLevel != null)
            {
                OnReachedEndOfLevel();
            }
        }
    }

    // Guard가 player를 발견했을 때 움직이는 것을 방지.
    void Disable()
    {
        disabled = true;
    }

    // Ridgebody는 Fixedupdate에 의해 제어되어야 함.
    // FixedUpdate() : 주로 물리엔진에 이용.
    private void FixedUpdate()
    {
        // 오일러 앵글에는 짐벌락 현상이라는 한계가 존재하여 보다 잘 쓰기위해 쿼터니언(x,y,z,회전값)이라는 특수한 체계를 이용.
        // 오일러 앵글을 쿼터니언으로 변환 뒤 다시 쿼터니언 체계를 다시 Vector3를 쓰는 오일러 체계로 전환.
        rigidbody.MoveRotation(Quaternion.Euler(Vector3.up * angle));
        rigidbody.MovePosition(rigidbody.position + velocity * Time.deltaTime);

    }

    // 오브젝트가 파괴되었을 때 자동적으로 불러오는 함수.
    private void OnDestroy()
    {
        Guard.OnGuardHasSpottedPlayer -= Disable;
    }
}