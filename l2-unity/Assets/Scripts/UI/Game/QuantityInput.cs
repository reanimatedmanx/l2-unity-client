using System.Collections;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Windows;

public class QuantityInput : L2Window
{
    private static QuantityInput _instance;
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

        TextField _userInput = (TextField)GetElementById("UserInputField");
        VisualElement _inputBg = (VisualElement)GetElementById("UserInputBg");
        _userInput.RegisterCallback<ClickEvent>((evt) => OnClick(evt , _inputBg));

        yield return new WaitForEndOfFrame();

        
    }

    private void OnClick(ClickEvent evt ,  VisualElement inputBg)
    {
        Debug.Log("Click Input files !!!");
        if (!inputBg.ClassListContains("calculator_field_click_bg"))
        {
            inputBg.AddToClassList("calculator_field_click_bg");
        }
    }












    private void OnDestroy()
    {
        _instance = null;
    }

}
