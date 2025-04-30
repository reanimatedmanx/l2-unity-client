using UnityEngine;
using UnityEngine.UIElements;


public class DataCell
{
    private bool _isBusy;
    private int _idSkill;
    private int _level;
    private VisualElement _element;
    private int _position = 0;
    private Skillgrp _skill;

    public DataCell(int idSkill , VisualElement element , int position)
    {
        _idSkill = idSkill;
        _element = element;
        _isBusy = false;
        _position = position;
    }

    public void ResetData()
    {
        SetSkillId(-1);
        SetLevel(-1);
        SetBusy(false);
        _skill = null;
    }

    public void RefreshData(int skillId , bool isbusy ,int level)
    {
        _skill = SkillgrpTable.Instance.GetSkill(skillId , level);
        SetSkillId(skillId);
        SetLevel(level);
        SetBusy(isbusy);

        if(isbusy != false) SetIcon(skillId, level);
    }

    public int GetSkillId()
    {
        return _idSkill;
    }

    public void SetSkillId(int skillId)
    {
        _idSkill = skillId;
    }

    public void SetLevel(int level)
    {
        _level = level;
    }

    public int GetLevel()
    {
        return _level;
    }

    public bool IsBusy()
    {
        return _isBusy;
    }

    public void SetBusy(bool isBusy)
    {
        _isBusy = isBusy;
    }

    public bool IsPassiveSkill()
    {
        if (_skill != null)
        {
            if (_skill.OperateType == 2) return true;
        }
        
        return false;
    }

    public void ShowCell(bool show)
    {
        if (show)
        {
            
            _element.style.display = DisplayStyle.Flex;
        }
        else
        {
            _element.style.display = DisplayStyle.None;
        }
        
    }

    public VisualElement GetElement()
    {
        return _element;
    }

    public int GetPosition()
    {
        return _position;
    }

    public void SetIcon(int skillId  , int level)
    {
        if (_skill == null) _skill = SkillgrpTable.Instance.GetSkill(skillId , level);
        var icon = IconManager.Instance.LoadTextureByName(_skill.Icon);
        var foundChild = _element.Query<VisualElement>(name: "SlotBg").First();
        if(foundChild != null) foundChild.style.backgroundImage = icon;
    }




}
