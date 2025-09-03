using System;
using System.Collections;
using System.Collections.Generic;
using Data.Player;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SaveSlot
{
    private const string BACKGROUND = "SlotBackground";
    private const string LOCK_OBJECT = "SlotLock";
    private const string PLAY_TIME_TEXT = "LastPlayTimeText";
    private const string TRASH_BIN_BUTTON = "TrashBinButton";
    private readonly Color EMPTY_SLOT_COLOR = new Color(140.0f, 140.0f, 140.0f, 255.0f);
    private readonly Color OCCUPIED_SLOT_COLOR_HEX = new Color(189.0f, 223.0f, 255.0f, 255.0f);

    public Image BackgroundImage;
    public GameObject SlotLock;
    public TextMeshProUGUI LastSaveTimeText;
    public Button TrashBinButton;
    public bool IsSlotUsed;

    public SaveSlot(Image background, GameObject lockObject, TextMeshProUGUI saveTime, Button binButton, bool used = false)
    {
        BackgroundImage = background;
        SlotLock = lockObject;
        LastSaveTimeText = saveTime;
        TrashBinButton = binButton;
        IsSlotUsed = used;
    }

    public static SaveSlot CreateSlot(GameObject slot)
    {
        Image background = Util.FindChild<Image>(slot, BACKGROUND, true);
        GameObject slotLock = Util.FindChild(slot, LOCK_OBJECT, true);
        TextMeshProUGUI timeText = Util.FindChild<TextMeshProUGUI>(slot, PLAY_TIME_TEXT, true);
        Button binButton = Util.FindChild<Button>(slot, TRASH_BIN_BUTTON, true);

        return new SaveSlot(background, slotLock, timeText, binButton);
    }

    public void SetSlotStatus(bool isUsed, string time = "")
    {
        IsSlotUsed = isUsed;
        LastSaveTimeText.text = $"마지막 세이브 시간 : \n{time}";

        if (isUsed)
        {
            BackgroundImage.color = OCCUPIED_SLOT_COLOR_HEX;
            SlotLock.SetActive(false);
            TrashBinButton.gameObject.SetActive(true);
        }
        else
        {
            BackgroundImage.color = EMPTY_SLOT_COLOR;
            SlotLock.SetActive(true);
            TrashBinButton.gameObject.SetActive(false);
        }
    }
}

public class UI_SaveLoad : UI_Base
{
    private enum GameObjects
    {
        SaveSlot0,
        SaveSlot1,
        SaveSlot2,
        SaveSlot3,
        SaveSlot4
    }

    private enum Buttons
    {
        ExitButton
    }

    private const string SLOT_NAME_PREFIX = "SaveSlot";
    private UserDataManager userDataManager => Managers.Data.UserDataManager;
    private enum Mode { Save, Load };
    private List<SaveSlot> _saveSlots = new List<SaveSlot>();
    private Mode _currentMode;
    private int _selectedSlotNumber = 0;

    public void SetMode(int mode)
    {
        if (Enum.IsDefined(typeof(Mode), mode))
        {
            _currentMode = (Mode)mode;
        }
        else
        {
            _currentMode = Mode.Save;
        }
    }

    private int GetSlotNumber(string name)
    {
        if (name.StartsWith(SLOT_NAME_PREFIX))
        {
            string numberString = name.Substring(SLOT_NAME_PREFIX.Length);

            if (int.TryParse(numberString, out int number))
            {
                return number;
            }
        }

        return 0;
    }

    private void OnSlotClicked(PointerEventData data)
    {
        _selectedSlotNumber = GetSlotNumber(data.pointerClick.name);
        bool isUsed = _saveSlots.SafeGetListValue(_selectedSlotNumber).IsSlotUsed;

        if (_currentMode == Mode.Save)
        {
            MessageID id = isUsed ? MessageID.ReCreateSaveData : MessageID.CreateSaveData;

            Managers.UI.ShowMessageBoxUI(id, CreateCharacterSetting);
        }
        else if (_currentMode == Mode.Load)
        {
            if (!isUsed)
            {
                return;
            }

            Managers.Game.StartGame(_selectedSlotNumber);
        }
    }

    private void CreateCharacterSetting()
    {
        UI_CharacterSetting characterSetting = Managers.UI.ShowUI<UI_CharacterSetting>();
        characterSetting.SetSelectedSlotId(_selectedSlotNumber);
    }

    private void DeleteSaveFile()
    {
        Managers.Data.DeleteData(_selectedSlotNumber);
        _saveSlots.SafeGetListValue(_selectedSlotNumber).SetSlotStatus(false);
    }

    private void OnTrashBinButtonClicked(PointerEventData data)
    {
        _selectedSlotNumber = GetSlotNumber(data.pointerClick.transform.parent.name);
        Managers.UI.ShowMessageBoxUI(MessageID.DeleteSaveData, DeleteSaveFile);
    }

    private void OnExitButtonClicked(PointerEventData data)
    {
        Managers.UI.CloseUI(this.gameObject);
    }

    private void BindUIElements()
    {
        Bind<Button>(typeof(Buttons));
        Bind<GameObject>(typeof(GameObjects));
    }

    private void SetSlotStatus(int slotNumber, SaveSlot slot)
    {
        PlayerData data = userDataManager.GetSaveData(slotNumber);

        if (data != null)
        {
            slot.SetSlotStatus(true, data.saveTime);
            slot.TrashBinButton.gameObject.BindEvent(OnTrashBinButtonClicked);
        }
        else
        {
            slot.SetSlotStatus(false);
        }
    }

    private void InitSlotElements()
    {
        foreach (GameObjects slot in Enum.GetValues(typeof(GameObjects)))
        {
            GameObject slotObject = GetObject((int)slot);

            if (slotObject == null)
            {
                continue;
            }

            SaveSlot saveSlot = SaveSlot.CreateSlot(slotObject);
            int slotNumber = GetSlotNumber(slotObject.name);

            SetSlotStatus(slotNumber, saveSlot);
            slotObject.BindEvent(OnSlotClicked);
            _saveSlots.Add(saveSlot);
        }
    }

    private void BindButtonEvent()
    {
        GetButton((int)Buttons.ExitButton).gameObject.BindEvent(OnExitButtonClicked);
    }

    public override void Init()
    {
        BindUIElements();
        InitSlotElements();
        BindButtonEvent();
    }
}
