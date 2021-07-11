using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace IndiePixel
{

    [RequireComponent(typeof(PlayerInput))]

    public class IP_Drone_Inputs : MonoBehaviour
    {   
        
        // region 사용이유 : 작은 블록들로 코드를 구성하여 보기 위해
        
        // 변수
        // Drone_Input에 선언했던거 사용
        #region Varaiables

        private Vector2 cyclic;
        private float pedals;
        private float throttle;

        // 속성(프로퍼티)을 사용
        public Vector2 Cyclic { get => cyclic; }
        public float Pedals { get => pedals; }
        public float Throttle { get => throttle;}
        #endregion


        #region Main Methods
        // Update is called once per frame
        void Update()
        {

        }
        #endregion


        #region Input Methods
        // Input 함수생성
        // 해당 입력이 들어오면 값을 지정해줌

        // WASD
        private void OnCyclic(InputValue value)
        {
            cyclic = value.Get<Vector2>();
        }

        // 방향키 <-, ->
        private void OnPedals(InputValue value)
        {
            pedals = value.Get<float>();
        }

        // 방향키 위,아래
        private void OnThrottle(InputValue value)
        {
            throttle = value.Get<float>();
        }
        #endregion
    }

}