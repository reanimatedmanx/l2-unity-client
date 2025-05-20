using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Windows;
using static L2Slot;
using static UnityEditor.Experimental.GraphView.Port;
using static UnityEditor.Rendering.FilterWindow;

public class QuantityInput : L2PopupWindow
{
    private static QuantityInput _instance;
    private DealerWindow _dealerWindow;
    private VisualElement _root;
    private TextField _userInput;
    private Product _selectProduct;
    private SlotType _type;
    private List<Product> _listServer;
    private int _position;
    public static QuantityInput Instance { get { return _instance; } }

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
        _windowTemplate = LoadAsset("Data/UI/_Elements/Game/Calculator/QuantityInput");
    }



    protected override IEnumerator BuildWindow(VisualElement root)
    {


        InitWindow(root);

        _userInput = (TextField)GetElementById("UserInputField");
        VisualElement inputBg = (VisualElement)GetElementById("UserInputBg");
        Button close = (Button)GetElementById("CloseButton");
        Button success = (Button)GetElementById("SuccessButton");


        close.RegisterCallback<ClickEvent>(OnClickClose);
        success.RegisterCallback<ClickEvent>(OnClickSuccess);
        RegisterClickWindowEvent(_windowEle, null);


        RegisterAllNumbers();

        _userInput.RegisterValueChangedCallback(evt => OnValueChanged(evt));
        _userInput.RegisterCallback<ClickEvent>((evt) => OnClick(evt, inputBg));

        OnCenterScreen(root);
        yield return new WaitForEndOfFrame();

        
    }

    private void RegisterAllNumbers()
    {
        for(int i = 0; i < 10; i++)
        {
            var btn = (Button)GetElementById("Button"+i);
            btn.RegisterCallback<ClickEvent>(OnPlus);
        }


        Button buttonBs = (Button)GetElementById("ButtonBs");
        buttonBs.RegisterCallback<ClickEvent>(Back);
        Button buttonC = (Button)GetElementById("ButtonC");
        buttonC.RegisterCallback<ClickEvent>(OnClear);
        Button buttonALL = (Button)GetElementById("ButtonALL");
        buttonALL.RegisterCallback<ClickEvent>(OnAll);

        Debug.Log("");
    }


    private void OnPlus(ClickEvent evt)
    {
        Button clickedElement = evt.currentTarget as Button;
        string number = clickedElement.name.Replace("Button", "");
        if (_userInput.value.Length > 20) return;
        if (long.TryParse(number, out _))
        {
            string value1 = ToolTipsUtils.ConvertPriceToNormal(_userInput.value + number);
            long numberInt = long.Parse(value1);
            _userInput.value = ToolTipsUtils.ConvertToPrice(numberInt);
        }
    }

    private void OnClear(ClickEvent evt)
    {
        _userInput.value = 0.ToString();
    }

    private void Back(ClickEvent evt)
    {
        string value1 = ToolTipsUtils.ConvertPriceToNormal(_userInput.value);
        if(value1.Length == 1) _userInput.value = "0";
        if (value1.Length > 0)
        {
            string modifiedString = value1.Substring(0, value1.Length - 1);
            _userInput.value = modifiedString;
        }

    }

    private void OnAll(ClickEvent evt)
    {
        if(_selectProduct.Count > 0)
        {
            _userInput.value = _selectProduct.Count.ToString();
        }
    }


    private void OnClick(ClickEvent evt ,  VisualElement inputBg)
    {
        Debug.Log("Click Input files !!!");
        if (!inputBg.ClassListContains("calculator_field_click_bg"))
        {
            inputBg.AddToClassList("calculator_field_click_bg");
        }
    }

    private void OnClickSuccess(ClickEvent evt)
    {
        string value = ToolTipsUtils.ConvertPriceToNormal(_userInput.value);
        _dealerWindow.RefreshOpacity(1);
        _dealerWindow.Move—ellElseQuantitySelected(_type,  _listServer, _position , int.Parse(value));
        base.HideWindow();
    }

    private void OnClickClose(ClickEvent evt)
    {
        _dealerWindow.RefreshOpacity(1);
        base.HideWindow();
    }

    private void OnValueChanged(ChangeEvent<string> evt)
    {

        _userInput.UnregisterValueChangedCallback(OnValueChanged);

        if (evt.newValue.Length == 0 & evt.previousValue.Length >= 1)
        {
            _userInput.SetValueWithoutNotify("0");
            _userInput.RegisterValueChangedCallback(OnValueChanged);
            return;
        }
        

        string digitsOnly = ToolTipsUtils.ConvertPriceToNormal(evt.newValue);
        if (digitsOnly.Length > 20) return;
        if (string.IsNullOrEmpty(digitsOnly) || !long.TryParse(digitsOnly, out long newValue))
        {
            _userInput.SetValueWithoutNotify(evt.previousValue);
            _userInput.RegisterValueChangedCallback(OnValueChanged);
            return;
        }

        digitsOnly = digitsOnly.Trim();

        string formattedValue = ToolTipsUtils.ConvertToPrice(long.Parse(digitsOnly));
        
        _userInput.SetValueWithoutNotify(formattedValue);

        if (formattedValue.Length > 1) SetCursorToEnd(formattedValue);
        _userInput.RegisterValueChangedCallback(OnValueChanged);
    }

    private void SetCursorToEnd(string formattedValue)
    {
        _userInput.cursorIndex = formattedValue.Length + 1;
    }

    public void ShowWindow(DealerWindow dealerWindow , Product selectProduct , SlotType type, List<Product> listServer, int position)
    {
        _type = type;
        _listServer = listServer;
        _position = position;
        _selectProduct = selectProduct;
        _dealerWindow = dealerWindow;
        base.ShowWindow();
        OnCenterScreen(_root);
        _userInput.value = "0";
    }




    private void OnDestroy()
    {
        _instance = null;
    }

}
