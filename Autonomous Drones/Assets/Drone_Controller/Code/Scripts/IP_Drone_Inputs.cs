using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace IndiePixel
{

    [RequireComponent(typeof(PlayerInput))]

    public class IP_Drone_Inputs : MonoBehaviour
    {   
        
        // region ������� : ���� ��ϵ�� �ڵ带 �����Ͽ� ���� ����
        
        // ����
        // Drone_Input�� �����ߴ��� ���
        #region Varaiables

        private Vector2 cyclic;
        private float pedals;
        private float throttle;

        // �Ӽ�(������Ƽ)�� ���
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
        // Input �Լ�����
        // �ش� �Է��� ������ ���� ��������

        // WASD
        private void OnCyclic(InputValue value)
        {
            cyclic = value.Get<Vector2>();
        }

        // ����Ű <-, ->
        private void OnPedals(InputValue value)
        {
            pedals = value.Get<float>();
        }

        // ����Ű ��,�Ʒ�
        private void OnThrottle(InputValue value)
        {
            throttle = value.Get<float>();
        }
        #endregion
    }

}