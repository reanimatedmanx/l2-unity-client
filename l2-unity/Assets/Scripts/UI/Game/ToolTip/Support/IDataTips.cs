using UnityEngine;

public interface IDataTips
{
    string GetName();
    string GetDiscription();

    string GetPrice();

    string GetItemDiscription();

    ItemName[] GetSets();

    ItemSets[] GetSetsEffect();
    string GetIcon();

    Texture2D GetGradeTexture();
}
