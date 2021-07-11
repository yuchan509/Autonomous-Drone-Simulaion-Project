using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using UnityEngine.UI;

public class Drone_Vapo : Agent
{

    public GameObject goal;
    public GameObject head;
    public GameObject wall;
    public float StartingHeightMin = 0f;
    public float StartingHeightMax = 2f;
    public float UpForce = 10f;

    Vector3 goalInitPos;
    Vector3 game0bjectXpoint;
    Quaternion InitRot;
    Rigidbody rBody;

    Vector3 droneInitPos;
    Quaternion RBInitRot;
    Quaternion RFInitRot;
    Quaternion LBInitRot;
    Quaternion LFInitRot;

    Quaternion droneInitRot;


    public GameObject RF_Engine;
    public GameObject LB_Engine;
    public GameObject RB_Engine;
    public GameObject LF_Engine;
    public GameObject L_Engine;
    public GameObject R_Engine;

    Rigidbody rb1;
    Rigidbody rb2;
    Rigidbody rb3;
    Rigidbody rb4;
    Rigidbody rb5;
    Rigidbody rb6;

    float preDist;
    float curDist;

    public override void Initialize()
    {
        goalInitPos = goal.transform.position;


        droneInitPos = gameObject.transform.position;
        droneInitRot = gameObject.transform.rotation;

        rBody = gameObject.GetComponent<Rigidbody>();
        rb1 = RF_Engine.GetComponent<Rigidbody>();
        rb2 = LB_Engine.GetComponent<Rigidbody>();
        rb3 = RB_Engine.GetComponent<Rigidbody>();
        rb4 = LF_Engine.GetComponent<Rigidbody>();
        rb5 = L_Engine.GetComponent<Rigidbody>();
        rb6 = R_Engine.GetComponent<Rigidbody>();
    }

    public override void OnEpisodeBegin()
    {
        // ������ ���� ���۵� �� �ԷµǴ� ��
        gameObject.transform.position = droneInitPos;
        gameObject.transform.rotation = droneInitRot;
        rBody.velocity = Vector3.zero;
        rBody.angularVelocity = Vector3.zero;

        // ���ӵ�, �߷°��ӵ� �̷��� �ʱ�ȭ ���ֱ�
        rb1.velocity = Vector3.zero;
        rb2.velocity = Vector3.zero;
        rb3.velocity = Vector3.zero;
        rb4.velocity = Vector3.zero;
        rb5.velocity = Vector3.zero;
        rb6.velocity = Vector3.zero;
        rb1.angularVelocity = Vector3.zero;
        rb2.angularVelocity = Vector3.zero;
        rb3.angularVelocity = Vector3.zero;
        rb4.angularVelocity = Vector3.zero;
        rb5.angularVelocity = Vector3.zero;
        rb6.angularVelocity = Vector3.zero;

        // ����ǥ���� ����ϱ����� ����
        var theta1 = Random.Range(-180f, 180f);
        var theta2 = Random.Range(-180f, 180f);
        var radius = Random.Range(10f, 1000f);

        // �ʱ�ȭ�Ǵ� ��ǥ�� ��ǥ�� ���� ����
        //goal.transform.position = goalInitPos + new Vector3(radius * Mathf.Sin(theta1) * Mathf.Cos(theta2),
        //                                                    Random.Range(1f, 80f),
        //                                                    radius * Mathf.Cos(theta1));

        preDist = (goal.transform.position - gameObject.transform.position).magnitude;
    }

    public override void CollectObservations(VectorSensor sensor)
    {



        if (goal != null)
        {
            //sensor.AddObservation(goal.transform.position - head.transform.position);
            sensor.AddObservation(goal.transform.position - gameObject.transform.position);
            sensor.AddObservation(gameObject.transform.rotation);
            sensor.AddObservation(rBody.velocity);
            sensor.AddObservation(rBody.angularVelocity);
            sensor.AddObservation(gameObject.transform.up);
            //sensor.AddObservation(gameObject.transform.right);
            //sensor.AddObservation(gameObject.transform.forward);


        }
    }

    public override void OnActionReceived(float[] vectorAction)
    {

        // Vector3 �� ���¸� ���� �Լ��� ������ش�.
        // �׷��� x,y,z ���� �Է�����
        Vector3 controlSignal = Vector3.zero;
        controlSignal.x = vectorAction[0];
        controlSignal.y = vectorAction[1];
        controlSignal.z = vectorAction[2];


        rb1.AddForce(controlSignal * UpForce);
        rb2.AddForce(controlSignal * UpForce);
        rb3.AddForce(controlSignal * UpForce);
        rb4.AddForce(controlSignal * UpForce);
        rb5.AddForce(controlSignal * UpForce);
        rb6.AddForce(controlSignal * UpForce);
        //gameObject.transform.Rotate(new Vector3(0, ((vectorAction[3] * UpForce) / 2), 0));
        AddReward(-0.01f);




        // ���� �����Ͽ� �Ÿ��� 1.5f ���ϵǸ� ������ 10�� ��� success ��
        // V2.4 
        // if ((goal.transform.position - gameObject.transform.position).magnitude < 1.5f)
        // V2.5 
        if ((goal.transform.position - gameObject.transform.position).magnitude < 50f)
        {
            SetReward(100);
            // EndEpisode();
            Debug.Log("Success.");
        }
        // ������� ���� ���ǵ��� �޾��ش�
        // 1. x,y,z ���� 50�̻� ���̳��� �Ǵ� ���
        // 2. ����� y���� 0.2 ���Ϸ� �������� ���
        // 3. ����� up�� y�� 0.5 ���Ϸ� �������� ���
        // 4. y�� 0.2 �����϶��� �׳� ����������
        else if (gameObject.transform.position.y < 0.2f)
            //gameObject.transform.up.y < 0.5f )
        {
            SetReward(-100f);
            EndEpisode();
            Debug.Log("Failed.");
        }

        else if (Mathf.Abs(gameObject.transform.position.x - goal.transform.position.x) > 2400f ||
            Mathf.Abs(gameObject.transform.position.z - goal.transform.position.z) > 2400f ||
            gameObject.transform.position.y - goal.transform.position.y > 1000f)
        {
            SetReward(-100f);
            EndEpisode();
            Debug.Log("distance reset");
        }
        else if (goal.transform.position.y < 0.2f)
        {
            EndEpisode();
            Debug.Log("reset");
        }
        // �̺κ��� �Ϻ��� ���ش� �ȵƴµ� �ϴ� ������ Ÿ�ֿ̹� ��ǥ�� ����� �Ÿ��� ����ؼ�
        // ������ ����ؼ���
        else
        {
            var reward = (preDist - curDist);
            var distance = Vector3.Distance(goal.transform.position, gameObject.transform.position);
            curDist = distance;

            SetReward(reward/10f);
            
            preDist = curDist;
        }

    }

    private void OnCollisionEnter(Collision collision)
    {
        // Cube crashed into the block wall
        if (collision.gameObject.CompareTag("wall"))
        {
            SetReward(-80f);
            EndEpisode();
            Debug.Log("wall");
        }
    }


}
