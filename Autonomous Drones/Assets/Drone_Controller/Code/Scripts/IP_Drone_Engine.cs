 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IndiePixel
{
    // Unity Collider
    [RequireComponent(typeof(BoxCollider))]

    public class IP_Drone_Engine : MonoBehaviour, IEngine
    {

        #region Variables
        [Header("Engine Properties")]
        [SerializeField] private float maxPower = 8f;

        [Header("Propeller Properties")]
        [SerializeField] private Transform propeller;
        // �����Ӵ� 300ȸ ȸ��
        [SerializeField] private float proRotSpeed = 300;
        #endregion



        #region Interface Methods
        public void InitEngine()
        {
            throw new System.NotImplementedException();
        }

        public void UpdateEngine(Rigidbody rb, IP_Drone_Inputs input)
        {
            // throw new System.NotImplementedException();
            // Debug.Log("Running Engine: " + gameObject.name);
            
            // �ڿ������� ���� ���� ����
            Vector3 upVec = transform.up;
            upVec.x = 0f;
            upVec.z = 0f;
            float diff = 1 - upVec.magnitude;
            float finalDiff = Physics.gravity.magnitude * diff;


            // Vector3�� ���� ���̶�� �θ��� ���� 3.0���� �ʱ�ȭ (��� �������� 0���� ����)
            Vector3 engineForce = Vector3.zero;

            // ���, �ϰ�
            // ����� ������ 4���̱� ������ 4�� ������
            engineForce = transform.up * ((rb.mass * Physics.gravity.magnitude + finalDiff)  + (input.Throttle * maxPower)) / 6f;

            rb.AddForce(engineForce, ForceMode.Force);

            HandlePropellers();
        }

        void HandlePropellers()
        {
            if (!propeller)
            {
                return;
            }

            
            propeller.Rotate(Vector3.forward, proRotSpeed);
        }
        #endregion
    }
}
