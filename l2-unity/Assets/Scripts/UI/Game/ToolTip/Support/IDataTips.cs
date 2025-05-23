using UnityEngine;

public interface IDataTips
{
    string GetName();
    string GetDiscription();

    string GetItemDiscription();

    ItemName[] GetSets();
    string GetIcon();
}
