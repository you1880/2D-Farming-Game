using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;

public class DroppedItem : MonoBehaviour
{
    private const float CAN_LOOT_DELAY = 1.0f;
    [SerializeField] private Rigidbody2D _rigidBody;
    [SerializeField] private SpriteRenderer _itemSpriteRenderer;
    [SerializeField] private TextMeshPro _quantityText;
    private bool _canLoot = false;
    private int _lootItemCode = 0;
    private int _lootItemQuantity = 0;
    private int _lootItemGrade = 0;
    public bool CanLoot { get { return _canLoot; } }
    public Define.PropType PropType { get; private set; } = Define.PropType.DropItem;

    public void SetDropItem(int itemCode, int quantity = 0)
    {
        if (itemCode == 0)
        {
            return;
        }

        _lootItemCode = itemCode;
        _lootItemQuantity = quantity;

        SetDropItemSprite();
        SetDropItemQunatity();
    }

    public void LootItem()
    {
        if (Managers.Data.InventoryDataManager.AddItemInventory(_lootItemCode, _lootItemQuantity, _lootItemGrade))
        {
            Managers.Resource.Destroy(this.gameObject);
        }
    }

    private void SetDropItemSprite()
    {
        if (_itemSpriteRenderer == null)
        {
            return;
        }

        _itemSpriteRenderer.sprite = Managers.Resource.LoadItemSprite(_lootItemCode);
    }

    private void SetDropItemQunatity()
    {
        if (_quantityText == null)
        {
            return;
        }

        if (_lootItemQuantity > 1)
        {
            _quantityText.text = $"{_lootItemQuantity}";
        }
        else
        {
            _quantityText.text = "";
        }
    }

    private void SetRandomlyPosition()
    {
        if (_rigidBody == null)
        {
            return;
        }

        transform.position = new Vector3(transform.position.x + 0.5f, transform.position.y + 0.5f, 0.0f);
        float angle = Random.Range(0.0f, 360.0f) * Mathf.Deg2Rad;
        float power = Random.Range(2.0f, 5.0f);

        Vector2 direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
        _rigidBody.AddForce(direction * power, ForceMode2D.Impulse);

        StartCoroutine(WaitForAnimation(0.1f));
    }

    private IEnumerator WaitForAnimation(float delayTime)
    {
        yield return new WaitForSeconds(delayTime);

        if (_rigidBody != null)
        {
            _rigidBody.bodyType = RigidbodyType2D.Kinematic;
            _rigidBody.velocity = Vector2.zero;
            _rigidBody.angularVelocity = 0.0f;
        }
    }

    private IEnumerator WaitForLootDelay()
    {
        yield return new WaitForSeconds(CAN_LOOT_DELAY);

        _canLoot = true;
    }

    private void Start()
    {
        SetRandomlyPosition();
        StartCoroutine(WaitForLootDelay());
    }
}
