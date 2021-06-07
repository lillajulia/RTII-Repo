using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

//Source : https://www.youtube.com/watch?v=c6NXkZWXHnc
//Source: https://www.benjaminoutram.com/blog/2018/7/13/procedural-audio-in-unity-noise-and-tone
public class MainScript : MonoBehaviour

{
    public bool isMute=false;
    
    public Button octaveButton;
    public Button muteButton;
    public Sprite EnabledMuteSprite;
    public Sprite DisabledMuteSprite;

    private int octave = 1;

    [CanBeNull] private AudioListener _audioListener;
    
    
    private float samplingFrequency = 48000;
    public float frequency;
    public float gain = 0.05f;
    private float increment;
    private float phase;

    public Text notes;


    private bool IsCameraAvaliable;

    public WebCamTexture PhoneCamera;

    private Texture defaultBackground;

    public RawImage background;
    public AspectRatioFitter fit;

    public int CenterX = Screen.width / 2;
    public int CenterY = Screen.height / 2;

    
    void OnAudioFilterRead(float[] data, int channels)
    {
        float tone = 0;
        
        increment = frequency * 2f * Mathf.PI / samplingFrequency;

        for (int i = 0; i < data.Length; i++)
        {
            phase = phase + increment;
            if (phase > 2 * Mathf.PI) phase = 0;
            
            tone = (1f) * (float) (gain * Mathf.Sin(phase));

            data[i] = tone;
            if (channels == 2)
            {
                data[i + 1] = data[i];
                i++;
            }
        }

        void Awake()
        {
            
        }
    }

    void Start()
    {
        octaveButton.onClick.AddListener(octaveSwitch);

        defaultBackground = background.texture;
        WebCamDevice[] devices = WebCamTexture.devices; // start looking for available cameras on the device 

        if (devices.Length == 0) // if devices.length is zero, it means there are no available cameras on the device
        {
            Debug.Log("No camera"); // if there are no available cameras, we print out No Camera
            IsCameraAvaliable = false; // We also set the IsCameraAvailable boolean to false, because there is no cam available. Duh. 
            return; // if there is no camera, the program should not go further
        }

        for (int i = 0; i < devices.Length; i++) // We go through all available cameras on the device, and select the one that is not front facing
        {
            if (!devices[i].isFrontFacing) // if not front facing camera then....
            {
                PhoneCamera = new WebCamTexture(devices[i].name, Screen.width, Screen.height); //...set the device's PhoneCamera texture to the incoming video input
            }
        }

        if (PhoneCamera == null) //if we couldn't find a back facing camera,we should display No camera, and not proceed further
        {
            Debug.Log("No camera");
            return;
        }

        PhoneCamera.Play(); 
        background.texture = PhoneCamera;  // render the incoming video to the background 
        IsCameraAvaliable = true;
    }


    void Update()
    {
        _audioListener = GetComponent<AudioListener>();
        if (!IsCameraAvaliable)
        {
            return;
        }

        float ratio = (float) PhoneCamera.width / (float) PhoneCamera.height; // calculate aspect ratio
        fit.aspectRatio = ratio; //set the aspect ratio

        float scaleY = PhoneCamera.videoVerticallyMirrored ? -1f : 1f; 
        background.rectTransform.localScale = new Vector3(1f, scaleY, 1f);

        int orient = -PhoneCamera.videoRotationAngle;

        background.rectTransform.localEulerAngles = new Vector3(0, 0, orient);

        float RedAvg = 0;
        float GreenAvg = 0;
        float BlueAvg = 0;
        
        for (int i = -20; i <= 20; i++)
        {
            for (int j = -20; j <= 20; j++)
            {
                RedAvg += (PhoneCamera.GetPixel(CenterX - i, CenterY - j).r);
                GreenAvg += (PhoneCamera.GetPixel(CenterX - i, CenterY - j).g);
                BlueAvg += (PhoneCamera.GetPixel(CenterX - i, CenterY - j).b);
            }
        }

        RedAvg = (RedAvg / (41 * 41));
        GreenAvg = (GreenAvg / (41 * 41));
        BlueAvg = (BlueAvg / (41 * 41));

        float H, S, V;

        Color.RGBToHSV(new Color(RedAvg, GreenAvg, BlueAvg, 1), out H, out S, out V);

        H = H * 360;
        S = S * 360;
        V = V * 360;
        
        if (S <= 40 && V <= 60)
        {
            frequency = 261.626f;
            print("C4");
            UpdateNote("C"+(octave+3));
        }
        else if (H <= 30 && S >= 40 && V >= 40)
        {
            frequency = 293.665f;
            print("D4");
            UpdateNote("D"+(octave+3));
        }
        else if (H >= 31 && H <= 70 && S >= 40 && V >= 40)
        {
            frequency = 329.628f;
            print("E4");
            UpdateNote("E"+(octave+3));
        }
        else if (H >= 71 && H <= 150 && S >= 40 && V >= 40)
        {
            frequency = 349.228f;
            print("F4");
            UpdateNote("F"+(octave+3));
        }
        else if (H >= 151 && H <= 240 && S >= 40 && V >= 40)
        {
            frequency = 391.995f;
            print("G4");
            UpdateNote("G"+(octave+3));
        }
        else if (H >= 241 && H <= 330 && S >= 40 && V >= 40)
        {
            frequency = 440;
            print("A4");
            UpdateNote("A"+(octave+3));
        }
        else if (H >= 331 && H <= 360 && S >= 40 && V >= 40)
        {
            frequency = 493.883f;
            print("B4");
            UpdateNote("B"+(octave+3));
        }

        frequency = frequency * octave;

    }

    public void octaveSwitch()
    {
        if (octave < 4)
        {
            octave++;
        }
        else
        {
            octave = 1;
        }
    }
    public void mute()
    {
        if (!isMute)
        {
            
            isMute = true;
            gain = 0;
            muteButton.image.sprite = DisabledMuteSprite;
        }
        else
        {
            isMute = false;
            gain = 0.05f; 
            muteButton.image.sprite = EnabledMuteSprite;
        }
    }

    public void UpdateNote(String note)
    {
        notes.text = note;
    }
}