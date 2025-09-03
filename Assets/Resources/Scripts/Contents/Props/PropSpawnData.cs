using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#nullable enable
public class PropSpawnData
{
    public Vector3Int tilePosition;
    public Define.PropType propType = Define.PropType.None;

    #region Crop
    public Define.CropType? cropType;
    public int? growLevel;
    public bool? isDead;
    #endregion

    #region DropItem
    public int? itemCode;
    public int? itemQuantity;
    #endregion

    #region  Furniture
    public Define.FurnitureType? furnitureType;
    public string? chestId;
    public bool? isMelting;
    #endregion
}
