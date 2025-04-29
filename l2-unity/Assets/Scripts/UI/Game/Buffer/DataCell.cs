using UnityEngine;
using UnityEngine.UIElements;

public class DataCell
{
    private bool _isBusy;
    private int _idSkill;
    private VisualElement _element;


    public DataCell(int idSkill , VisualElement element)
    {
        _idSkill = idSkill;
        _element = element;
        _isBusy = false;

    }

    public void RefreshData(int skillId , bool isbusy ,int level)
    {
        SetSkillId(skillId);
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

    public bool IsBusy()
    {
        return _isBusy;
    }

    public void SetBusy(bool isBusy)
    {
        _isBusy = isBusy;
    }

    public void ShowCell(bool show)
    {
        if (show)
        {
            _element.style.opacity = 1;
        }
        else
        {
            _element.style.opacity = 0;
        }
        
    }

    public VisualElement GetElement()
    {
        return _element;
    }



    public void SetIcon(int skillId  , int level)
    {
        Skillgrp skill = SkillgrpTable.Instance.GetSkill(skillId , level);
        var icon = IconManager.Instance.LoadTextureByName(skill.Icon);
        var foundChild = _element.Query<VisualElement>(name: "SlotBg").First();
        if(foundChild != null) foundChild.style.backgroundImage = icon;
    }




}
