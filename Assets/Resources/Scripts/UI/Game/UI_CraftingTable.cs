using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RecipeSlot
{
    private const string SLOT_IMAGE = "RecipeImage";
    public int OutputItemCode { get; private set; }
    public List<Data.Game.CraftingMaterial> Materials { get; private set; }
    private Image _slotImage;

    public RecipeSlot(Image slotImage)
    {
        _slotImage = slotImage;
    }

    public static RecipeSlot CreateRecipeSlot(GameObject slot)
    {
        Image slotImage = Util.FindChild<Image>(slot, SLOT_IMAGE, true);

        return new RecipeSlot(slotImage);
    }

    public void SetSlotItem(int itemCode, List<Data.Game.CraftingMaterial> craftingMaterials)
    {
        if (_slotImage == null)
        {
            return;
        }

        OutputItemCode = itemCode;
        Materials = craftingMaterials;
        _slotImage.sprite = Managers.Resource.LoadItemSprite(itemCode);
        _slotImage.color = Color.white;
    }
}

public class MaterialSlot
{
    private const string SLOT_IMAGE = "MaterialImage";
    private const string REQUIRE_QUANTITY = "RequireQuantity";
    private const string CHECK_MARK = "CheckMark";
    private const string UNCHECK_MARK = "UncheckMark";

    private Image _slotImage;
    private TextMeshProUGUI _requireQuantityText;
    private Image _checkImage;
    private Image _uncheckImage;

    public MaterialSlot(Image slotImage, TextMeshProUGUI quantity, Image check, Image uncheck)
    {
        _slotImage = slotImage;
        _requireQuantityText = quantity;
        _checkImage = check;
        _uncheckImage = uncheck;
    }

    public static MaterialSlot CreateMaterialSlot(GameObject slot)
    {
        Image slotImage = Util.FindChild<Image>(slot, SLOT_IMAGE, true);
        TextMeshProUGUI quantity = Util.FindChild<TextMeshProUGUI>(slot, REQUIRE_QUANTITY, true);
        Image check = Util.FindChild<Image>(slot, CHECK_MARK, true);
        Image uncheck = Util.FindChild<Image>(slot, UNCHECK_MARK, true);

        return new MaterialSlot(slotImage, quantity, check, uncheck);
    }

    public void SetSlot(int itemCode, int quantity)
    {
        if (_slotImage == null || _requireQuantityText == null || _checkImage == null || _uncheckImage == null) 
        {
            return;
        }

        _slotImage.sprite = Managers.Resource.LoadItemSprite(itemCode);
        _slotImage.color = Color.white;
        _requireQuantityText.text = $"X {quantity}";

        bool hasItem = Managers.Data.InventoryDataManager.HasAtLeastItemInInventory(itemCode, quantity);
        if (hasItem)
        {
            _checkImage.color = Color.white;
            _uncheckImage.color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
        }
        else
        {
            _checkImage.color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
            _uncheckImage.color = Color.white;
        }
    }
}

public class UI_CraftingTable : UI_Base
{
    public enum Buttons
    {
        CraftButton,
        ExitButton
    }

    public enum GameObjects
    {
        RecipeContents,
        MaterialContents
    }

    public enum Images
    {
        OutputItemImage
    }

    private const string RECIPE_SLOT_PATH = "UI/Etc/RecipeSlot";
    private const string RECIPE_SLOT_PREFIX = "RecipeSlot";
    private const string MATERIAL_SLOT_PATH = "UI/Etc/CraftingMaterialSlot";
    private const string MATERIAL_SLOT_PREFIX = "MaterialSlot";
    private Dictionary<GameObject, RecipeSlot> _recipeSlots = new Dictionary<GameObject, RecipeSlot>();
    private Dictionary<GameObject, MaterialSlot> _materialSlots = new Dictionary<GameObject, MaterialSlot>();
    private GameObject _recipeContents;
    private GameObject _materialContents;
    private RecipeSlot _currentRecipeSlot = null;
    private Image _outputImage;
    private Vector3Int _craftingTablePosition;

    public void SetTablePosition(Vector3Int position)
    {
        _craftingTablePosition = position;
    }

    private void OnRecipeSlotClicked(PointerEventData data)
    {
        if (!_recipeSlots.TryGetValue(data.pointerClick.gameObject, out RecipeSlot recipeSlot))
        {
            return;
        }

        if (_outputImage == null)
        {
            return;
        }

        _outputImage.sprite = Managers.Resource.LoadItemSprite(recipeSlot.OutputItemCode);
        _outputImage.color = Color.white;

        InitMaterialSlots(recipeSlot);

        _currentRecipeSlot = recipeSlot;
    }

    private void InitMaterialSlots(RecipeSlot recipeSlot)
    {
        if (_materialContents == null)
        {
            return;
        }

        _materialSlots.Clear();
        for (int i = _materialContents.transform.childCount - 1; i >= 0; i--)
        {
            Transform child = _materialContents.transform.GetChild(i);
            Managers.Resource.Destroy(child.gameObject);
        }

        List<Data.Game.CraftingMaterial> craftingMaterials = recipeSlot.Materials;
        
        for (int i = 0; i < craftingMaterials.Count; i++)
        {
            GameObject slotObject = Managers.Resource.Instantiate(MATERIAL_SLOT_PATH, _materialContents.transform);
            MaterialSlot materialSlot = MaterialSlot.CreateMaterialSlot(slotObject);

            slotObject.name = $"{MATERIAL_SLOT_PREFIX}{i}";
            materialSlot.SetSlot(craftingMaterials[i].materialItemCode, craftingMaterials[i].materialQuantity);
            _materialSlots[slotObject] = materialSlot;
        }
    }

    private void OnCraftingButtonClicked(PointerEventData data)
    {
        if (_currentRecipeSlot == null)
        {
            return;
        }

        Managers.Game.CraftItem(_craftingTablePosition, _currentRecipeSlot.OutputItemCode, _currentRecipeSlot.Materials);
    }

    private void OnExitButtonClicked(PointerEventData data)
    {
        Managers.UI.CloseUI(this.gameObject);
    }

    private void BindUIElements()
    {
        Bind<Button>(typeof(Buttons));
        Bind<GameObject>(typeof(GameObjects));
        Bind<Image>(typeof(Images));
    }

    private void GetUIElements()
    {
        _recipeContents = GetObject((int)GameObjects.RecipeContents);
        _materialContents = GetObject((int)GameObjects.MaterialContents);
        _outputImage = GetImage((int)Images.OutputItemImage);
    }

    private void BindButtonEvent()
    {
        GetButton((int)Buttons.ExitButton).gameObject.BindEvent(OnExitButtonClicked);
        GetButton((int)Buttons.CraftButton).gameObject.BindEvent(OnCraftingButtonClicked);
    }

    private void BuildRecipeSlots()
    {
        if (_recipeContents == null)
        {
            return;
        }

        List<Data.Game.CraftingRecipe> recipes = Managers.Data.GameDataManager.GetAllRecipes();

        for (int i = 0; i < recipes.Count; i++)
        {
            GameObject slotObject = Managers.Resource.Instantiate(RECIPE_SLOT_PATH, _recipeContents.transform);
            RecipeSlot recipeSlot = RecipeSlot.CreateRecipeSlot(slotObject);

            slotObject.BindEvent(OnRecipeSlotClicked);
            slotObject.name = $"{RECIPE_SLOT_PREFIX}{i}";
            recipeSlot.SetSlotItem(recipes[i].outputItemCode, recipes[i].requireMaterials);
            _recipeSlots[slotObject] = recipeSlot;
        }
    }

    public override void Init()
    {
        BindUIElements();
        GetUIElements();
        BindButtonEvent();
        BuildRecipeSlots();
    }
}
