using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;
using static UnityEditor.Rendering.FilterWindow;

public abstract class AbstractDataProvider
{

    protected void SetDataWeaponInTemplate(VisualElement container, Weapongrp weapon, int price , IDataTips text)
    {
        container.Q<Label>("nameWeapon").text = text.GetName();

        VisualElement gradeBox = container.Q<VisualElement>("grade");
        Texture2D grade = text.GetGradeTexture();
        SetImageElement(gradeBox, grade);

        VisualElement groupPriceLabel = container.Q<VisualElement>("PriceName");
        Label priceLabel = (Label)container.Q<Label>("priceLabel");
        AddElementPriceifNot0(groupPriceLabel, priceLabel, price);

        VisualElement groupType = container.Q<VisualElement>("SettingType");
        Label typeWeapon = container.Q<Label>("typeWeapon");
        AddElementIfNotEmpty(groupType, typeWeapon, WeaponTypeParser.WeaponTypeName(weapon.WeaponType));


        Label labelEnchant = container.Q<Label>("enchant");
        AddElementEnchantifNot0(labelEnchant, labelEnchant, text.GetEnchant());

        VisualElement groupPhys = container.Q<VisualElement>("phisAtkText");
        Label physLabel = container.Q<Label>("physLabel");
        AddElementIfNotEmpty(groupPhys, physLabel, weapon.PAtk.ToString());

        VisualElement groupMag = container.Q<VisualElement>("magAtkText");
        Label magLabel = container.Q<Label>("magLabel");
        AddElementIfNotEmpty(groupMag, magLabel, weapon.Matk.ToString());



        VisualElement groupSpd = container.Q<VisualElement>("speedText");
        Label spdLabel = container.Q<Label>("spdLabel");
        AddElementIfNotEmpty(groupSpd, spdLabel, weapon.GetSpeedName());



        VisualElement groupCrit = container.Q<VisualElement>("critText");
        Label critLabel = container.Q<Label>("critLabel");
        AddElementIfNotEmpty(groupCrit, critLabel, weapon.CriticalRate.ToString());



        VisualElement groupSoul = container.Q<VisualElement>("soulText");
        Label soulLabel = container.Q<Label>("soullabel");
        AddElementIfNotEmpty(groupSoul, soulLabel, "X" + weapon.Soulshot.ToString());



        VisualElement groupSpirit = container.Q<VisualElement>("spiritText");
        Label spiritLabel = container.Q<Label>("spiritlabel");
        AddElementIfNotEmpty(groupSpirit, spiritLabel, "X" + weapon.Spiritshot.ToString());


        VisualElement groupWeight = container.Q<VisualElement>("weightText");
        Label weightLabel = container.Q<Label>("weightlabel");
        AddElementIfNotEmpty(groupWeight, weightLabel, weapon.Weight.ToString());


        VisualElement groupDescr = container.Q<VisualElement>("descriptedText");
        Label descrLabel = container.Q<Label>("descriptedLabel");
        AddElementIfNotEmpty(groupDescr, descrLabel, text.GetItemDiscription());


        VisualElement groupIcon = container.Q<VisualElement>("GrowIcon");
        var icon = container.Q<VisualElement>("icon");
        AddElementIfNotNull(groupIcon, icon, IconManager.Instance.LoadTextureByName(weapon.Icon));
    }


    public void SetDataAccessoriesInTemplate(VisualElement container, Armorgrp armor, IDataTips text , int price , string accessoriesName)
    {
        container.Q<Label>("nameAccessories").text = text.GetName();

        VisualElement gradeBox = container.Q<VisualElement>("grade");
        Texture2D grade = text.GetGradeTexture();
        SetImageElement(gradeBox, grade);

        VisualElement groupBoxTypeS1 = container.Q<VisualElement>("typeTextS1");
        Label typeS1 = container.Q<Label>("typeLabelS1");
        VisualElement groupBoxMDef = container.Q<VisualElement>("mdeText");

        VisualElement groupBoxIcon = container.Q<VisualElement>("GrowIcon");
        VisualElement icon = container.Q<VisualElement>("icon");
        Texture2D texture = IconManager.Instance.LoadTextureByName(armor.Icon);
        AddElementIfNotNull(groupBoxIcon, icon, texture);
        AddElementIfNotEmpty(groupBoxTypeS1, typeS1, "");

        Label labelEnchant = container.Q<Label>("enchant");
        AddElementEnchantifNot0(labelEnchant, labelEnchant, text.GetEnchant());


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


    protected void SetOther(VisualElement container , IDataTips text , string etcIcon , int priceItem ,  string accessoriesName , string weight)
    {
     
            container.Q<Label>("nameAccessories").text = text.GetName();

            //set icon
            VisualElement groupBoxIcon = container.Q<VisualElement>("GrowIcon");
            VisualElement icon = container.Q<VisualElement>("icon");
            Texture2D texture = IconManager.Instance.LoadTextureByName(etcIcon);
            AddElementIfNotNull(groupBoxIcon, icon, texture);

            VisualElement gradeBox = container.Q<VisualElement>("grade");
            Texture2D grade = text.GetGradeTexture();
            SetImageElement(gradeBox, grade);

            Label labelEnchant = container.Q<Label>("enchant");
            AddElementEnchantifNot0(labelEnchant, labelEnchant, text.GetEnchant());


            Label mpLabel = container.Q<Label>("mpLabel");
            VisualElement mpGroup = container.Q<VisualElement>("mpText");
            AddElementIfNot0(mpGroup, mpLabel, 0);

            Label groupPriceLabel = (Label)container.Q<Label>("PriceName");
            Label priceLabel = (Label)container.Q<Label>("priceLabel");
            AddElementPriceifNot0(groupPriceLabel, priceLabel, priceItem);
            //price.style.color = ToolTipsUtils.GetColorPrice(text.GetDiscription());
            //price.text = ToolTipsUtils.ConvertToPrice(priceItem) + " Adena";


            VisualElement groupBoxMdef = (VisualElement)container.Q<VisualElement>("mdeText");
            Label lebelMdef = (Label)container.Q<Label>("mdefLabel");

            //set mdef
            AddElementIfNot0(groupBoxMdef, lebelMdef, 0);

            //set type
            VisualElement groupTypeLabel = container.Q<VisualElement>("SettingType");
            Label typeLabel = container.Q<Label>("typeLabel");
            AddElementIfNotEmpty(groupTypeLabel, typeLabel, accessoriesName);

            VisualElement groupBoxWeight = container.Q<VisualElement>("weightText");
            Label weightLabel = container.Q<Label>("weightlabel");
            AddElementIfNotEmpty(groupBoxWeight, weightLabel, weight);

            Label descriptionLabel = container.Q<Label>("descriptedLabel");
            AddElementIfNotEmpty(descriptionLabel, descriptionLabel, text.GetItemDiscription());
        //container.Q<Label>("descriptedLabel").text = text.GetItemDiscription();

        
    }

    protected void AddArmor(VisualElement container, int price, string armorTypeName ,  IDataTips text, Armorgrp armor, VisualTreeAsset setsElements, VisualTreeAsset setsEffects, CreatorSets creator)
    {
        //set icon
        VisualElement groupBoxIcon = container.Q<VisualElement>("GrowIcon");
        
        VisualElement icon = container.Q<VisualElement>("icon");
        Texture2D texture = IconManager.Instance.LoadTextureByName(armor.Icon);
        AddElementIfNotNull(groupBoxIcon, icon, texture);

        container.Q<Label>("nameAccessories").text = text.GetName();

        VisualElement gradeBox = container.Q<VisualElement>("grade");
        Texture2D grade = text.GetGradeTexture();
        SetImageElement(gradeBox, grade);
    

        //VisualElement groupBoxType = container.Q<VisualElement>("typeText");
        //VisualElement groupBoxMDef = container.Q<VisualElement>("mdeText");

        //EnabledRow(groupBoxType);
        //EnabledRow(groupBoxMDef);


        Label labelEnchant = container.Q<Label>("enchant");
        AddElementEnchantifNot0(labelEnchant, labelEnchant, text.GetEnchant());

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

        Label descriptionLabel = container.Q<Label>("descriptedLabel");
        AddElementIfNotEmpty(descriptionLabel, descriptionLabel, text.GetItemDiscription());

        //container.Q<Label>("descriptedLabel").text = text.GetItemDiscription();
    }

    protected void AddArmorShield(VisualElement container, int price, string armorTypeName , IDataTips text, Weapongrp armor)
    {
        //set icon
        VisualElement groupBoxIcon = container.Q<VisualElement>("GrowIcon");
        VisualElement icon = container.Q<VisualElement>("icon");
        Texture2D texture = IconManager.Instance.LoadTextureByName(armor.Icon);
        AddElementIfNotNull(groupBoxIcon, icon, texture);

        container.Q<Label>("nameAccessories").text = text.GetName();

        VisualElement gradeBox = container.Q<VisualElement>("grade");
        Texture2D grade = text.GetGradeTexture();
        SetImageElement(gradeBox, grade);

        VisualElement groupBoxType = container.Q<VisualElement>("typeText");
        VisualElement groupBoxMDef = container.Q<VisualElement>("mdeText");

        EnabledRow(groupBoxType);
        EnabledRow(groupBoxMDef);



        VisualElement groupType = container.Q<VisualElement>("typeText");
        Label typeLabel = container.Q<Label>("typeLabel");

        Label groupPriceLabel = (Label)container.Q<Label>("PriceName");
        Label priceLabel = (Label)container.Q<Label>("priceLabel");

        Label labelEnchant = container.Q<Label>("enchant");
        AddElementEnchantifNot0(labelEnchant, labelEnchant, text.GetEnchant());

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

        Label descriptionLabel = container.Q<Label>("descriptedLabel");
        AddElementIfNotEmpty(descriptionLabel, descriptionLabel, text.GetItemDiscription());


        //container.Q<Label>("descriptedLabel").text = text.GetItemDiscription();
    }

    protected void SetDataIngredient(VisualElement container, IDataTips text, string icon, int count, string name)
    {

        VisualElement gradeBox = container.Q<VisualElement>("grade");
        Texture2D grade = text.GetGradeTexture();
        SetImageElement(gradeBox, grade);

        VisualElement groupIcon = container.Q<VisualElement>("GrowIcon");
        var iconElement = container.Q<VisualElement>("icon");
        AddElementIfNotNull(groupIcon, iconElement, IconManager.Instance.LoadTextureByName(icon));

        VisualElement groupPriceLabel = container.Q<VisualElement>("GrowPrice");
        Label priceLabel = (Label)container.Q<Label>("price");
        AddElementCountXifNot0(groupPriceLabel, priceLabel, count);

        VisualElement groupName = container.Q<VisualElement>("GrowName");
        Label labelName = container.Q<Label>("nameLabel");
        AddElementIfNotEmpty(groupName, labelName, name);
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

    protected void AddElementCountXifNot0(VisualElement groupPriceLabel, Label priceLabel, int price)
    {
        if (price == 0)
        {
            AddElementIfNotEmpty(groupPriceLabel, priceLabel, null);
        }
        else
        {
            AddElementIfNotEmpty(groupPriceLabel, priceLabel, ToolTipsUtils.ConvertToPrice(price));
        }
    }

    protected void AddElementEnchantifNot0(VisualElement groupPriceLabel, Label priceLabel, int enchant)
    {
        if(groupPriceLabel != null)
        {
            if (enchant == 0)
            {
                AddElementIfNotEmpty(groupPriceLabel, priceLabel, null);
            }
            else
            {
                AddElementIfNotEmpty(groupPriceLabel, priceLabel, "+" + enchant);
            }
        }

    }




    protected void AddElementIfNotEmpty(VisualElement groupElement, Label labelData, string text)
    {
        if(groupElement != null)
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

    protected void SetImageElement(VisualElement element, Texture2D texture)
    {
        if (element != null)
        {
            if (texture == null)
            {
                element.style.display = DisplayStyle.None;
            }
            else
            {
                element.style.display = DisplayStyle.Flex;
                element.style.backgroundImage = texture;
            }
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
