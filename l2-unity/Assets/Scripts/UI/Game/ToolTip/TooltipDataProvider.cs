using System.Globalization;
using UnityEngine;
using UnityEngine.UIElements;

public class TooltipDataProvider
{
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

            Label price = (Label)container.Q<Label>("priceLabel");
            price.style.color = ToolTipsUtils.GetColorPrice(text.GetDiscription());
            price.text = ToolTipsUtils.ConvertToPrice(product.Price) + " Adena";

            container.Q<Label>("typeLabel").text = product.GetTypeAccessoriesName();
            container.Q<Label>("mdefLabel").text = armor.MDef.ToString();
            container.Q<Label>("weightlabel").text = armor.Weight.ToString();
            container.Q<Label>("descriptedLabel").text = text.GetItemDiscription();

        }
    }

   
}
