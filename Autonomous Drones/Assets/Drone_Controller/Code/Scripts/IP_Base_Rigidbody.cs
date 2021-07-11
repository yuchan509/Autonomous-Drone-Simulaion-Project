using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace IndiePixel
{  
    [RequireComponent(typeof(Rigidbody))]


    public class IP_Base_Rigidbody : MonoBehaviour
    {
        #region Variables
        // ����ڰ� ���⿡ ����ġ�� �ο��ϵ��� ���
        [Header("Rigidbody Properties")]
        // ��� ���� ���� 1�Ŀ��
        [SerializeField] private float weightInLbs = 0.1f;

        const float lbsTokg = 0.454f;

        // ���۵ɶ� ���� 
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
                // ų�α׷����� ��������
                rb.mass = weightInLbs * lbsTokg;
                // �巡�װ� �ν����Ϳ��� �����Ǿ� ����
                startDrag = rb.drag;
                startAngularDrag = rb.angularDrag;
            }
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            // rigidbody�� ã�� ������ ���
            if (!rb)
            {
                return;
            }

            // �ڵ��� ȣ���� �Լ�
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
