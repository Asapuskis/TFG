using System;
using UnityEngine;

public class CocheDAgentV2 : Agent
{
    public Vector3 carDStartPosition2;
    private int Dcolision2 = 0; //1 ha chocado con una pared. 2 ha llegado a meta.
    RayPerception rayPerc;
    private DateTime Tinicio;
    private DateTime Tfin;

    public override void InitializeAgent()
    {
        rayPerc = GetComponent<RayPerception>();
    }

    private void Start()
    {
        carDStartPosition2 = gameObject.transform.position;
        Tinicio = DateTime.Now;
    }

    public override void CollectObservations()
    {

        //0f = (1,0) en circle unit
        //90f = izquierda
        //270f = derecha

        string [] detectableObjects = new string[] { "Paredes", "Meta" };

        float[] rayosLaterales = { 90f, 270f };
        float rayDistanceLaterales = 0.7f;
        AddVectorObs(rayPerc.Perceive(rayDistanceLaterales, rayosLaterales, detectableObjects, 0f, 0f));

        float[] rayoFrontal = { 0f };
        float distanciaFrontal = 2.5f;
        AddVectorObs(rayPerc.Perceive(distanciaFrontal, rayoFrontal, detectableObjects, 0f, 0f));

        float[] rayosDiagonales = { 45f, 315f };
        float distanciaDiagonal = 1.2f;
        AddVectorObs(rayPerc.Perceive(distanciaDiagonal, rayosDiagonales, detectableObjects, 0f, 0f));

        float[] rayosDiagonales2 = { 22f, 337f };
        float distanciaDiagonal2 = 1.8f;
        AddVectorObs(rayPerc.Perceive(distanciaDiagonal2, rayosDiagonales2, detectableObjects, 0f, 0f));

        //La rotación del coche, en este caso, el eje 'y' será sobre el que rotará el coche al girar en las curvas.
        AddVectorObs(this.transform.localRotation.y);

    }

    public override void AgentAction(float[] vectorAction, string textAction)
    {

        gameObject.transform.Translate(2f * Time.deltaTime, 0f, 0f);

        gameObject.transform.Rotate(new Vector3(0, vectorAction[0], 0), 1);

        switch (Dcolision2)
        {
            case 1: //Ha chocado con una pared
                AddReward(-1f);
                Done();
                break;
            case 2: //Ha llegado a meta
                //AddReward(1f);
                Tfin = DateTime.Now;
                TimeSpan segundos = Tfin.Subtract(Tinicio);
                float tiemp = Mathf.Clamp((float)segundos.TotalMilliseconds, 0, 1f);
                AddReward(1f - tiemp);
                Done();
                break;
        }

        //Le recompensamos que aguante en el circuito sin chocar.
        AddReward(0.0001f);

        Dcolision2 = 0;

    }

    public override void AgentReset()
    {
        gameObject.transform.rotation = new Quaternion(0f, 0f, 0f, 0f);
        gameObject.transform.position = carDStartPosition2;
        Dcolision2 = 0;
        Tinicio = DateTime.Now;

        Debug.Log(GetCumulativeReward());
    }

    private void OnCollisionEnter(Collision collision)
    {
        //Debug.Log(collision.gameObject.name);
        if (collision.gameObject.name == "Paredes")
        {
            //Debug.Log("Ha chocado contra una pared.");
            Dcolision2 = 1;
        }
        else if (collision.gameObject.name == "Meta")
        {
            //Debug.Log("Ha llegado a meta.");
           Dcolision2 = 2;
        }
    }

}
