using UnityEngine;

public class SimpleToolTipData : IDataTips
{
    private string _name;
    private string _description;
    private string _item_description;
    private Product _product;
    public SimpleToolTipData(object data)
    {
        if(data.GetType() == typeof(Product))
        {
            _product = (Product)data;
            _name = _product.GetName();
            _description = _product.GetDescription();
            _item_description = _product.GetItemDescription();
        }
    }
    public string GetName()
    {
        if(_product.Count > 1) _name = _name +" (" + _product.Count + ")";
        return _name;
    }

    public ItemName[] GetSets()
    {
        return _product.GetSets();
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
