using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class Clock : MonoBehaviour
{
    private const int AddedValueForSunAlignment = -110; //Because 0,0,0 rotation does not correspond to 24:00 or 12:00
    private const float degreesPerHour = 30f,
        degreesPerMinute = 6f,
        degreesPerSecond = 6f;
    private DateTime time;
    private TimeSpan timeSpan;
    private bool lightsOn = false;
    public bool continuous, manual;
    public int hours = 0, minutes = 0, seconds = 0, angle = 0;
    public float sunRotSpeed = 1;
    public Transform hoursTransform, minutesTransform, secondsTransform, backlightTransform;
    public TextMeshProUGUI digitalText;
    public Material[] materials;
    public GameObject spotLight, smallLights, directionalLight;

    private void Start()
    {
        UpdateLight();
        StartCoroutine(ActivateLights());
    }

    void Update()
    {
        UpdateTime();
        UpdateDigitalTime();
        UpdateClockTime();
        UpdateLight();
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ToggleLights();
        }
    }

    private IEnumerator ActivateLights(){
        yield return new WaitForEndOfFrame();
        if (angle % 360 < -13 || angle % 360 > 190)
        {
            ToggleLights();
        }
    }

    private void ToggleLights()
    {
        if (lightsOn)
        {
            hoursTransform.GetChild(0).GetComponent<Renderer>().material = materials[6];
            minutesTransform.GetChild(0).GetComponent<Renderer>().material = materials[0];
            secondsTransform.GetChild(0).GetComponent<Renderer>().material = materials[3];
            backlightTransform.GetComponent<Renderer>().material = materials[4];
            spotLight.SetActive(false);
            smallLights.SetActive(false);
        }
        else
        {
            hoursTransform.GetChild(0).GetComponent<Renderer>().material = materials[7];
            minutesTransform.GetChild(0).GetComponent<Renderer>().material = materials[1];
            secondsTransform.GetChild(0).GetComponent<Renderer>().material = materials[2];
            backlightTransform.GetComponent<Renderer>().material = materials[5];
            spotLight.SetActive(true);
            smallLights.SetActive(true);
        }
        lightsOn = !lightsOn;
    }

    // Logic to decide how to adjust the clock (Manual, Continuous, Discreet)
    private void UpdateClockTime()
    {
        if (manual)
        {
            ManualSetTime();
        }
        else
        {
            if (continuous)
            {
                timeSpan = DateTime.Now.TimeOfDay;
                UpdateContinuous();
            }
            else
            {
                UpdateDiscreet();
            }
        }
    }

    private void UpdateTime()
    {
        time = DateTime.Now;
        if (!manual)
        {
            hours = time.Hour;
            minutes = time.Minute;
            seconds = time.Second;
        }
    }

    //Should be in different script normally.
    private void UpdateLight()
    {
        angle = (int)(hours * 15 + minutes * 0.25) + AddedValueForSunAlignment;
        directionalLight.transform.rotation = Quaternion.SlerpUnclamped(directionalLight.transform.rotation, Quaternion.Euler(angle, 8, 0), sunRotSpeed * Time.deltaTime);
    }

    // Requires (bool) manual = true. Allows the manual change of the time of the day. (Inspector)
    private void ManualSetTime()
    {
        hoursTransform.localRotation = Quaternion.Euler(0f, hours * degreesPerHour, 0f);
        minutesTransform.localRotation = Quaternion.Euler(0f, minutes * degreesPerMinute, 0f);
        secondsTransform.localRotation = Quaternion.Euler(0f, seconds * degreesPerSecond, 0f);
    }

    private void UpdateDiscreet()
    {
        hoursTransform.localRotation = Quaternion.Euler(0f, time.Hour * degreesPerHour, 0f);
        minutesTransform.localRotation = Quaternion.Euler(0f, time.Minute * degreesPerMinute, 0f);
        secondsTransform.localRotation = Quaternion.Euler(0f, time.Second * degreesPerSecond, 0f);
    }

    private void UpdateContinuous()
    {
        hoursTransform.localRotation = Quaternion.Euler(0f, (float)timeSpan.TotalHours * degreesPerHour, 0f);
        minutesTransform.localRotation = Quaternion.Euler(0f, (float)timeSpan.TotalMinutes * degreesPerMinute, 0f);
        secondsTransform.localRotation = Quaternion.Euler(0f, (float)timeSpan.TotalSeconds * degreesPerSecond, 0f);
    }

    // Updates digital clock (text).
    private void UpdateDigitalTime() => digitalText.text = IntToTwoDigitString(hours, 24) + ":" + IntToTwoDigitString(minutes, 60) + ":" + IntToTwoDigitString(seconds, 60);

    private string IntToTwoDigitString(int num, int modulo)
    {
        if (num < 0) num *= -1;
        num = num % modulo;

        if (num < 10)
        {
            return "0" + num.ToString();
        }
        else
        {
            return num.ToString();
        }
    }
}
