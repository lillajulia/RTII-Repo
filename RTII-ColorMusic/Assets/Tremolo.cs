using System;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Tremolo : MonoBehaviour
{
    public Button button;
    
    public bool filterOn = false;

    [Range(0, 3)] public float depth = 1f;

    public float rangeTremoloStart = 2000;
    public float rangeTremoloEnd = 8000;

    [Range(2000, 8000)] public float effectRate = 2000;

    static int counter;
    static int tester;
    static float offset;
	
	
    public float scale(float OldMin, float OldMax, float NewMin, float NewMax, float OldValue){
 
        float OldRange = (OldMax - OldMin);
        float NewRange = (NewMax - NewMin);
        float NewValue = (((OldValue - OldMin) * NewRange) / OldRange) + NewMin;
 
        return(NewValue);
    }

	

    void Start()
    {
        counter = 1;
        tester = 0;
        offset = 1 - depth;
        button.onClick.AddListener(onoff);
		
    }
	

    private void Update()
    {
        Accellorometer();
    }

    void OnAudioFilterRead(float[] data, int channels)
    {
        if (!filterOn)
            return;
        for (int i = 0; i < data.Length; i++)
        {
            data[i] = performTremolo(data[i]);
            tremoloShift();
        }
    }

    float performTremolo(float xin)
    {
        float dataOut;
        float aux;

        aux = (float) tester * depth / effectRate;
        dataOut = (aux + offset) * xin;
        return dataOut;
    }

    void tremoloShift()
    {

        tester += counter;
        if (tester > effectRate)
        {
            counter = -1;
        }
        else if (tester == 0)
        {
            counter = 1;
        }
    }

    public void onoff()
    {
        print("BRUH");
        if (!filterOn)
        {
            filterOn = true;
        }
        else
        {
            filterOn = false;
        }

    }

    public void Accellorometer()
    {
        Debug.Log(Input.acceleration.x);
        float rotation = Input.acceleration.x;
        effectRate = map(rotation, -1.0f, 1.0f, rangeTremoloStart, rangeTremoloEnd);
        Debug.Log(effectRate);

    }

    float map(float s, float a1, float a2, float b1, float b2)
    {
        return b1 + (s - a1) * (b2 - b1) / (a2 - a1);
    }

}