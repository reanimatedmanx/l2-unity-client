using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class ShowListWindow : L2PopupWindow
{
    private static ShowListWindow _instance;
    public static ShowListWindow Instance { get { return _instance; } }
    private Button _сancelButton;
    private Button _okButton;
    private DropdownField dropdown;
    private VisualTreeAsset _item;
    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    protected override void LoadAssets()
    {
        _windowTemplate = LoadAsset("Data/UI/_Elements/Game/ShowList/ShowListWindow");
        _item = LoadAsset("Data/UI/_Elements/Template/DroplistItem");
    }

    protected override IEnumerator BuildWindow(VisualElement root)
    {
        InitWindow(root);
        yield return new WaitForEndOfFrame();

        _сancelButton = _windowEle.Q<Button>("CancelButton");
        _okButton = _windowEle.Q<Button>("OkButton");
        var conent = _windowEle.Q<VisualElement>("content");

        dropdown = conent.Q<DropdownField>("comboBox");
        //var item1 = ToolTipsUtils.CloneOne(_item);
       

        dropdown.choices = new List<string>
        {
            "Option 1",
            "Option 2",
            "Option 3",
            "Option 4"
       };


    
        // Подписка на события фокуса
        dropdown.RegisterCallback<FocusInEvent>(OnDropdownFocusIn);
        dropdown.RegisterCallback<FocusOutEvent>(OnDropdownFocusOut);


        RegisterClickWindowEvent(_windowEle, null);
        OnCenterScreen(_root);
    }







    private void OnDropdownFocusIn(FocusInEvent evt)
    {
        if(evt.currentTarget != null)
        {

            Debug.Log("");
        }
        Debug.Log("Dropdown gained focus.");
        // Ваш код для обработки события фокуса
    }

    private void OnDropdownFocusOut(FocusOutEvent evt)
    {
        Debug.Log("Dropdown lost focus.");
        if (evt.currentTarget != null)
        {
            Debug.Log("");
        }
        // Ваш код для обработки события потери фокуса
    }

    private void OnDestroy()
    {
        _instance = null;
    }

}
