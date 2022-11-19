using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class HUDInterface : MonoBehaviour
{
    [SerializeField]
    private Slider timestepSlider;
    [SerializeField]
    private TextMeshProUGUI timestepReadout;
    [SerializeField]
    private TextMeshProUGUI yearReadout;
    [SerializeField]
    private TextMeshProUGUI dayReadout;
    [SerializeField]
    private TextMeshProUGUI hourReadout;
    [SerializeField]
    private TextMeshProUGUI minuteReadout;
    [SerializeField]
    private TextMeshProUGUI secondReadout;

    [Space]

    [SerializeField]
    private TextMeshProUGUI observedHeader;
    [SerializeField]
    private observedDistance[] observedDistances;

    private SolarCamController camController;

    private void Awake()
    {
        camController = FindObjectOfType<SolarCamController>();

        // Sort distances, smallest to largest
        observedDistances = observedDistances.OrderBy(o => o.DistanceThreshold).ToArray();
    }

    private void Update()
    {
        UpdateTimeStepGUI();
        UpdateObservableGUI();
    }

    private void UpdateTimeStepGUI()
    {
        int timestepValue = (int)(timestepSlider.value - (timestepSlider.maxValue / 2f));
        timestepReadout.text = timestepValue == 0 ? "RT" : timestepValue.ToString();

        float percent = Mathf.Abs(timestepValue) / (timestepSlider.maxValue / 2f);
        float scalar = 20000000 * percent / 20f;
        GlobalTime.SetTickRate(timestepValue == 0 ? 1 : timestepValue * scalar);

        yearReadout.text = GlobalTime.Year.ToString("F0");
        dayReadout.text = GlobalTime.Day.ToString("F0");
        hourReadout.text = GlobalTime.Hour.ToString("F0");
        minuteReadout.text = GlobalTime.Minute.ToString("F0");
        secondReadout.text = GlobalTime.Second.ToString("F0");
    }

    private void UpdateObservableGUI()
    {
        float distance = camController.transform.position.magnitude;
        for (int i = 0; i < observedDistances.Length; i++)
        {
            observedDistance observable = observedDistances[i];
            if (distance <= observable.DistanceThreshold)
            {
                observedHeader.text = observable.Header;
                break;
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        for (int i = 0; i < observedDistances.Length; i++)
        {
            Gizmos.color = observedDistances[i].DebugColor;
            Gizmos.DrawWireSphere(Vector3.zero, observedDistances[i].DistanceThreshold);
        }
    }

    [System.Serializable]
    private struct observedDistance
    {
        [SerializeField]
        private string header;
        [SerializeField]
        private float distanceThreshold;
        [SerializeField]
        private Color debugColor;

        public string Header { get { return header; } }
        public float DistanceThreshold { get { return distanceThreshold; } }
        public Color DebugColor { get { return debugColor; } }
    }
}