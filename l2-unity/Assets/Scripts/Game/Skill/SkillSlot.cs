using UnityEngine;
using UnityEngine.UIElements;

public class SkillSlot : L2DraggableSlot
{
    public int SkillId { get; private set; }

    public SkillSlot(VisualElement slotElement, int position, SlotType slotType) : base(position, slotElement, slotType, true, false)
    {
        _slotElement = slotElement;
        _position = position;
    }

    protected override void HandleLeftClick()
    {
        Debug.Log("CLICK EVENT SKILL SLOT");
        //PlayerActions.Instance.UseAction((ActionType)ActionId);
    }

    protected override void HandleRightClick()
    {
    }

    protected override void HandleMiddleClick()
    {
    }

    public void AssignSkill(int skillId , int level)
    {
        //ButtonClickSoundManipulator _buttonClickSoundManipulator = new ButtonClickSoundManipulator(_slotElement);
        Skillgrp skill = SkillgrpTable.Instance.GetSkill(skillId , level);
        _slotDragManipulator.enabled = true;
        _slotElement.RemoveFromClassList("empty");
        var icon = IconManager.Instance.LoadTextureByName(skill.Icon);
        _slotBg.style.backgroundImage = icon;

        if (_tooltipManipulator != null)
        {
            _tooltipManipulator.SetText("Demo Text ToolTips");
        }
    }

    
}
