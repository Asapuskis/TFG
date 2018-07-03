using System.Collections.Generic;
using UnityEngine;

public class CocheDAgent : Agent
{
    public Vector3 carDStartPosition;
    private int Dcolision = 0; //1 ha chocado con una pared. 2 ha llegado a meta.
    RayPerception rayPer;
    public GameObject Meta;

    public override void InitializeAgent()
    {
        rayPer = GetComponent<RayPerception>();
    }

    private void Start()
    {
        carDStartPosition = gameObject.transform.position;
    }

    public override void CollectObservations()
    {

        //0f = (1,0) en circle unit
        //90f = izquierda
        //270f = derecha

        string [] detectableObjects = new string[] { "Paredes", "Meta" };

        float[] rayosLaterales = { 90f, 270f };
        float rayDistanceLaterales = 0.7f;
        AddVectorObs(rayPer.Perceive(rayDistanceLaterales, rayosLaterales, detectableObjects, 0f, 0f));

        float[] rayoFrontal = { 0f };
        float distanciaFrontal = 2f;
        AddVectorObs(rayPer.Perceive(distanciaFrontal, rayoFrontal, detectableObjects, 0f, 0f));

        float[] rayosDiagonales = { 45f, 315f };
        float distanciaDiagonal = 1.2f;
        AddVectorObs(rayPer.Perceive(distanciaDiagonal, rayosDiagonales, detectableObjects, 0f, 0f));

        //La rotación del coche, en este caso, el eje 'y' será sobre el que rotará el coche al girar en las curvas.
        AddVectorObs(this.transform.localRotation.y);

    }

    public override void AgentAction(float[] vectorAction, string textAction)
    {

        gameObject.transform.Translate(2f * Time.deltaTime, 0f, 0f);

        //Al llegar un float en el vector de acción, de esta forma, lo redondeamos a -1, 0 o 1.
        int ejeY = Mathf.RoundToInt(Mathf.Clamp(vectorAction[0], -1, 1));

        gameObject.transform.Rotate(new Vector3(0, ejeY, 0), 1);

        switch (Dcolision)
        {
            case 1: //Ha chocado con una pared
                AddReward(-1f);
                Done();
                break;
            case 2: //Ha llegado a meta
                AddReward(1f);
                Done();
                break;
        }

        //Le recompensamos que aguante en el circuito sin chocar.
        AddReward(0.0005f);

        Dcolision = 0;

    }

    public override void AgentReset()
    {
        gameObject.transform.rotation = new Quaternion(0f, 0f, 0f, 0f);
        gameObject.transform.position = carDStartPosition;
        Dcolision = 0;
        //Debug.Log(GetCumulativeReward());
    }

    private void OnCollisionEnter(Collision collision)
    {
        //Debug.Log(collision.gameObject.name);
        if (collision.gameObject.name == "Paredes")
        {
            //Debug.Log("Ha chocado contra una pared.");
            Dcolision = 1;
        }
        else if (collision.gameObject.name == "Meta")
        {
            //Debug.Log("Ha llegado a meta.");
           Dcolision = 2;
        }
    }
}
