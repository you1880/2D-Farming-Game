using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crop : Prop, IPointInteractable
{
    [SerializeField] private SpriteRenderer _cropSpriteRenderer;

    protected override void Init()
    {
        PropType = Define.PropType.Crop;
    }

    public void SetCropStatus(Define.CropType cropType, int growLevel, bool isDead = false)
    {
        SetCropSprite(cropType, growLevel, isDead);
    }

    private void SetCropSprite(Define.CropType cropType, int growLevel, bool isDead)
    {
        _cropSpriteRenderer.sprite = Managers.Resource.LoadCropSprite(cropType, growLevel, isDead);
    }

    public override void TryBreakProp(int damage = 1)
    {
        if (Managers.Game.TryBreakProp(_objectPosition, PropType, _dropTable))
        {
            Managers.Prop.RemovePropFromDict(_objectPosition);
        }
    }

    public bool IsInteractable(GameObject caller, Vector3 mousePosition)
    {
        return Util.IsToolReachable(mousePosition, caller.transform.position);
    }
    
    public void Interact(GameObject caller, Vector3 mousePosition)
    {
        if (!caller.CompareTag("Player") || !IsInteractable(caller, mousePosition))
        {
            return;
        }

        Managers.Game.TryHarvestCropAtPosition(mousePosition, transform.position);
    }
}
