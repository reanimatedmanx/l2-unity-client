using System.Runtime.CompilerServices;
using UnityEngine;

public class OtherModel
{
    public object _otherData;
    public int _id;
    public string _count;
    public int _price;
    public OtherModel(object otherData)
    {
        _otherData = otherData;
    }

    public OtherModel(int id , string count , int price)
    {
        _id = _id;
        _count = count;
        _price = price;
    }

    public object GetOtherModel()
    {
        return _otherData;
    }


}
