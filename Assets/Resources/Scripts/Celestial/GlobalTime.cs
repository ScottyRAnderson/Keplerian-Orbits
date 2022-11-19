using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GlobalTime : MonoBehaviour
{
    [SerializeField]
    private float tickRate = 1f;

    [SerializeField][Range(0f, 60f)]
    private float minute;
    [SerializeField][Range(0f, 60f)]
    private float hour;
    [SerializeField][Range(0f, 24f)]
    private float day;
    [SerializeField][Range(0f, 365f)]
    private float year;

    private float rawSecond;
    private float rawMinute;
    private float rawHour;
    private float rawDay;
    private float rawYear;

    private static GlobalTime instance;

    public static float TimeStep { get { return Time.deltaTime; } }
    public static float MinuteTick { get { return Instance.tickRate * TimeStep; } }
    public static float HourTick { get { return (Instance.tickRate / 60f) * TimeStep; } }
    public static float DayTick { get { return (Instance.tickRate / (60f * 60f)) * TimeStep; } }
    public static float YearTick { get { return (Instance.tickRate / (60f * 60f * 24f)) * TimeStep; } }

    public static float Second { get { return Instance.rawSecond % 60f; } }
    public static float Minute { get { return (int)(Instance.rawMinute / 60f) % 60f; } }
    public static float Hour { get { return (int)(Instance.rawHour / 60f) % 24f; } }
    public static float Day { get { return (int)(Instance.rawDay / 24f) % 365f; } }
    public static float Year { get { return (int)(Instance.rawYear / 365f); } }

    public static GlobalTime Instance
    {
        get
        {
            if (!instance){
                instance = FindObjectOfType<GlobalTime>();
            }
            return instance;
        }
    }

    public static TimeEvent OnValuesChanged;
    public delegate void TimeEvent();

    private void OnValidate(){
        OnValuesChanged?.Invoke();
    }

    private void Update()
    {
        if (Application.isPlaying){
            UpdateTimeStep();
        }
    }

    public static void SetTickRate(float tickRate){
        Instance.tickRate = tickRate;
    }

    private void UpdateTimeStep()
    {
        minute += MinuteTick;
        hour += HourTick;
        day += DayTick;
        year += YearTick;

        // Numbers are getting too big to read from...
        rawSecond += MinuteTick;
        rawMinute += MinuteTick;
        rawHour += HourTick;
        rawDay += DayTick;
        rawYear += YearTick;

        minute %= 60f;
        hour %= 60f;
        day %= 24f;
        year %= 365f;
    }
}