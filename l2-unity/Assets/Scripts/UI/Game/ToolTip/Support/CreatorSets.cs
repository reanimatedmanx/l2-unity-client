using UnityEditorInternal.Profiling.Memory.Experimental;
using UnityEngine;
using UnityEngine.UIElements;

public class CreatorSets
{
    public void AddSetsElement(VisualElement container, VisualTreeAsset setsTemplate, ItemName[] setsName)
    {
        string element = "setsElements";

        if (setsName != null && setsName.Length > 0)
        {

            RemoveSetsElements(container, element);
            CreateSetsPart(container, setsTemplate, setsName);
        }
        else
        {
            RemoveSetsElements(container, element);
        }
    }

    private void RemoveSetsElements(VisualElement container, string nameElement)
    {
        var elementsToRemove = container.Query<VisualElement>(nameElement).ToList();

        if (elementsToRemove != null && elementsToRemove.Count > 0)
        {
            foreach (var element in elementsToRemove)
            {
                element.RemoveFromHierarchy();

            }
        }
    }


    private void CreateSetsPart(VisualElement container, VisualTreeAsset setsTemplate, ItemName[] setsName)
    {
        for (int i = 0; i < setsName.Length; i++)
        {
            ItemName itemName = setsName[i];
            int id = itemName.Id;
            var weapon = WeapongrpTable.Instance.GetWeapon(id);
            var armor = ArmorgrpTable.Instance.GetArmor(id);
            var item = EtcItemgrpTable.Instance.GetEtcItem(id);


            if (weapon != null)
            {

                container.Insert(i, CreateVisualElement(i, weapon, itemName.Name, setsTemplate));
            }
            else if (armor != null)
            {

                container.Insert(i, CreateVisualElement(i, armor, itemName.Name, setsTemplate));
            }
            else if (item != null)
            {

                container.Insert(i, CreateVisualElement(i, item, itemName.Name, setsTemplate));
            }
        }
    }

    private TemplateContainer CreateVisualElement(int i, Abstractgrp grpItem, string name, VisualTreeAsset setsTemplate)
    {
        var template = setsTemplate.CloneTree();


        CreateHeaderif0Count(i, template);
        SetDataIcon(grpItem, template);
        SetDataLabel(name, template);

        return template;
    }

    private void CreateHeaderif0Count(int i , TemplateContainer template)
    {
        if (i == 0)
        {
            Label labelHeader = template.Q<Label>("labelSetHeader");
            labelHeader.text = "<Set Effect>";
        }
    }

    private void SetDataIcon(Abstractgrp grpItem , TemplateContainer template)
    {
        VisualElement iconSet = template.Q<VisualElement>("iconSet");
        if (iconSet != null)
        {
            Texture2D texture = IconManager.Instance.LoadTextureByName(grpItem.Icon);
            iconSet.style.backgroundImage = texture;
        }
    }

    private void SetDataLabel(string name, TemplateContainer template)
    {
        Label labelSet = template.Q<Label>("labelNameSet");
        if (labelSet != null)
        {
            labelSet.text = name;
        }

    }

}
