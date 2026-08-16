using System;
using HSM;
using UnityEngine;
using UnityEngine.UIElements;

public class FungiToSpore : MonoBehaviour
{
    [SerializeField] private PlayerStateDriver ctx;

    [SerializeField] private UnlockedFungi unlocks;


    private UIDocument document;

    private Button[] ShroomButtons;

    private Image[] ShroomImages;

    [SerializeField] private FungiButtonInfo[] shroomInfo;
    [SerializeField] private Sprite LockImage;

    [SerializeField] private MushroomDetails[] testUnlocks;

    private VisualElement root;

    private void Awake()
    {
        TestUnlock();



        document = GetComponent<UIDocument>();

        root = document.rootVisualElement;

        ShroomButtons = new Button[shroomInfo.Length];
        ShroomImages = new Image[shroomInfo.Length];



        ShroomButtons[0] = document.rootVisualElement.Q("Random") as Button;
        ShroomButtons[0].RegisterCallback<ClickEvent>(OnRandomClick);

        if (ShroomButtons[0] != null)
        {
            ShroomImages[0] = ShroomButtons[0].Q<Image>();
        }
        for(int i = 1;i<ShroomButtons.Length;i++){
            ShroomButtons[i] =  document.rootVisualElement.Q(i.ToString()) as Button;
            ShroomButtons[i].RegisterCallback<ClickEvent,int>(OnShroomClick,i);
        }


        for(int i = 0; i<ShroomImages.Length;i++){
            if (ShroomButtons[i] != null)
            {
                ShroomImages[i] = ShroomButtons[i].Q<Image>();
            }
        }

        ShroomImages[0].sprite = shroomInfo[0].icon;
        RefreshImages();
        SetData(0);

        CloseWindow();
    }

    void OnDisable()
    {
        ShroomButtons[0].UnregisterCallback<ClickEvent>(OnRandomClick);
        for(int i = 1; i<ShroomButtons.Length;i++){
            ShroomButtons[i].UnregisterCallback<ClickEvent,int>(OnShroomClick);
        }
    }

    private void RefreshImages()
    {
        for(int i = 1; i<ShroomImages.Length;i++){
            if (ShroomButtons[i] != null)
            {
                if(CheckIfUnlocked(i)) ShroomImages[i].sprite = shroomInfo[i].icon;
                else ShroomImages[i].sprite = LockImage;
            }
        }
    }

    private bool CheckIfUnlocked(int i)
    {
        return true;/*
        if(unlocks == null) return false;

        return unlocks.CheckIfUnlocked(shroomInfo[i].shroom);
        */
    }

    private void OnRandomClick(ClickEvent evt)
    {
        ctx.ctx.intendedSpore = null;
        SetData(0);
    }

    private void OnShroomClick(ClickEvent evt,int i)
    {
        ctx.ctx.intendedSpore = shroomInfo[i].shroom;
        SetData(i);
    }


    public void TestUnlock()
    {
        foreach(MushroomDetails shroom in testUnlocks)
        {
            unlocks.UnlockFungi(shroom);
        }
    }

    public void CloseWindow()
    {
        document.rootVisualElement.style.display = DisplayStyle.None;
    }
    public void OpenWindow()
    {
        document.rootVisualElement.style.display = DisplayStyle.Flex;
        RefreshImages();
    }
    
    private void SetData(int i)
    {
        root.dataSource = shroomInfo[i];
    }
}
