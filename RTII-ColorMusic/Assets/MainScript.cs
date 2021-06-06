using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

//sauces : https://www.youtube.com/watch?v=c6NXkZWXHnc
public class MainScript : MonoBehaviour

{
    
    
    
    private float sampling_frequency = 48000;
    

    public float frequency;
    public float gain = 0.05f;
    private float increment;
    private float phase;
  
    
    
    
    


    private bool IsCameraAvaliable;

    public WebCamTexture PhoneCamera;

    private Texture defaultBackground;

    public RawImage background;
    public AspectRatioFitter fit;

    public int CenterX = Screen.width / 2;
    public int CenterY = Screen.height / 2;
    
    
    public float scale(float OldMin, float OldMax, float NewMin, float NewMax, float OldValue){
 
        float OldRange = (OldMax - OldMin);
        float NewRange = (NewMax - NewMin);
        float NewValue = (((OldValue - OldMin) * NewRange) / OldRange) + NewMin;
 
        return(NewValue);
    }
   
    void OnAudioFilterRead(float[] data, int channels)
    {
        float tonalPart = 0;
        
        // update increment in case frequency has changed
        increment = frequency * 2f * Mathf.PI / sampling_frequency;

        for (int i = 0; i < data.Length; i++)
        {
            
            

            phase = phase + increment;
            if (phase > 2 * Mathf.PI) phase = 0;
            
            //tone
            tonalPart = (1f ) * (float)(gain * Mathf.Sin(phase));
            
            data[i] = tonalPart;

            // if we have stereo, we copy the mono data to each channel
            if (channels == 2)
            {
                data[i + 1] = data[i];
                i++;
            }

            
        }

        void Awake()
        {
            sampling_frequency = AudioSettings.outputSampleRate;
            Update();
        }
        
        
    }
    void Start()
    {
       
      
        defaultBackground = background.texture;
        WebCamDevice[] devices = WebCamTexture.devices;

        if (devices.Length == 0)
        {
            Debug.Log("No camera");
            IsCameraAvaliable = false;
            return;
        }

        for (int i = 0; i < devices.Length; i++)
        {
            if (!devices[i].isFrontFacing)
            {
                PhoneCamera = new WebCamTexture(devices[i].name, Screen.width, Screen.height);
            }
        }

        if (PhoneCamera == null)
        {
            Debug.Log("No camera");
            return;

        }

        PhoneCamera.Play();
        background.texture = PhoneCamera;
        IsCameraAvaliable = true;
        
        
    }


    void Update()
    {

        if (!IsCameraAvaliable)
        {
            return;
        }

        float ratio = (float) PhoneCamera.width / (float) PhoneCamera.height;
        fit.aspectRatio = ratio;

        float scaleY = PhoneCamera.videoVerticallyMirrored ? -1f : 1f;
        background.rectTransform.localScale = new Vector3(1f, scaleY, 1f);

        int orient = -PhoneCamera.videoRotationAngle;

        background.rectTransform.localEulerAngles = new Vector3(0, 0, orient);

        PhoneCamera.GetPixel(CenterX, CenterY);

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

        RedAvg = (RedAvg / (41 * 41)) * 255;
        GreenAvg = (GreenAvg / (41 * 41)) * 255;
        BlueAvg = (BlueAvg / (41 * 41)) * 255;

       


        //float Red = PhoneCamera.GetPixel(CenterX,CenterY).r;
        //float Green = PhoneCamera.GetPixel(CenterX,CenterY).g; 
        //float Blue = PhoneCamera.GetPixel(CenterX,CenterY).b; 

        //print(RedAvg);
        //print(BlueAvg);
        //print(GreenAvg);
        //float combined = RedAvg + GreenAvg + BlueAvg;

        //frequency=scale(0, 3, 440, 4000, combined);
        //print(scale(0, 3, 440, 4000, combined));
        // print("RED"); 
        // print(RedAvg);
        // print("GREEN"); 
        // print(GreenAvg);
        // print("BLUE"); 
        // print(BlueAvg);


        if (RedAvg <= 80  && GreenAvg <= 80 && BlueAvg <= 80)
        {
            frequency = 349.2f;
            print("F");
        }
        else if (RedAvg >= 170 && GreenAvg <= 75 && BlueAvg <= 75)
        {
            frequency = 392;
            print("G");
        }
        else if (RedAvg >= 170 && GreenAvg >= 170  && BlueAvg <= 75)
        {
            frequency = 440;
            print("A");
        }
        else if (RedAvg <= 75 && GreenAvg >= 170 && BlueAvg <= 75)
        {
            frequency = 493.9f;
            print("B");
        }
        else if (RedAvg <= 75 && GreenAvg <= 75 && BlueAvg >= 170)
        { 
            frequency = 523.3f;
            print("C");
        }
        else if (RedAvg >= 170 && GreenAvg <= 75 && BlueAvg >= 170)
        { 
            frequency = 587.3f;
            print("D");
        }
        else if (RedAvg >= 170 && GreenAvg >= 170 && BlueAvg >= 170)
        { 
            frequency = 659.3f;
            print("E");
        }
        
        



    }
    }
