using System;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Tremolo : MonoBehaviour
{
    public bool filterOn = false;

    public Button TremoloButton;
    public Sprite DisabledTremoloSprite;
    public Sprite EnabledTremoloSprite;


    [Range(0, 3)] public float depth = 1f;

    public float rangeTremoloStart = 2000;
    public float rangeTremoloEnd = 8000;

    [Range(2000, 8000)] public float effectRate = 2000;

    static int counter;
    static int tester;
    static float offset;

    void Start()
    {
        counter = 1;
        tester = 0;
        offset = 1 - depth;
        TremoloButton.onClick.AddListener(onOff);
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

    public void onOff()
    {
      
        if (!filterOn)
        {
            filterOn = true;
            TremoloButton.image.sprite = EnabledTremoloSprite;
        }
        else
        {
            filterOn = false;
            TremoloButton.image.sprite = DisabledTremoloSprite;
        }
    }

    public void Accellorometer()
    {
        float rotation = Input.acceleration.x;
        effectRate = map(rotation, -1.0f, 1.0f, rangeTremoloStart, rangeTremoloEnd);
    }

    float map(float s, float a1, float a2, float b1, float b2)
    {
        return b1 + (s - a1) * (b2 - b1) / (a2 - a1);
    }
}