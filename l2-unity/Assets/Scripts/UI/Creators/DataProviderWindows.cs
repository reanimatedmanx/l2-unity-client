using UnityEngine;
using UnityEngine.UIElements;

public class DataProviderWindows :AbstractDataProvider
{
   public void AddLearnSkill(int skillId , int correctCost , int level , VisualElement container)
   {
        Skillgrp skill = SkillgrpTable.Instance.GetSkill(skillId , level);
        SkillNameData skillName = SkillgrpTable.Instance.GetSkillName(skillId, level);
        //set icon
        VisualElement groupBoxIcon = container.Q<VisualElement>("GrowIcon");
        VisualElement icon = container.Q<VisualElement>("icon");
        Texture2D texture = IconManager.Instance.LoadTextureByName(skill.Icon);
        AddElementIfNotNull(groupBoxIcon, icon, texture);

        Label nameLabel = container.Q<Label>("nameLabel");
        Label levelLabel = container.Q<Label>("settingLevel");
        Label spLabel = container.Q<Label>("settingSp");


        AddElementIfNotEmpty(nameLabel, nameLabel, skillName._name);
        AddElementIfNotEmpty(levelLabel, levelLabel, level.ToString());
        AddElementIfNotEmpty(spLabel, spLabel, correctCost.ToString());
    }
}
