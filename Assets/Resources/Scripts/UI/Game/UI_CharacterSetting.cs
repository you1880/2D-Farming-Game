using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_CharacterSetting : UI_Base
{
    public enum Buttons
    {
        MaleButton,
        FemaleButton,
        OkButton,
        BackButton
    }

    public enum Texts
    {
        MaleButtonText,
        FemaleButtonText
    }

    public enum GameObjects
    {
        PlayerNameInputField,
        FarmNameInputField
    }

    public enum Images
    {
        CharacterImage
    }

    [SerializeField] Sprite _manSprite;
    [SerializeField] Sprite _womanSprite;
    private TMP_InputField _nameField;
    private TMP_InputField _farmNameField;
    private TextMeshProUGUI _maleButtonText;
    private TextMeshProUGUI _femaleButtonText;
    private Image _playerImage;
    private int _selectedSlotId = 0;
    private Define.PlayerGender _currentGender = Define.PlayerGender.Man;

    public void SetSelectedSlotId(int slotId)
    {
        _selectedSlotId = slotId;
    }

    private void SetGenderState(Define.PlayerGender gender)
    {
        if (_currentGender == gender)
        {
            return;
        }

        _currentGender = gender;

        switch (gender)
        {
            case Define.PlayerGender.Man:
                _playerImage.sprite = _manSprite;
                _maleButtonText.color = Color.white;
                _femaleButtonText.color = Color.gray;
                break;
            case Define.PlayerGender.Woman:
                _playerImage.sprite = _womanSprite;
                _maleButtonText.color = Color.gray;
                _femaleButtonText.color = Color.white;
                break;
        }
    }

    private void OnMaleButtonClicked(PointerEventData data)
    {
        SetGenderState(Define.PlayerGender.Man);
    }

    private void OnFemaleButtonClicked(PointerEventData data)
    {
        SetGenderState(Define.PlayerGender.Woman);
    }

    private void OnOkButtonClicked(PointerEventData data)
    {
        string name = _nameField.text;
        string farm = _farmNameField.text;

        if (string.IsNullOrEmpty(name))
        {
            Managers.UI.ShowMessageBoxUI(MessageID.NameNotFilled, null, true);

            return;
        }

        if (string.IsNullOrEmpty(farm))
        {
            Managers.UI.ShowMessageBoxUI(MessageID.FarmNameNotFilled, null, true);

            return;
        }

        Managers.Game.StartNewGame(_selectedSlotId, name, farm, _currentGender);
    }

    private void OnBackButtonClicked(PointerEventData data)
    {
        Managers.UI.CloseUI(this.gameObject);
    }

    private void BindUIElements()
    {
        Bind<Button>(typeof(Buttons));
        Bind<TextMeshProUGUI>(typeof(Texts));
        Bind<GameObject>(typeof(GameObjects));
        Bind<Image>(typeof(Images));
    }

    private void GetUIElements()
    {
        _maleButtonText = Get<TextMeshProUGUI>((int)Texts.MaleButtonText);
        _femaleButtonText = Get<TextMeshProUGUI>((int)Texts.FemaleButtonText);
        _nameField = Get<GameObject>((int)GameObjects.PlayerNameInputField).GetComponent<TMP_InputField>();
        _farmNameField = Get<GameObject>((int)GameObjects.FarmNameInputField).GetComponent<TMP_InputField>();
        _playerImage = Get<Image>((int)Images.CharacterImage);
    }

    private void BindButtonEvent()
    {
        GetButton((int)Buttons.MaleButton).gameObject.BindEvent(OnMaleButtonClicked);
        GetButton((int)Buttons.FemaleButton).gameObject.BindEvent(OnFemaleButtonClicked);
        GetButton((int)Buttons.OkButton).gameObject.BindEvent(OnOkButtonClicked);
        GetButton((int)Buttons.BackButton).gameObject.BindEvent(OnBackButtonClicked);
    }

    public override void Init()
    {
        BindUIElements();
        GetUIElements();
        BindButtonEvent();
    }

    private void OnDisable()
    {
        SetGenderState(Define.PlayerGender.Man);
        _nameField.text = "";
        _farmNameField.text = "";
    }
}
