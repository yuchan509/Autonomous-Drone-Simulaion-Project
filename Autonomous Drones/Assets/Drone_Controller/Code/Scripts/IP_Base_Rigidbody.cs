using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace IndiePixel
{  
    [RequireComponent(typeof(Rigidbody))]


    public class IP_Base_Rigidbody : MonoBehaviour
    {
        #region Variables
        // 사용자가 여기에 가중치를 부여하도록 허용
        [Header("Rigidbody Properties")]
        // 드론 무게 설정 1파운드
        [SerializeField] private float weightInLbs = 0.1f;

        const float lbsTokg = 0.454f;

        // 시작될때 변수 
        protected Rigidbody rb; 
        protected float startDrag;
        protected float startAngularDrag;

        #endregion


        #region Main Methods
        // Start is called before the first frame update
        void Awake()
        {
            rb = GetComponent<Rigidbody>();
            if (rb)
            {
                // 킬로그램으로 설정가능
                rb.mass = weightInLbs * lbsTokg;
                // 드래그가 인스펙터에서 설정되어 있음
                startDrag = rb.drag;
                startAngularDrag = rb.angularDrag;
            }
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            // rigidbody를 찾지 못했을 경우
            if (!rb)
            {
                return;
            }

            // 핸들을 호출할 함수
            HandlePhysics();
        }
        #endregion

        #region Custom Methods
        protected virtual void HandlePhysics()
        {

        }
        #endregion
    }
}
