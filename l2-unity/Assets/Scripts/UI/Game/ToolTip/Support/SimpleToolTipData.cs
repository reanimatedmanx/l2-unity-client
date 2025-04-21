using UnityEngine;

public class SimpleToolTipData : IDataTips
{
    private string _name;
    private string _description;
    private string _item_description;
    public SimpleToolTipData(object data)
    {
        if(data.GetType() == typeof(Product))
        {
            Product product = (Product)data;
            _name = product.GetName();
            _description = product.GetDescription();
            _item_description = product.GetItemDescription();
        }
    }
    public string GetName()
    {
        return _name;
    }

    public string GetDiscription()
    {
        return _description;
    }

    public string GetItemDiscription()
    {
        return _item_description;
    }

    public string GetIcon()
    {
        throw new System.NotImplementedException();
    }

   
}
