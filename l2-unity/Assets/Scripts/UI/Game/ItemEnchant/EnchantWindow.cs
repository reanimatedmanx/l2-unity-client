using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;
using static L2Slot;
using static UnityEngine.EventSystems.EventTrigger;

public class EnchantWindow : L2PopupWindow
{
    private static EnchantWindow _instance;
    private VisualElement _content;
    private VisualTreeAsset _inventorySlotTemplate;
    private VisualTreeAsset _inventorySlotChoiceTemplate;
    private VisualElement _slot1;
    private VisualElement _slot2;
    private VisualElement _itemBox;
    private VisualElement _progressBar;
    private VisualElement _progressBarBg;
    private VisualElement _animElement;
    private List<Texture2D> _loadingAnimPng;
    private bool _isStartAnim = false;
    private bool _isRun = false;
    public float _moveDuration = 1.5f;

    public static EnchantWindow Instance
    {
        get { return _instance; }
    }
    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            _loadingAnimPng = new List<Texture2D>();
            CachTexture();
        }
        else
        {
            Destroy(this);
        }
    }


    protected override void LoadAssets()
    {
        _windowTemplate = LoadAsset("Data/UI/_Elements/Game/ItemEnchant/EnchantWindow");
        _inventorySlotTemplate = LoadAsset("Data/UI/_Elements/Template/SlotEnchant");
        _inventorySlotChoiceTemplate = LoadAsset("Data/UI/_Elements/Template/SlotEnchantChoice");
    }

    private void CachTexture()
    {
        List<string> list = EnchantListTextures.GetNameTexturesLoading();
        
        foreach (string href in list)
        {
            _loadingAnimPng.Add(IconManager.Instance.LoadTextureOtherSources(href));
        }
    }
    protected override IEnumerator BuildWindow(VisualElement root)
    {
        InitWindow(root);

        yield return new WaitForEndOfFrame();
    }

    protected override void InitWindow(VisualElement root)
    {
        base.InitWindow(root);

        var dragArea = GetElementByClass("drag-area");

        DragManipulator drag = new DragManipulator(dragArea, _windowEle);
        dragArea.AddManipulator(drag);
        var buttonStart = GetElementById("StartButton");
        _progressBar = GetElementById("ProgressGauge");
        _progressBarBg = GetElementById("ProgressBg");
        _animElement = GetElementById("animelement");
        var footer = GetElementById("footer");
        RegisterCloseWindowEventByName("CancelButton");
        RegisterClickWindowEvent(_windowEle, dragArea);
        buttonStart.RegisterCallback<ClickEvent>((evt) => OnClick(evt));

        _slot1 = GetElementById("slot1");
        _slot2 = GetElementById("slot2");
        _itemBox = GetElementById("itemBox");
        InitSlotElement1(_slot1);
        InitSlotElement2(_slot2);
        OnCenterScreen(root);

    }


    private void OnDestroy()
    {
        _instance = null;
    }

    private void InitSlotElement1(VisualElement slotBox)
    {
        VisualElement slotElement = CretaVisualElement();
        EnchantSlot slot = CreateEnchantSlot(0, slotElement, SlotType.Enchant);
        slotBox.Add(slotElement);
    }

    private void InitSlotElement2(VisualElement slotBox)
    {
        VisualElement slotElement = CretaVisualElementChoice();
        EnchantSlot slot = CreateEnchantSlot(0, slotElement, SlotType.Enchant);
        slotBox.Add(slotElement);
    }


    private VisualElement CretaVisualElement()
    {
        return _inventorySlotTemplate.Instantiate()[0];
    }

    private VisualElement CretaVisualElementChoice()
    {
        return _inventorySlotChoiceTemplate.Instantiate()[0];
    }

    private EnchantSlot CreateEnchantSlot(int i, VisualElement slotElement, SlotType slotType)
    {
        return new EnchantSlot(i, slotElement, null, slotType, false);
    }
   
    private async void OnClick(ClickEvent evt)
    {
        if (!_isRun)
        {
            _isRun = true;
            DecreaseProgressBar();
            StartCoroutine(MoveElementsToCenter(_slot1, _slot2));
        }
    }

   
    private async void  DecreaseProgressBar()
    {
        float endValue = 245;
        int index = 0;

        while (index <= endValue)
        {
            index = index + 7;
            UpdatePb(_progressBarBg, _progressBar, endValue, index, endValue);
            //index++;
            //await Task.Delay(100);
            float delayPerIteration = _moveDuration / endValue * 1000;
            await Task.Delay((int)delayPerIteration);
            Debug.Log("Iteration number index " + index);
            Debug.Log("Iteration number delayPerIteration " + delayPerIteration);
        }
        SetPb0(_progressBar);
        _isRun = false ;
    }

    private void UpdatePb(VisualElement pbBg, VisualElement pbGauge, float bgWidth, float currentData, float maxData)
    {
        if (pbBg != null && pbGauge != null)
        {

            // Вычисляем процент заполненного значения
            double hpRatio = (currentData / maxData); // Доля заполненного значения (от 0 до 1)

            // Вычисляем ширину оставшейся части прогресс-бара
            double barWidth = bgWidth * (1 - hpRatio); // Уменьшаем ширину

            pbGauge.style.width = Convert.ToSingle(barWidth);
        }
    }



    private void SetPb0(VisualElement pbGauge)
    {
        pbGauge.style.width = Convert.ToSingle(0);
    }
    


    private IEnumerator MoveElementsToCenter(VisualElement element1, VisualElement element2)
    {



        float offsetLeft2 = 76;//default offset element2
        float offsetLeft1 = 0;//default offset element2
        float fadeStartTime = _moveDuration / 2;

        element2.style.opacity = 1;
        element1.style.left = offsetLeft1;
        element2.style.left = offsetLeft2;

        Vector2 startPos1 = new Vector2(offsetLeft1, element1.resolvedStyle.top);
        Vector2 startPos2 = new Vector2(offsetLeft2, element2.resolvedStyle.top);

        Vector2 intersectionPoint = (startPos1 + startPos2) / 2;

        float elapsedTime = 0f;

        while (elapsedTime < _moveDuration)
        {
            float t = elapsedTime / _moveDuration;

            float left1 = Mathf.Lerp(startPos1.x, intersectionPoint.x, t);
            element1.style.left = left1;

            float left2 = Mathf.Lerp(startPos2.x, intersectionPoint.x, t);
            float newLeft = left2 - offsetLeft2;
            element2.style.left = newLeft;

            Debug.Log("left2 calc " + left2);
            Debug.Log("left2 calc x " + startPos2.x);
            Debug.Log("left2 calc intersectionPoint " + intersectionPoint.x);
            float animTime = _moveDuration - 0.4f;
            PlayAnim(_loadingAnimPng, _animElement, animTime);
            StartFinalEvent(fadeStartTime, elapsedTime, element2);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        SetOpacity0(element2);
        _isRun = false;
        _isStartAnim = false;
    }

    
    private void StartFinalEvent(float fadeStartTime, float elapsedTime, VisualElement element2)
    {
        if (elapsedTime >= fadeStartTime)
        {
            AlphaChanged(fadeStartTime, elapsedTime, element2);
            //PlayAnim(_loadingAnimPng, _animElement , fadeStartTime);
        }
            
    }

    private async void PlayAnim(List<Texture2D> listTextures, VisualElement animElement , float fadeStartTime)
    {
        if (!_isStartAnim)
        {
            animElement.style.display = DisplayStyle.Flex;
            //offset 32x32 icon up 10px
            animElement.style.marginTop = -14;
            _isStartAnim = true;
            foreach (Texture2D texture in listTextures)
            {
                animElement.style.backgroundImage = new StyleBackground(texture);
                float delayPerIteration = fadeStartTime / listTextures.Count * 1000;
                await Task.Delay((int)delayPerIteration);
            }
            animElement.style.display = DisplayStyle.None;
            animElement.style.marginTop = 0;
        }

    }


    private void AlphaChanged(float fadeStartTime , float elapsedTime , VisualElement element1)
    {
        if (elapsedTime >= fadeStartTime)
        {
            float fadeT = (elapsedTime - fadeStartTime) / (_moveDuration - fadeStartTime);
            float alpha = Mathf.Lerp(1f, 0f, fadeT);
            element1.style.opacity = alpha; 
        }
    }

    private void SetOpacity0(VisualElement element1)
    {
        element1.style.opacity = 0;
    }
}

