using System.Globalization;
using UnityEditorInternal.Profiling.Memory.Experimental;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEditor.Rendering.FilterWindow;
using static UnityEngine.Rendering.DebugUI.MessageBox;

public class TooltipDataProvider
{
    private CreatorSets _creator;
    public TooltipDataProvider()
    {
        _creator = new CreatorSets();
    }
    public  void AddDataWeapon(TemplateContainer container, Product product)
    {

        if (product != null)
        {
            Weapongrp weapon = product.GetWeapon();
            IDataTips text = ToolTipManager.GetInstance().GetProductText(product);

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

    }

    public void AddDataAccessories(TemplateContainer container, Product product)
    {
        if (product != null)
        {
            Armorgrp armor = product.GetArmor();

            IDataTips text = ToolTipManager.GetInstance().GetProductText(product);

            container.Q<Label>("nameAccessories").text = text.GetName();

            if (ItemGrade.none != armor.Grade)
            {
                var gradeAccesories = container.Q<Label>("gradeAcs");
                if(gradeAccesories != null) gradeAccesories.text = ItemGradeParser.Converter(armor.Grade);
            }

            VisualElement groupBoxType = container.Q<VisualElement>("typeText");
            VisualElement groupBoxMDef = container.Q<VisualElement>("mdeText");

            VisualElement groupBoxIcon = container.Q<VisualElement>("GrowIcon");
            VisualElement icon = container.Q<VisualElement>("icon");
            Texture2D texture = IconManager.Instance.LoadTextureByName(armor.Icon);
            AddElementIfNotNull(groupBoxIcon, icon, texture);
            EnabledRow(groupBoxType);
            EnabledRow(groupBoxMDef);

            Label mpLabel = container.Q<Label>("mpLabel");
            VisualElement mpGroup = container.Q<VisualElement>("mpText");
            AddElementIfNot0(mpGroup, mpLabel, armor.MpBonus);

            Label textMdef = container.Q<Label>("mdefLabel");
            textMdef.text = armor.MDef.ToString();

            VisualElement groupTypeLabel = container.Q<VisualElement>("SettingType");
            Label typeLabel  = container.Q<Label>("typeLabel");
            AddElementIfNotEmpty(groupTypeLabel, typeLabel, product.GetTypeAccessoriesName());

            Label price = (Label)container.Q<Label>("priceLabel");
            //price.style.color = ToolTipsUtils.GetColorPrice(text.GetDiscription());
            price.text = ToolTipsUtils.ConvertToPrice(product.Price) + " Adena";

            
            //typeLabel.text = product.GetTypeAccessoriesName();
           
            container.Q<Label>("weightlabel").text = armor.Weight.ToString();
            container.Q<Label>("descriptedLabel").text = text.GetItemDiscription();

        }
    }

    public void AddDataOther(TemplateContainer container, Product product)
    {
        if (product != null)
        {
            
            EtcItemgrp etcItem = product.GetEtcItem();

            IDataTips text = ToolTipManager.GetInstance().GetProductText(product);

            container.Q<Label>("nameAccessories").text = text.GetName();

            //set icon
            VisualElement groupBoxIcon = container.Q<VisualElement>("GrowIcon");
            VisualElement icon = container.Q<VisualElement>("icon");
            Texture2D texture = IconManager.Instance.LoadTextureByName(etcItem.Icon);
            AddElementIfNotNull(groupBoxIcon, icon, texture);

            Label mpLabel = container.Q<Label>("mpLabel");
            VisualElement mpGroup = container.Q<VisualElement>("mpText");
            AddElementIfNot0(mpGroup, mpLabel, 0);

            //set price
            Label price = (Label)container.Q<Label>("priceLabel");
            //price.style.color = ToolTipsUtils.GetColorPrice(text.GetDiscription());
            price.text = ToolTipsUtils.ConvertToPrice(product.Price) + " Adena";


            VisualElement groupBoxMdef = (VisualElement)container.Q<VisualElement>("mdeText");
            Label lebelMdef = (Label)container.Q<Label>("mdefLabel");

            //set mdef
            AddElementIfNot0(groupBoxMdef, lebelMdef, 0);

            //set type
            VisualElement groupTypeLabel = container.Q<VisualElement>("SettingType");
            Label typeLabel = container.Q<Label>("typeLabel");
            AddElementIfNotEmpty(groupTypeLabel, typeLabel, product.GetTypeAccessoriesName());

            VisualElement groupBoxWeight = container.Q<VisualElement>("weightText");
            Label weightLabel = container.Q<Label>("weightlabel");
            AddElementIfNotEmpty(groupBoxWeight, weightLabel, etcItem.Weight.ToString());
            
            container.Q<Label>("descriptedLabel").text = text.GetItemDiscription();

        }
    }


    public void AddDataArmor(TemplateContainer container, Product product , VisualTreeAsset setsElements)
    {
        if (product != null)
        {

            Abstractgrp data = product.GetOAbstractItem();

            if (data != null)
            {
                IDataTips text = ToolTipManager.GetInstance().GetProductText(product);

                if (data.GetType() == typeof(Armorgrp))
                {
                    Armorgrp armor = (Armorgrp)data;

                    if(armor != null)
                    {
                        AddArmor(container, product, text, armor , setsElements);
                    }
                }
                else if (data.GetType() == typeof(Weapongrp))
                {

                    Weapongrp armor = (Weapongrp)data;
                    if (armor.WeaponType == WeaponType.shield)
                    {
                        AddArmorShield(container, product, text, armor);
                    }

                }
            }
        }
    }

    private void AddArmorShield(TemplateContainer container, Product product , IDataTips text , Weapongrp armor)
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

        Label price = (Label)container.Q<Label>("priceLabel");
        //price.style.color = ToolTipsUtils.GetColorPrice(text.GetDiscription());
        price.text = ToolTipsUtils.ConvertToPrice(product.Price) + " Adena";

        AddElementIfNotEmpty(groupType, typeLabel, product.GetTypeWeaponName());

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

    private void AddArmor(TemplateContainer container, Product product, IDataTips text, Armorgrp armor , VisualTreeAsset setsElements)
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


        Label price = (Label)container.Q<Label>("priceLabel");
        //price.style.color = ToolTipsUtils.GetColorPrice(text.GetDiscription());
        price.text = ToolTipsUtils.ConvertToPrice(product.Price) + " Adena";


        AddElementIfNotEmpty(groupType, typeLabel, product.GetTypeArmorName());

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

        _creator.AddSetsElement(footer_container, setsElements, text.GetSets());


        container.Q<Label>("weightlabel").text = armor.Weight.ToString();
        container.Q<Label>("descriptedLabel").text = text.GetItemDiscription();
    }

    private void AddElementIfNot0(VisualElement groupElement , Label labelData , int addParam)
    {
        if(addParam != 0)
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

    private void AddElementIfNotNull(VisualElement groupElement, VisualElement icon, Texture2D texture)
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

 


  


    private void AddElementArrIfNot0(VisualElement groupElement, Label labelData, ItemName[] setsName)
    {
        if (setsName != null && setsName.Length > 0)
        {
            for(int i=0; i < setsName.Length; i++)
            {
                if(i == 0)
                {
                    labelData.text = setsName[i].Name.ToString();
                }
                else
                {
                    labelData.text = labelData.text + "\n"+ setsName[i].Name.ToString();
                }
            }
            groupElement.style.display = DisplayStyle.Flex;
        }
        else
        {
            groupElement.style.display = DisplayStyle.None;
            labelData.text = "";
        }
    }

    private void AddElementIfNotEmpty(VisualElement groupElement, Label labelData, string text)
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

    private void DisabledRow(VisualElement element )
    {
        if(element != null)
        {
            element.style.display = DisplayStyle.None;
        }
    }

    private void EnabledRow(VisualElement element)
    {
        if (element != null)
        {
            element.style.display = DisplayStyle.Flex;
        }
    }


}
