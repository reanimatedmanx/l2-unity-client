using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CreatorVerticalScrollWindows : ICreator
{
    public event Action<int, ItemCategory, int> EventLeftClick;
    private ScrollView _scrollView;
    private VisualElement _content;
    private string[] _nameTabs;
    private VisualTreeAsset _templateItems;
    private DataProviderWindows _dataProvider;

    public CreatorVerticalScrollWindows()
    {
        _dataProvider = new DataProviderWindows();
    }



    public void CreateTabs(VisualElement content, VisualTreeAsset empty, VisualTreeAsset templateItems)
    {
        _scrollView = (ScrollView)content.Q<ScrollView>("ScrollView");

        if(_scrollView == null & templateItems == null)
        {
            Debug.LogError("CreatorHorizontalWindows> Not found ScrollView in content element");
        }

        _content = _scrollView.contentContainer;
        _templateItems = templateItems;
    }

    public ItemInstance GetActiveByPosition(int position)
    {
        throw new NotImplementedException();
    }

    public void InitTabs(string[] nameTabs)
    {
        _nameTabs = nameTabs ?? new string[0];
    }

    public void SetClickActiveTab(int position)
    {
        throw new NotImplementedException();
    }

    private VisualElement CloneTemplte(VisualTreeAsset templateItems)
    {
        return ToolTipsUtils.CloneOne(templateItems);
    }


    public void AddDataItem(List<object> allItems)
    {
        throw new NotImplementedException();
    }

    public void AddData(List<ItemInstance> allItems)
    {
        throw new NotImplementedException();
    }

    public void AddOtherData(List<OtherModel> allItems)
    {
        if (_content != null & allItems != null)
        {
            for (int i = 0; i < allItems.Count; i++)
            {
                AcquireData item = (AcquireData)allItems[i].GetOtherModel();

                VisualElement element = CloneTemplte(_templateItems);
                _dataProvider.AddLearnSkill(item.GetId(), item.GetCost(), item.GetValue1(), element);
                _content.Add(element);
            }
        }
    }
}
