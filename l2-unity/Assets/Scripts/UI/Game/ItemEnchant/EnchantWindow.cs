using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;
using static L2Slot;

public class EnchantWindow : L2PopupWindow
{
    private static EnchantWindow _instance;
    private VisualElement _content;
    private VisualTreeAsset _inventorySlotTemplate;
    private VisualTreeAsset _inventorySlotChoiceTemplate;

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
        //_content = GetElementByClass("content");
        //DragManipulator drag = new DragManipulator(dragArea, _windowEle);
        // dragArea.AddManipulator(drag);

        // RegisterCloseWindowEvent("btn-close-frame");
        //RegisterClickWindowEvent(_windowEle, dragArea);
        VisualElement slot1 = GetElementById("slot1");
        VisualElement slot2 = GetElementById("slot2");
        InitSlotElement1(slot1);
        InitSlotElement2(slot2);
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


}
