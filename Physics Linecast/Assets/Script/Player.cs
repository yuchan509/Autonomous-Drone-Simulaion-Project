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
    // boll �⺻���� false.
    bool disabled;

    void Start()
    {
        // ������Ʈ �� ������Ҹ� ������.
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
        // �Է� ����. : �Է°��� ���� ������ ���̸� �˱� ���� �ʼ�.
        // magnitude : ������ ���̸� ��ȯ.
        float inputMagnitude = inputDirection.magnitude;

        // ref : �̹� �ʱ�ȭ�� ���� ������ ��ü. �Լ� �ܺ� -> �Լ� ���η� ���� ������ ��� �̿�.
        smoothInputMagnitude = Mathf.SmoothDamp(smoothInputMagnitude, inputMagnitude, ref smoothMoveVelocity, smoothMoveTime);


        // Mathf.Rad2Deg : ������ ������ ����(radian)���� ��(degree) ������ ��ȯ.
        float targetAngle = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg;
         
        // LerpAngle(�ش簪, ��ǥ��, ����) : t�ð� ���� a���� b���� ����Ǵ� ������ ��ȯ��.�ε巯�� ȸ���� ������ �� ���� �Լ�.

        angle = Mathf.LerpAngle(angle, targetAngle, turnspeed * Time.deltaTime * inputMagnitude);

        velocity = transform.forward * moveSpeed * smoothInputMagnitude;
        
        // eulerAngles : ���Ϸ� �ޱ��� Vector3�� ǥ���Ǵ� ȸ����.
        // transform.eulerAngles = Vector3.up * targetAngle;

        // Transform.Translate�� ���ӿ�����Ʈ�� �̵���Ű�� ���� �Լ�
        // Space.World : ���� ��ǥ �������� ������ ���.
        // Time.deltaTime : ���ϴ� ������ �ʴ� ȣ��Ǵ� �������� ����̽����� �ٸ��� ������ �ð��� ���� ������ �Ÿ��� �����̰� �ϱ� ����.
        // transform.Translate(transform.forward * moveSpeed * Time.deltaTime * inputMagnitude, Space.World);
    }

    // ���� �������� �����ϸ� ��.
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

    // Guard�� player�� �߰����� �� �����̴� ���� ����.
    void Disable()
    {
        disabled = true;
    }

    // Ridgebody�� Fixedupdate�� ���� ����Ǿ�� ��.
    // FixedUpdate() : �ַ� ���������� �̿�.
    private void FixedUpdate()
    {
        // ���Ϸ� �ޱۿ��� ������ �����̶�� �Ѱ谡 �����Ͽ� ���� �� �������� ���ʹϾ�(x,y,z,ȸ����)�̶�� Ư���� ü�踦 �̿�.
        // ���Ϸ� �ޱ��� ���ʹϾ����� ��ȯ �� �ٽ� ���ʹϾ� ü�踦 �ٽ� Vector3�� ���� ���Ϸ� ü��� ��ȯ.
        rigidbody.MoveRotation(Quaternion.Euler(Vector3.up * angle));
        rigidbody.MovePosition(rigidbody.position + velocity * Time.deltaTime);

    }

    // ������Ʈ�� �ı��Ǿ��� �� �ڵ������� �ҷ����� �Լ�.
    private void OnDestroy()
    {
        Guard.OnGuardHasSpottedPlayer -= Disable;
    }
}