
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEditor.Progress;


public class TooltipDataProvider : AbstractDataProvider
{
    private CreatorSets _creator;
    public TooltipDataProvider()
    {
        _creator = new CreatorSets();
    }
    public  void AddDataWeapon(TemplateContainer container, object item)
    {

        if (item != null)
        {
            if(item.GetType() == typeof(Product))
            {
                Product product = (Product)item;
                Weapongrp weapon = product.GetWeapon();
                IDataTips text = ToolTipManager.GetInstance().GetProductText(product);
                SetDataWeaponInTemplate(container, weapon, text);
            }else if (item.GetType() == typeof(ItemInstance))
            {
                ItemInstance item_inventory = (ItemInstance)item;
                Weapongrp weapon = item_inventory.GetWeapon();
                IDataTips text = ToolTipManager.GetInstance().GetProductText(item_inventory);
                SetDataWeaponInTemplate(container, weapon, text);
            }
        }

    }

 

    public void AddDataAccessories(TemplateContainer container, object item)
    {
        if (item != null)
        {
            if (item.GetType() == typeof(Product))
            {
                Product product = (Product)item;
                Armorgrp weapon = product.GetArmor();
                IDataTips text = ToolTipManager.GetInstance().GetProductText(product);
                SetDataAccessoriesInTemplate(container, weapon, text , product.Price , product.GetTypeAccessoriesName());
            }
            else if (item.GetType() == typeof(ItemInstance))
            {
                ItemInstance item_inventory = (ItemInstance)item;
                Armorgrp weapon = item_inventory.GetArmor();
                IDataTips text = ToolTipManager.GetInstance().GetProductText(item_inventory);
                SetDataAccessoriesInTemplate(container, weapon, text , 0 , item_inventory.GetTypeAccessoriesName());
            }
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


    public void AddDataArmor(TemplateContainer container, object item , VisualTreeAsset setsElements, VisualTreeAsset setsEffect)
    {
        if (item != null)
        {
            Abstractgrp data = null;
            int price = 0;
            string armorTypeName = "";

            if (item.GetType() == typeof(Product))
            {
                Product product = (Product)item;
                price = product.Price;
                armorTypeName = product.GetTypeArmorName();
                data = product.GetOAbstractItem();
            }
            else if (item.GetType() == typeof(ItemInstance))
            {
                ItemInstance itemInstance = (ItemInstance)item;
                armorTypeName = itemInstance.GetTypeArmorName();
                data = itemInstance.GetOAbstractItem();
            }
                

            if (data != null)
            {
                IDataTips text = ToolTipManager.GetInstance().GetProductText(item);

                if (data.GetType() == typeof(Armorgrp))
                {
                    Armorgrp armor = (Armorgrp)data;

                    if(armor != null)
                    {
                        AddArmor(container, price, armorTypeName, text, armor , setsElements , setsEffect,  _creator);
                    }
                }
                else if (data.GetType() == typeof(Weapongrp))
                {

                    Weapongrp armor = (Weapongrp)data;
                    if (armor.WeaponType == WeaponType.shield)
                    {
                        AddArmorShield(container, price, armorTypeName, text, armor);
                    }

                }
            }
        }
    }

    

    



   

 


 




}
