using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class StreetLamp : MonoBehaviour
{
    [SerializeField] Light2D _lampLight;
    private bool _isOn = false;

    private void TurnLight()
    {
        Define.TimeSection timeSection = Managers.Time.GetTimeSection();

        if (timeSection == Define.TimeSection.Evening || timeSection == Define.TimeSection.Night)
        {
            if (_isOn)
            {
                return;
            }

            _isOn = true;
            _lampLight.enabled = true;
        }
        else
        {
            if (!_isOn)
            {
                return;
            }

            _isOn = false;
            _lampLight.enabled = false;
        }
    }

    private void OnEnable()
    {
        Managers.Time.OnTimeChanged -= TurnLight;
        Managers.Time.OnTimeChanged += TurnLight;
    }

    private void OnDisable()
    {
        Managers.Time.OnTimeChanged -= TurnLight;
    }
}
