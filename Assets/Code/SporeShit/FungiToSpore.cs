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

    private void Awake()
    {
        TestUnlock();



        document = GetComponent<UIDocument>();

        ShroomButtons = new Button[shroomInfo.Length];
        ShroomImages = new Image[shroomInfo.Length];



        ShroomButtons[0] = document.rootVisualElement.Q("Random") as Button;
        ShroomButtons[0].RegisterCallback<ClickEvent>(OnRandomClick);

        if (ShroomButtons[0] != null)
        {
            ShroomImages[0] = ShroomButtons[0].Q<Image>();
        }

        ShroomButtons[1] =  document.rootVisualElement.Q("JackOLantern") as Button;
        ShroomButtons[1].RegisterCallback<ClickEvent>(OnJackClick);

        for(int i = 0; i<ShroomImages.Length;i++){
            if (ShroomButtons[i] != null)
            {
                ShroomImages[i] = ShroomButtons[i].Q<Image>();
            }
        }

        ShroomImages[0].sprite = shroomInfo[0].icon;
        RefreshImages();

        CloseWindow();
    }

    void OnDisable()
    {
        ShroomButtons[1].UnregisterCallback<ClickEvent>(OnJackClick);
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
        if(unlocks == null) return false;

        return unlocks.CheckIfUnlocked(shroomInfo[i].shroom);
    }

    private void OnRandomClick(ClickEvent evt)
    {
        ctx.ctx.intendedSpore = null;
    }

    private void OnJackClick(ClickEvent evt)
    {
        ctx.ctx.intendedSpore = shroomInfo[1].shroom;
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
    
}
