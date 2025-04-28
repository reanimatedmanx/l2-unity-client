using UnityEngine;
using UnityEngine.UIElements;

public class DataCell : MonoBehaviour
{
    private bool _isBusy;
    private float _idSkill;
    private VisualElement _element;

    public DataCell(float idSkill , VisualElement element)
    {
        _idSkill = idSkill;
        _element = element;
        _isBusy = false;
    }

    public bool IsBusy()
    {
        return _isBusy;
    }

    public void SetBusy(bool isBusy)
    {
        _isBusy = isBusy;
    }

    public VisualElement GetElement()
    {
        return _element;
    }

    public float GetSkillId()
    {
        return _idSkill;
    }

    public void SetIcon(int skillId  , int level)
    {
        Skillgrp skill = SkillgrpTable.Instance.GetSkill(skillId , level);
        Texture2D icon = IconManager.Instance.LoadTextureByName(skill.Icon);
        var foundChild = _element.Query<VisualElement>(name: "SlotBg").First();
        foundChild.style.backgroundImage = icon;
    }
}
