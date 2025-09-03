using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] private Animator _doorAnimatior;
    public enum DoorState { Close, Opening, Closing };
    private DoorState _currentDoorState = DoorState.Close;

    public DoorState CurrentDoorState
    {
        get { return _currentDoorState; }
        set
        {
            if (_currentDoorState == value)
            {
                return;
            }

            _currentDoorState = value;
            string animationName = GetAnimationName();
            if (string.IsNullOrEmpty(animationName) || _doorAnimatior == null)
            {
                return;
            }

            _doorAnimatior.Play(animationName);
        }
    }

    public IEnumerator OpenDoor()
    {
        CurrentDoorState = DoorState.Opening;

        yield return new WaitForSeconds(1.0f);

        CurrentDoorState = DoorState.Closing;
    }

    private string GetAnimationName()
    {
        return Enum.GetName(typeof(DoorState), _currentDoorState);
    }
}
