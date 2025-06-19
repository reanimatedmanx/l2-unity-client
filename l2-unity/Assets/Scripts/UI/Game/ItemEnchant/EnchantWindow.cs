using System;
using System.Collections;
using System.Collections.Generic;
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
    private int _startPosition1 = 0;
    private int _startPosition2 = 66;

    public static EnchantWindow Instance
    {
        get { return _instance; }
    }
    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;

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

    private VisualElement CreateVisualElementDisabled()
    {
        VisualElement slotElement = CretaVisualElement();
        slotElement.AddToClassList("inventory-slot");
        slotElement.AddToClassList("disabled");
        return slotElement;
    }
    private EnchantSlot CreateEnchantSlot(int i, VisualElement slotElement, SlotType slotType)
    {
        return new EnchantSlot(i, slotElement, null, slotType, false);
    }
    private bool isRun = false;
    private void OnClick(ClickEvent evt)
    {
        StartCoroutine(DecreaseProgressBar());
        float centerX = _itemBox.resolvedStyle.width / 2;
        float centerY = _itemBox.resolvedStyle.height / 2;
        if (!isRun)
        {
            isRun = true;
            StartCoroutine(MoveElementsToCenter(_slot1, _slot2, centerX, centerY));
        }
    }

   
    private IEnumerator DecreaseProgressBar()
    {
        float endValue = 245;
        int index = 0;

        while (index <= endValue)
        {

            UpdatePb(_progressBarBg, _progressBar, endValue, index, endValue);
            index++;
            yield return new WaitForSeconds(0.1f);
        }

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
    public float moveDuration = 1.0f; // Длительность 

    
    private IEnumerator MoveElementsToCenter(VisualElement element1, VisualElement element2, float centerX, float centerY)
    {
        float st = _startPosition1 - element1.resolvedStyle.left;
       
        //element2.style.left = _startPosition2 - element1.resolvedStyle.left;
        // Получаем начальные позиции

        Debug.Log("Start posirion 1 left " + element1.resolvedStyle.left);
        element1.style.left = 0;
        Debug.Log("Start posirion 1 _startPosition1 " + _startPosition1);
        Debug.Log("Start posirion 1 left " + element1.style.left);
        Debug.Log("Start posirion 1 st " + st);

        Vector2 startPos1 = new Vector2(0, element1.resolvedStyle.top);
        Vector2 startPos2 = new Vector2(element2.resolvedStyle.left, element2.resolvedStyle.top);
        // Вычисляем точку пересечения (среднюю точку между элементами)
        Vector2 intersectionPoint = (startPos1 + startPos2) / 2;

        float elapsedTime = 0f;

            while (elapsedTime < moveDuration)
            {

                // Пропорция времени
                float t = elapsedTime / moveDuration;

                float left1 = Mathf.Lerp(startPos1.x, intersectionPoint.x, t);
                // Интерполяция позиций
                element1.style.left = left1;
                //element1.style.top = Mathf.Lerp(startPos1.y, intersectionPoint.y, t);
                float left2 = Mathf.Lerp(startPos2.x, intersectionPoint.x, t);

                // float top = Mathf.Lerp(startPos2.y, intersectionPoint.y, t);
                //Debug.Log("Start posirion left1 " + left1);
                Debug.Log("Start posirion left2 " + left2);
                Debug.Log("Start posirion left2 resolved " + element2.resolvedStyle.left);
                var cur_left2 = left2 - element2.resolvedStyle.left;
                Debug.Log("Start posirion left2 current " + element2.style.left);
                Debug.Log("Start posirion left2 left " + cur_left2);
                //Debug.Log("Start posirion 2_3 " + top);
                //element2.style.left = cur_left2;
                // element2.style.top = top

                elapsedTime += Time.deltaTime;
                yield return null; // Ждем следующего кадра
            }

        isRun = false;
    }
}
