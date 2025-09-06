using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Define
{
    public enum SceneType
    {
        Unknown,
        Lobby,
        Main,
        DayEnd,
        Cave,
    }

    public enum UIEvent
    {
        Click,
        PointerEnter,
        PointerExit,
    }

    #region KeyBoard
    public enum KeyBoardEvent
    {
        Up,
        Right,
        Down,
        Left,
        Inventory,
        Setting,
        Num1, Num2, Num3, Num4, Num5, Num6, Num7, Num8, Num9
    }

    public enum InputKeyType
    {
        Hold,
        Down,
    }
    #endregion

    public enum MouseEvent
    {
        LClick,
        RClick
    }

    #region Sound
    public enum SoundType
    {
        Bgm,
        Effect,
        Count,
    }

    public enum EffectSoundType
    {
        None,
        Door,
        ChestOpen,
        ChestClose,
        FootstepGrass,
        FootstepRoad,
        FootstepHouseFloor,
        FootstepFarmland,
        RockBreak,
        FurnaceOn,
        DeliveryBoxInput,
    }
    #endregion

    #region Player    
    public enum PlayerState
    {
        Idle,
        Walk,
        Tool,
    }

    public enum PlayerDirection
    {
        Up,
        Left,
        Down,
        Right
    }

    public enum PlayerGender
    {
        None,
        Man = 1,
        Woman = 2
    }
    #endregion

    #region Item
    public enum ItemGrade
    {
        None,
        Bronze,
        Silver,
        Gold,
        Amethyst
    }

    public enum ItemCategory
    {
        None = 0,
        Material,
        Tool,
        Crop,
        Furniture,
    }

    public enum ItemType
    {
        None = 0,
        Raw,
        Ingot,
        Hoe,
        WateringCan,
        Shovel,
        Pickaxe,
        Seed,
        Axe,
        Furniture
    }

    public enum FurnitureType
    {
        None = 0,
        Chest,
        CraftingTable,
        Furnace,
        DeliveryBox,
    }
    #endregion

    #region Time
    public enum TimeSection
    {
        Morning = 1,
        Afternoon = 2,
        Evening = 3,
        Night = 4
    }

    public enum Season
    {
        Spring,
        Summer,
        Fall,
        Winter
    }
    #endregion

    public enum Area
    {
        None,
        Farm,
        FarmHouse,
        Route,
        Town,
        Shop,
        Forest,
        Cliff,
        CaveEntrance,
        Cave,
    }

    public enum PropType
    {
        None,
        Crop,
        DropItem,
        Furniture,
        Tree,
        Rock,
        Ladder,
        IronOre,
        GoldOre,
        CoalOre,
    }

    public enum CropType
    {
        None,
        Pumpkin,
        Wheat,
        Carrot,
        Turnip,
        Pepper,
        Tomato,
        Corn,
    }

    public enum ShopType
    {
        None,
        Town,
    }

    public enum SurfaceType
    {
        None,
        Grass,
        Farmland,
        Road,
        HouseFloor
    }
}
