using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[CreateAssetMenu(fileName = "NewPurchaseable", menuName = "Inventory/Purchaseable")]
public class Purchaseable : ScriptableObject
{
    public ItemDetails item;
    public int amt=1;
    public int price;
    public string Description;
}