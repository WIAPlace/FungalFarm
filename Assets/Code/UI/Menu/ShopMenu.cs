using System;
using System.Collections.Generic;
using HSM;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.XR;

public class ShopMenu : MonoBehaviour
{
    private UIDocument document;

    private Button milkButton;
    private Button brushButton;
    private Button trowelButton;

    [SerializeField] private Money bank;
    [SerializeField] private ItemDetails Milk;
    [SerializeField] private PlayerStateDriver ctx;
    [SerializeField] private BasketMerchant bm;

    public int handUpgradeCost;
    public int milkUpgradeCost;


    

    [field:SerializeField] public Purchaseable[] purchaseables;

    private List<Button> storeSlots = new List<Button>(); // use if we want all of the buttons to do one thing, like make a sound on click

    private void Awake()
    {
        document = GetComponent<UIDocument>();

        milkButton = document.rootVisualElement.Q("MilkButton") as Button;
        milkButton.RegisterCallback<ClickEvent>(OnMilkButtonClick);

        brushButton = document.rootVisualElement.Q("BrushButton") as Button;
        brushButton.RegisterCallback<ClickEvent>(OnBrushButtonClick);

        trowelButton = document.rootVisualElement.Q("TrowelButton") as Button;
        trowelButton.RegisterCallback<ClickEvent>(OnTrowelButtonClick);

        document.rootVisualElement.style.display = DisplayStyle.None;
    }

    private void OnDisable()
    {
        milkButton.UnregisterCallback<ClickEvent>(OnMilkButtonClick);
        brushButton.UnregisterCallback<ClickEvent>(OnBrushButtonClick);
        trowelButton.UnregisterCallback<ClickEvent>(OnTrowelButtonClick);
    }

    private bool TryBuyItem(int cost)
    {
        if( bank == null) return false;
        if (bank.Amt >= cost)
        {
            bank.Amt -= cost;
            return true;
        }
        else return false;
    }

    private void OnMilkButtonClick(ClickEvent evt)
    {
        if (TryBuyItem(milkUpgradeCost))
        {
            Debug.Log("Milk Transacted");
            bm.AddItems(Milk,1);
            //milkButton.focusable = false;
        }
        else Debug.Log("Milk Poor");
    }
    private void OnBrushButtonClick(ClickEvent evt)
    {
        
        if (TryBuyItem(handUpgradeCost)&& !ctx.ctx.brushUpgrade)
        {
            Debug.Log("Brush Transacted");
            ctx.ctx.brushUpgrade = true;
            brushButton.focusable = false;
        }
        else Debug.Log("Brush Poor");
    }
    private void OnTrowelButtonClick(ClickEvent evt)
    {
        if (TryBuyItem(handUpgradeCost)&& !ctx.ctx.trowelUpgrade)
        {
            Debug.Log("Trowel Transacted");
            ctx.ctx.trowelUpgrade = true;
            trowelButton.focusable = false;
        }
        else Debug.Log("Trowel Poor");
    }  

    public void ShowShopUI()
    {
        document.rootVisualElement.style.display = DisplayStyle.Flex;
    }
    public void CloseShopUI()
    {
        document.rootVisualElement.style.display = DisplayStyle.None;
    }



}
