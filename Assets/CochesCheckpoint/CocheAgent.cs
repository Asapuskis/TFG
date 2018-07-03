using System.Collections.Generic;
using UnityEngine;

public class CocheAgent : Agent
{
    public Vector3 carStartPosition;
    private int colision = 0; //1 ha chocado con una pared. 2 ha llegado a meta.

    private void Start()
    {
        carStartPosition = gameObject.transform.position;
    }

    public override void CollectObservations()
    {

        //Posicion 'x' y 'z' del gameObject, en este caso el coche.
        AddVectorObs(this.transform.position.x);
        AddVectorObs(this.transform.position.z);

        //La rotación del coche, en este caso, el eje 'y' será sobre el que rotará el coche al girar en las curvas.
        AddVectorObs(this.transform.rotation.y);

    }

    public override void AgentAction(float[] vectorAction, string textAction)
    {

        switch (colision)
        {
            case 1: //Ha chocado con una pared
                Done();
                AddReward(-1f);
                break;
            case 2: //Ha llegado a meta
                Done();
                AddReward(1f);
                break;
            case 3: //Ha pasado por el checkpoint
                AddReward(0.2f);
                break;
        }

        colision = 0;

        gameObject.transform.Translate(2f * Time.deltaTime, 0f, 0f);

        gameObject.transform.Rotate(new Vector3(0, vectorAction[0], 0), 1);

    }

    public override void AgentReset()
    {
        gameObject.transform.rotation = new Quaternion(0f, 0f, 0f, 0f);
        gameObject.transform.position = carStartPosition;
        colision = 0;
        //Debug.Log(GetCumulativeReward());
    }

    private void OnCollisionEnter(Collision collision)
    {
        //Debug.Log(collision.gameObject.name);
        if (collision.gameObject.name == "Paredes")
        {
            //Debug.Log("Ha chocado contra una pared.");
            colision = 1;
        }
        else if (collision.gameObject.name == "Meta")
        {
            //Debug.Log("Ha llegado a meta.");
            colision = 2;
        }
        else if (collision.gameObject.tag == "CheckPoints")
        {
            colision = 3;
        }
    }

}
