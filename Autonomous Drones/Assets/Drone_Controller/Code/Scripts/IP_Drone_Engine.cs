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
        // 프레임당 300회 회전
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
            
            // 자연스러운 비행 높이 조정
            Vector3 upVec = transform.up;
            upVec.x = 0f;
            upVec.z = 0f;
            float diff = 1 - upVec.magnitude;
            float finalDiff = Physics.gravity.magnitude * diff;


            // Vector3는 엔진 힘이라고 부르고 벡터 3.0으로 초기화 (모든 프레임이 0으로 변경)
            Vector3 engineForce = Vector3.zero;

            // 상승, 하강
            // 드론의 엔진이 4개이기 때문에 4로 나눠줌
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
