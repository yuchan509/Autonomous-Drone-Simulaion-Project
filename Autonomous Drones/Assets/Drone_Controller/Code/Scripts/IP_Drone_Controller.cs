using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace IndiePixel
{
    // ����Է��� ����
    [RequireComponent(typeof(IP_Drone_Inputs))]

    // ��� �⺻��ü ���
    public class IP_Drone_Controller : IP_Base_Rigidbody
    {
        #region Variables
        [Header("Control Properties")]
        // �ν����Ϳ��� ����ȭ�� �ʵ带 �����ϱ� ����
        [SerializeField] private float minMaxPitch = 30f;
        [SerializeField] private float minMaxRoll = 30f;
        [SerializeField] private float yawPower = 4f;
        [SerializeField] private float lerpSpeed = 2f;

        private IP_Drone_Inputs input;
        private List<IEngine> engines = new List<IEngine>();

        private float finalPitch;
        private float finalRoll;
        private float yaw;
        private float finalYaw;
        #endregion

        #region
        // Start is called before the first frame update
        void Start()
        {
            // �Է� ���� ���� �����Ͽ� IP_Drone_Inputs�� ���� ����Ѵ�.
            input = GetComponent<IP_Drone_Inputs>();
            // ��� ������ �ƴ϶� AI ������ ã��
            // List ���·� ����
            engines = GetComponentsInChildren<IEngine>().ToList<IEngine>();
        }
        #endregion



        #region Custom Methods
        protected override void HandlePhysics()
        {
            // base.HandlePhysics();
            // ������ ���� ���� ��Ʈ��
            HandleEngines();
            HandleControls();

        }
        protected virtual void HandleEngines()
        {
            // �������� ����
            // �߷¿� ���� ������
            // rb.AddForce(Vector3.up * (rb.mass * Physics.gravity.magnitude));

            // �� ������ �ݺ�
            foreach(IEngine engine in engines)
            {
                // ��� ��ü, �Էµ����͸� �޾ƿ�
                engine.UpdateEngine(rb, input);
            }
        }

        // ��� ������ȯ
        protected virtual void HandleControls()
        {
            // ������ ���� �Է°��� ���� �����͸� ������ ����ȭ�� �ʵ忡 ����
            float pitch = input.Cyclic.y * minMaxPitch;
            float roll = -input.Cyclic.x * minMaxRoll;
            // ����� ���ڸ� ������ȯ�� �ϰ�����
            yaw += input.Pedals * yawPower;

            finalPitch = Mathf.Lerp(finalPitch, pitch, Time.deltaTime * lerpSpeed);
            finalRoll = Mathf.Lerp(finalRoll, roll, Time.deltaTime * lerpSpeed);
            finalYaw = Mathf.Lerp(finalYaw, yaw, Time.deltaTime * lerpSpeed);

            // ȸ���� ���� rot ���� ����
            // ���ʹϾ�.Euler : ������ ���� �ε� ���� ���� (x,y,z)
            Quaternion rot = Quaternion.Euler(finalPitch, finalYaw, finalRoll);
            rb.MoveRotation(rot);
        }
        #endregion
    }
}
