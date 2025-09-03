using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_DayEnd : UI_Base
{
    private enum Buttons
    { 
        NextDayButton
    }

    private void OnNextDayButtonClicked(PointerEventData data)
    {
        Managers.Game.DayEnd();
    }

    private void BindUIElements()
    {
        Bind<Button>(typeof(Buttons));
    }

    private void BindButtonEvent()
    {
        GetButton((int)Buttons.NextDayButton).gameObject.BindEvent(OnNextDayButtonClicked);
    }

    public override void Init()
    {
        BindUIElements();
        BindButtonEvent();
    }
}
