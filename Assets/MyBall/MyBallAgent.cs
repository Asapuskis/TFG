using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyBallAgent : Agent
{
    public GameObject pelota;

    //Observaciones del entorno que recoge el agente
    public override void CollectObservations()
    {

        AddVectorObs(gameObject.transform.rotation.z);

        AddVectorObs(gameObject.transform.rotation.x);

        AddVectorObs((pelota.transform.position - gameObject.transform.position));

        AddVectorObs(pelota.transform.GetComponent<Rigidbody>().velocity);

    }

    //Función de recompensa del agente.
    public override void AgentAction(float[] vectorAction, string textAction)
    {
        if (brain.brainParameters.vectorActionSpaceType == SpaceType.continuous)
        {
            float action_z = 2f * Mathf.Clamp(vectorAction[0], -1f, 1f);
            if ((gameObject.transform.rotation.z < 0.25f && action_z > 0f) ||
                (gameObject.transform.rotation.z > -0.25f && action_z < 0f))
            {
                gameObject.transform.Rotate(new Vector3(0, 0, 1), action_z);
            }
            float action_x = 2f * Mathf.Clamp(vectorAction[1], -1f, 1f);
            if ((gameObject.transform.rotation.x < 0.25f && action_x > 0f) ||
                (gameObject.transform.rotation.x > -0.25f && action_x < 0f))
            {
                gameObject.transform.Rotate(new Vector3(1, 0, 0), action_x);
            }

            SetReward(0.1f);

        }

        //Si la pelota está por debajo de la plataforma, se dará por concluido la iteración del entrenamiento.
        if ((pelota.transform.position.y - gameObject.transform.position.y) < -2f ||
            Mathf.Abs(pelota.transform.position.x - gameObject.transform.position.x) > 3f ||
            Mathf.Abs(pelota.transform.position.z - gameObject.transform.position.z) > 3f)
        {
            Done();
            SetReward(-1f);
        }


    }

    //Al iniciar una nueva iteración del entrenamiento, se reiniciarán a estos valores.
    public override void AgentReset()
    {
        gameObject.transform.rotation = new Quaternion(0f, 0f, 0f, 0f);
        gameObject.transform.Rotate(new Vector3(1, 0, 0), Random.Range(-10f, 10f));
        gameObject.transform.Rotate(new Vector3(0, 0, 1), Random.Range(-10f, 10f));
        pelota.GetComponent<Rigidbody>().velocity = new Vector3(0f, 0f, 0f);
        pelota.transform.position = new Vector3(Random.Range(-1.5f, 1.5f), 4f, Random.Range(-1.5f, 1.5f)) + gameObject.transform.position;

    }

}
