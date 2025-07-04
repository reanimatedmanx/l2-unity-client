using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class L2SlotManager : L2PopupWindow
{
    [SerializeField] private L2Slot _draggedSlot;
    [SerializeField] private L2Slot _hoverSlot;
    private L2Slot _dragSlotData;

    private static L2SlotManager _instance;
    public static L2SlotManager Instance { get { return _instance; } }

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

    private void OnDestroy()
    {
        _instance = null;
    }

    protected override void LoadAssets()
    {
        _windowTemplate = LoadAsset("Data/UI/_Elements/Template/DraggedSlot");
    }

    protected override IEnumerator BuildWindow(VisualElement root)
    {
        InitWindow(root);

        yield return new WaitForEndOfFrame();
        _dragSlotData = new L2Slot(_windowEle);
        _windowEle.usageHints = UsageHints.None;
        _windowEle.pickingMode = PickingMode.Ignore;
        _windowEle.style.position = Position.Absolute;
    }

    public void SetHoverSlot(L2Slot slot)
    {
        _hoverSlot = slot;
    }

    public void SetDraggedSlot(L2Slot slot)
    {
        _draggedSlot = slot;

        _dragSlotData.Icon = slot.Icon;
        _dragSlotData.Id = slot.Id;

        RefreshImage(slot);
    }

    private void RefreshImage(L2Slot slot)
    {
        if (slot.GetType() == typeof(GearSlot))
        {
            StyleBackground background = slot.SlotElement.style.backgroundImage;
            _dragSlotData.SlotBg.style.backgroundImage = background;
        }
        else
        {
            if (slot.SlotBg != null)
            {
                StyleBackground background = slot.SlotBg.style.backgroundImage;
                _dragSlotData.SlotBg.style.backgroundImage = background;
            }
        }
    }

    internal void DragSlot(float right, float bottom)
    {
        ShowWindow();
        _windowEle.style.right = right;
        _windowEle.style.bottom = bottom;
    }

    public void ReleaseDrag()
    {

        HideWindow();

        if (!IsValidDrag() || IsSameSlot())
        {
            ResetSlots();
            return;
        }

        HandleDragRelease();
        ResetSlots();
    }

    private void HandleDragRelease()
    {

        switch (_draggedSlot.Type)
        {
            case L2Slot.SlotType.Gear:
                HandleGearDrag();
                break;
            case L2Slot.SlotType.Inventory:
            case L2Slot.SlotType.InventoryBis:
                HandleInventoryDrag();
                break;
            case L2Slot.SlotType.SkillBar:
                HandleSkillbarDrag();
                break;
            case L2Slot.SlotType.Action:
                HandleActionDrag();
                break;
            default:
                break;
        }
    }

    #region DragReleaseHandlers
    private void HandleActionDrag()
    {
        if (_hoverSlot == null)
        {
            return;
        }

        if (_hoverSlot.Type == L2Slot.SlotType.SkillBar)
        {
            AddActionToSkillbar();
        }
    }

    private void HandleGearDrag()
    {
        if (_hoverSlot.Type == L2Slot.SlotType.Inventory || _hoverSlot.Type == L2Slot.SlotType.InventoryBis)
        {
            Unequip();
        }
        else if (_hoverSlot.Type == L2Slot.SlotType.Trash)
        {
            //QuantityInput.Instance.ShowWindow(this, source, type, listServer, position);
            DestroyItem();
        }
    }

    private void HandleInventoryDrag()
    {
        var inventorySlot = (InventorySlot)_draggedSlot;

        if (_hoverSlot == null)
        {
            if (!L2GameUI.Instance.MouseOverUI)
            {
                DropItem();
            }

            return;
        }

        switch (_hoverSlot.Type)
        {
            case L2Slot.SlotType.Gear when IsItemGear(inventorySlot.ItemCategory):
                Equip();
                break;
            case L2Slot.SlotType.Inventory:
                ChangeItemOrder();
                break;
            case L2Slot.SlotType.Trash:
                DestroyItem();
                break;
            case L2Slot.SlotType.SkillBar:
                AddItemToSkillbar();
                break;
            case L2Slot.SlotType.Enchant:
                AddItemToEnchant();
                break;
            default:
                if (!L2GameUI.Instance.MouseOverUI)
                {
                    DropItem();
                }
                break;
        }
    }

    private void HandleSkillbarDrag()
    {
        if (SkillbarWindow.Instance.Locked)
        {
            return;
        }

        if (_hoverSlot == null)
        {
            if (!L2GameUI.Instance.MouseOverUI)
            {
                RemoveSkillbarSlot();
            }

            return;
        }

        switch (_hoverSlot.Type)
        {
            case L2Slot.SlotType.SkillBar:
                MoveSkillbarSlot();
                break;
            case L2Slot.SlotType.Enchant:
                int oldSlot = _draggedSlot.Id;
                int newSlot = _hoverSlot.Id;
                Debug.LogWarning($"Moving skillbar enchant from slot {oldSlot} to slot {newSlot}.");
                break;
            default:
                if (!L2GameUI.Instance.MouseOverUI)
                {
                    RemoveSkillbarSlot();
                }
                break;
        }
    }

    #endregion

    #region SlotVerification
    private bool IsValidDrag() => _draggedSlot != null && (_hoverSlot != null || !L2GameUI.Instance.MouseOverUI);

    private bool IsSameSlot() => _draggedSlot != null && _hoverSlot != null && _draggedSlot.Position == _hoverSlot.Position && _draggedSlot.Type == _hoverSlot.Type;

    private bool IsItemGear(ItemCategory category)
    {
        return category == ItemCategory.Weapon
            || category == ItemCategory.ShieldArmor
            || category == ItemCategory.Jewel;
    }

    #endregion

    #region DragActions

    private void Unequip()
    {
        // Unequip logic
        GearSlot slot = (GearSlot)_draggedSlot;
        Debug.Log($"Unequip {slot.Id}.");
        slot.UseItem();
    }

    private void Equip()
    {
        // Equip logic
        InventorySlot slot = (InventorySlot)_draggedSlot;
        Debug.Log($"Equip {slot.Id}.");
        slot.UseItem();
    }

    private void DropItem()
    {
        // Drop item logic
        InventorySlot slot = (InventorySlot)_draggedSlot;
        Debug.Log($"Drop {slot.Id}.");
    }

    private InventorySlot _slotTemp = null;
    private void DestroyItem()
    {
        // Destroy item logic
        InventorySlot slot = (InventorySlot)_draggedSlot;
        _slotTemp = slot;
        if (slot.Count > 1)
        {
            PauseAndShowQuantity(slot.Count);
        }
        else
        {
            SystemMessageWindow.Instance.OnButtonOk += OkDestroy;
            SystemMessageWindow.Instance.OnButtonClosed += OnCancel;
            SystemMessageWindow.Instance.ShowWindowDialogYesOrNot("Do you want to destroy your " + slot.Name);
            
        }
    }

    private void OkDestroy()
    {
        PlayerInventory.Instance.DestroyItem(_slotTemp.ObjectId, 1);
        //SystemMessageWindow.Instance.OnButtonOk -= OkDestroy;
        CancelEvent();
    }

    public void OnCancel()
    {
        CancelEvent();
    }

    private void CancelEvent()
    {
        SystemMessageWindow.Instance.OnButtonOk -= OkDestroy;
        SystemMessageWindow.Instance.OnButtonClosed -= OnCancel;
    }


    private void PauseAndShowQuantity(int count)
    {
        QuantityInput.Instance.ShowWindow(count);
        QuantityInput.Instance.OnButtonOk += OnSelectedQuantity;
    }

    private void OnSelectedQuantity(string value)
    {
        PlayerInventory.Instance.DestroyItem(_slotTemp.ObjectId, int.Parse(value));
        QuantityInput.Instance.OnButtonOk -= OnSelectedQuantity;
    }

    private void AddActionToSkillbar()
    {
        int actionId = ((ActionSlot)_draggedSlot).ActionId;
        int slot = _hoverSlot.Position;
        Debug.LogWarning($"Add action ID {actionId} into skillbar slot {slot}.");

        PlayerShortcuts.Instance.AddShortcut(slot, actionId, Shortcut.TYPE_ACTION , 0);
    }

    private void ChangeItemOrder()
    {
        int fromSlot = _draggedSlot.Position;
        int toSlot = _hoverSlot.Position;

        Debug.Log($"Moving item from slot {fromSlot} to slot {toSlot}.");

        PlayerInventory.Instance.ChangeItemOrder(fromSlot, toSlot);
    }

    private void AddItemToSkillbar()
    {
        int itemId = ((InventorySlot)_draggedSlot).ObjectId;
        int slot = _hoverSlot.Position;
        Debug.LogWarning($"Add item {itemId} to skillbar slot {slot}.");

        PlayerShortcuts.Instance.AddShortcut(slot, itemId, Shortcut.TYPE_ITEM, 0);
    }


    private void AddItemToEnchant()
    {
        int _objectId = ((InventorySlot)_draggedSlot).ObjectId;
        int slot = _hoverSlot.Position;
        Debug.LogWarning($"Add _objectId {_objectId} to EnchantWindow slot {slot}.");
        EnchantWindow.Instance.AddEnchantItem(_objectId);
    }

    private void MoveSkillbarSlot()
    {
        int oldSlot = _draggedSlot.Position;
        int newSlot = _hoverSlot.Position;
        Debug.LogWarning($"Moving skillbar shortcut from slot {oldSlot} to slot {newSlot}.");
        PlayerShortcuts.Instance.MoveShortcut(oldSlot, newSlot);
    }

    private void RemoveSkillbarSlot()
    {
        int oldSlot = _draggedSlot.Position;
        Debug.LogWarning($"Renoving skillbar shortcut from slot {oldSlot}.");
        PlayerShortcuts.Instance.DeleteShortcut(oldSlot , true);
    }

    #endregion

    private void ResetSlots()
    {
        _draggedSlot = null;
        _hoverSlot = null;
    }

    public override void ShowWindow()
    {
        _windowEle.style.opacity = 1;
    }

    public override void HideWindow()
    {
        _windowEle.style.opacity = 0;
    }
}
