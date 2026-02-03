using UnityEngine;
using UnityEngine.Rendering.Universal;

public class DayManager : MonoBehaviour
{
    [SerializeField] private Light2D sun;

    public void SetSun(float value)
    {
        sun.intensity = value;
    }
}
