using UnityEngine;
using UnityEngine.UIElements;
using static UnityEditor.Rendering.FilterWindow;

public abstract class AbstractDataProvider
{

    protected void SetDataWeaponInTemplate(TemplateContainer container, Weapongrp weapon, IDataTips text)
    {
        container.Q<Label>("nameWeapon").text = text.GetName();

        if (ItemGrade.none != weapon.Grade)
        {
            var gradeWeapon = container.Q<Label>("gradeWeapon");
            gradeWeapon.text = ItemGradeParser.Converter(weapon.Grade);
        }

        container.Q<Label>("typeWeapon").text = WeaponTypeParser.WeaponTypeName(weapon.WeaponType);
        container.Q<Label>("physLabel").text = weapon.PAtk.ToString();
        container.Q<Label>("magLabel").text = weapon.Matk.ToString();
        container.Q<Label>("spdLabel").text = weapon.GetSpeedName();
        container.Q<Label>("critLabel").text = weapon.CriticalRate.ToString();
        container.Q<Label>("soullabel").text = "X" + weapon.Soulshot.ToString();
        container.Q<Label>("spiritlabel").text = "X" + weapon.Spiritshot.ToString();
        container.Q<Label>("weightlabel").text = weapon.Weight.ToString();
        container.Q<Label>("descriptedLabel").text = text.GetItemDiscription();
        var icon = container.Q<VisualElement>("icon");
        icon.style.backgroundImage = IconManager.Instance.LoadTextureByName(weapon.Icon);
    }


    public void SetDataAccessoriesInTemplate(TemplateContainer container, Armorgrp armor, IDataTips text , int price , string accessoriesName)
    {
        container.Q<Label>("nameAccessories").text = text.GetName();

        if (ItemGrade.none != armor.Grade)
        {
            var gradeAccesories = container.Q<Label>("gradeAcs");
            if (gradeAccesories != null) gradeAccesories.text = ItemGradeParser.Converter(armor.Grade);
        }

        VisualElement groupBoxTypeS1 = container.Q<VisualElement>("typeTextS1");
        Label typeS1 = container.Q<Label>("typeLabelS1");
        VisualElement groupBoxMDef = container.Q<VisualElement>("mdeText");

        VisualElement groupBoxIcon = container.Q<VisualElement>("GrowIcon");
        VisualElement icon = container.Q<VisualElement>("icon");
        Texture2D texture = IconManager.Instance.LoadTextureByName(armor.Icon);
        AddElementIfNotNull(groupBoxIcon, icon, texture);
        AddElementIfNotEmpty(groupBoxTypeS1, typeS1, "");

        //EnabledRow(groupBoxMDef);

        Label mpLabel = container.Q<Label>("mpLabel");
        VisualElement mpGroup = container.Q<VisualElement>("mpText");
        AddElementIfNot0(mpGroup, mpLabel, armor.MpBonus);

        Label textMdef = container.Q<Label>("mdefLabel");
        //textMdef.text = armor.MDef.ToString();
        AddElementIfNotEmpty(groupBoxMDef, textMdef, armor.MDef.ToString());

        VisualElement groupTypeLabel = container.Q<VisualElement>("SettingType");
        Label typeLabel = container.Q<Label>("typeLabel");
        AddElementIfNotEmpty(groupTypeLabel, typeLabel, accessoriesName);

        Label groupPriceLabel = (Label)container.Q<Label>("PriceName");
        Label priceLabel = (Label)container.Q<Label>("priceLabel");
        AddElementPriceifNot0(groupPriceLabel, priceLabel, price);



        //typeLabel.text = product.GetTypeAccessoriesName();

        container.Q<Label>("weightlabel").text = armor.Weight.ToString();
        container.Q<Label>("descriptedLabel").text = text.GetItemDiscription();
    }


    protected void AddArmor(TemplateContainer container, int price, string armorTypeName ,  IDataTips text, Armorgrp armor, VisualTreeAsset setsElements, VisualTreeAsset setsEffects, CreatorSets creator)
    {
        //set icon
        VisualElement groupBoxIcon = container.Q<VisualElement>("GrowIcon");
        VisualElement icon = container.Q<VisualElement>("icon");
        Texture2D texture = IconManager.Instance.LoadTextureByName(armor.Icon);
        AddElementIfNotNull(groupBoxIcon, icon, texture);

        container.Q<Label>("nameAccessories").text = text.GetName();

        if (ItemGrade.none != armor.Grade)
        {
            var gradeAccesories = container.Q<Label>("gradeAcs");
            if (gradeAccesories != null) gradeAccesories.text = ItemGradeParser.Converter(armor.Grade);
        }

        //VisualElement groupBoxType = container.Q<VisualElement>("typeText");
        //VisualElement groupBoxMDef = container.Q<VisualElement>("mdeText");

        //EnabledRow(groupBoxType);
        //EnabledRow(groupBoxMDef);

        VisualElement groupType = container.Q<VisualElement>("typeText");
        Label typeLabel = container.Q<Label>("typeLabel");

        
        Label groupPriceLabel = (Label)container.Q<Label>("PriceName");
        Label priceLabel = (Label)container.Q<Label>("priceLabel");

        AddElementPriceifNot0(groupPriceLabel, priceLabel, price);
        AddElementIfNotEmpty(groupType, typeLabel, armorTypeName);

        Label pdefLabel = container.Q<Label>("pdefLabel");
        VisualElement pdefGroup = container.Q<VisualElement>("pdeText");

        Label mdefLabel = container.Q<Label>("mdefLabel");
        VisualElement mdefGroup = container.Q<VisualElement>("mdeText");

        Label chancedefLabel = container.Q<Label>("chancedefLabel");
        VisualElement chancedefGroup = container.Q<VisualElement>("chancedeText");

        Label dexLabel = container.Q<Label>("dexLabel");
        VisualElement dexGroup = container.Q<VisualElement>("dexText");

        Label mpLabel = container.Q<Label>("mpLabel");
        VisualElement mpGroup = container.Q<VisualElement>("mpText");



        AddElementIfNot0(mpGroup, mpLabel, armor.MpBonus);
        AddElementIfNot0(pdefGroup, pdefLabel, armor.PDef);
        AddElementIfNot0(mdefGroup, mdefLabel, armor.MDef);
        AddElementIfNot0(chancedefGroup, chancedefLabel, 0);
        AddElementIfNot0(dexGroup, dexLabel, 0);
        AddElementIfNot0(dexGroup, dexLabel, 0);

        VisualElement footer_container = container.Q<VisualElement>("Footer");

        creator.AddSetsElement(footer_container, setsElements, setsEffects , text.GetSets() , text.GetSetsEffect());


        container.Q<Label>("weightlabel").text = armor.Weight.ToString();
        container.Q<Label>("descriptedLabel").text = text.GetItemDiscription();
    }

    protected void AddArmorShield(TemplateContainer container, int price, string armorTypeName , IDataTips text, Weapongrp armor)
    {
        //set icon
        VisualElement groupBoxIcon = container.Q<VisualElement>("GrowIcon");
        VisualElement icon = container.Q<VisualElement>("icon");
        Texture2D texture = IconManager.Instance.LoadTextureByName(armor.Icon);
        AddElementIfNotNull(groupBoxIcon, icon, texture);

        container.Q<Label>("nameAccessories").text = text.GetName();

        if (ItemGrade.none != armor.Grade)
        {
            var gradeAccesories = container.Q<Label>("gradeAcs");
            if (gradeAccesories != null) gradeAccesories.text = ItemGradeParser.Converter(armor.Grade);
        }

        VisualElement groupBoxType = container.Q<VisualElement>("typeText");
        VisualElement groupBoxMDef = container.Q<VisualElement>("mdeText");

        EnabledRow(groupBoxType);
        EnabledRow(groupBoxMDef);


        VisualElement groupType = container.Q<VisualElement>("typeText");
        Label typeLabel = container.Q<Label>("typeLabel");

        Label groupPriceLabel = (Label)container.Q<Label>("PriceName");
        Label priceLabel = (Label)container.Q<Label>("priceLabel");

        AddElementPriceifNot0(groupPriceLabel, priceLabel, price);
        AddElementIfNotEmpty(groupType, typeLabel, armorTypeName);

        Label pdefLabel = container.Q<Label>("pdefLabel");
        VisualElement pdefGroup = container.Q<VisualElement>("pdeText");

        Label mdefLabel = container.Q<Label>("mdefLabel");
        VisualElement mdefGroup = container.Q<VisualElement>("mdeText");

        Label chancedefLabel = container.Q<Label>("chancedefLabel");
        VisualElement chancedefGroup = container.Q<VisualElement>("chancedeText");

        Label dexLabel = container.Q<Label>("dexLabel");
        VisualElement dexGroup = container.Q<VisualElement>("dexText");

        Label mpLabel = container.Q<Label>("mpLabel");
        VisualElement mpGroup = container.Q<VisualElement>("mpText");
        AddElementIfNot0(mpGroup, mpLabel, 0);

        AddElementIfNot0(pdefGroup, pdefLabel, armor.ShieldPdef);
        AddElementIfNot0(mdefGroup, mdefLabel, 0);
        AddElementIfNot0(chancedefGroup, chancedefLabel, armor.ShieldRate);
        AddElementIfNot0(dexGroup, dexLabel, armor.Dex);

        container.Q<Label>("weightlabel").text = armor.Weight.ToString();
        container.Q<Label>("descriptedLabel").text = text.GetItemDiscription();
    }

    protected void AddElementIfNotNull(VisualElement groupElement, VisualElement icon, Texture2D texture)
    {
        if (texture != null)
        {
            icon.style.backgroundImage = texture;
            groupElement.style.display = DisplayStyle.Flex;
        }
        else
        {
            groupElement.style.display = DisplayStyle.None;
            icon.style.backgroundImage = null;
        }
    }

    protected void AddElementPriceifNot0(VisualElement groupPriceLabel, Label priceLabel, int price)
    {
        if (price == 0)
        {
            AddElementIfNotEmpty(groupPriceLabel, priceLabel, null);
        }
        else
        {
            AddElementIfNotEmpty(groupPriceLabel, priceLabel, ToolTipsUtils.ConvertToPrice(price) + " Adena");
        }
    }




    protected void AddElementIfNotEmpty(VisualElement groupElement, Label labelData, string text)
    {
        if (!string.IsNullOrEmpty(text))
        {
            labelData.text = text;
            groupElement.style.display = DisplayStyle.Flex;
        }
        else
        {
            groupElement.style.display = DisplayStyle.None;
            labelData.text = "";
        }
    }

    protected void AddElementIfNot0(VisualElement groupElement, Label labelData, int addParam)
    {
        if (addParam != 0)
        {
            labelData.text = addParam.ToString();
            groupElement.style.display = DisplayStyle.Flex;
        }
        else
        {
            if (groupElement == null) { Debug.LogWarning(" ToolTipDataProvider: Не критическая ошибка мы не нашли элемент tooltips "); return; }
            groupElement.style.display = DisplayStyle.None;
            labelData.text = "";
        }
    }

    protected void EnabledRow(VisualElement element)
    {
        if (element != null)
        {
            element.style.display = DisplayStyle.Flex;
        }
    }
}
