using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace IndiePixel
{
    // 드론입력을 종속
    [RequireComponent(typeof(IP_Drone_Inputs))]

    // 드론 기본강체 상속
    public class IP_Drone_Controller : IP_Base_Rigidbody
    {
        #region Variables
        [Header("Control Properties")]
        // 인스펙터에서 직렬화된 필드를 수행하기 위함
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
            // 입력 변수 값을 지정하여 IP_Drone_Inputs의 값을 사용한다.
            input = GetComponent<IP_Drone_Inputs>();
            // 드론 엔진이 아니라 AI 엔진을 찾음
            // List 형태로 만듬
            engines = GetComponentsInChildren<IEngine>().ToList<IEngine>();
        }
        #endregion



        #region Custom Methods
        protected override void HandlePhysics()
        {
            // base.HandlePhysics();
            // 엔진에 대한 실제 컨트롤
            HandleEngines();
            HandleControls();

        }
        protected virtual void HandleEngines()
        {
            // 물리학을 증명
            // 중력에 대한 방정식
            // rb.AddForce(Vector3.up * (rb.mass * Physics.gravity.magnitude));

            // 각 엔진을 반복
            foreach(IEngine engine in engines)
            {
                // 드론 강체, 입력데이터를 받아옴
                engine.UpdateEngine(rb, input);
            }
        }

        // 드론 방향전환
        protected virtual void HandleControls()
        {
            // 조작을 통해 입력값을 받은 데이터를 각각의 직렬화된 필드에 곱함
            float pitch = input.Cyclic.y * minMaxPitch;
            float roll = -input.Cyclic.x * minMaxRoll;
            // 드론이 제자리 방향전환을 하게해줌
            yaw += input.Pedals * yawPower;

            finalPitch = Mathf.Lerp(finalPitch, pitch, Time.deltaTime * lerpSpeed);
            finalRoll = Mathf.Lerp(finalRoll, roll, Time.deltaTime * lerpSpeed);
            finalYaw = Mathf.Lerp(finalYaw, yaw, Time.deltaTime * lerpSpeed);

            // 회전을 위한 rot 변수 선언
            // 쿼터니언.Euler : 실제로 여러 부동 값을 펌핑 (x,y,z)
            Quaternion rot = Quaternion.Euler(finalPitch, finalYaw, finalRoll);
            rb.MoveRotation(rot);
        }
        #endregion
    }
}
