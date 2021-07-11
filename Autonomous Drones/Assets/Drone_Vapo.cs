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
        // 게임이 새로 시작될 때 입력되는 값
        gameObject.transform.position = droneInitPos;
        gameObject.transform.rotation = droneInitRot;
        rBody.velocity = Vector3.zero;
        rBody.angularVelocity = Vector3.zero;

        // 가속도, 중력가속도 이런거 초기화 해주기
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

        // 극좌표값을 사용하기위해 설정
        var theta1 = Random.Range(-180f, 180f);
        var theta2 = Random.Range(-180f, 180f);
        var radius = Random.Range(10f, 1000f);

        // 초기화되는 목표의 좌표값 새로 설정
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

        // Vector3 의 형태를 가진 함수를 만들어준다.
        // 그러고 x,y,z 값을 입력해줌
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




        // 둘이 접촉하여 거리가 1.5f 이하되면 점수를 10점 얻고 success 함
        // V2.4 
        // if ((goal.transform.position - gameObject.transform.position).magnitude < 1.5f)
        // V2.5 
        if ((goal.transform.position - gameObject.transform.position).magnitude < 50f)
        {
            SetReward(100);
            // EndEpisode();
            Debug.Log("Success.");
        }
        // 재시작을 위한 조건들을 달아준다
        // 1. x,y,z 값이 50이상 차이나게 되는 경우
        // 2. 드론의 y값이 0.2 이하로 떨어지는 경우
        // 3. 드론의 up의 y가 0.5 이하로 떨어지는 경우
        // 4. y가 0.2 이하일때는 그냥 끝나도록함
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
        // 이부분은 완벽히 이해는 안됐는데 일단 끝나는 타이밍에 목표와 드론의 거리를 계산해서
        // 상점을 계산해서줌
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
