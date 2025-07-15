using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ShowListWindow : L2PopupWindow
{
    private static ShowListWindow _instance;
    public static ShowListWindow Instance { get { return _instance; } }
    private Button _сancelButton;
    private Button _okButton;

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
    }

    protected override IEnumerator BuildWindow(VisualElement root)
    {
        InitWindow(root);
        yield return new WaitForEndOfFrame();

        _сancelButton = _windowEle.Q<Button>("CancelButton");
        _okButton = _windowEle.Q<Button>("OkButton");
        var conent = _windowEle.Q<VisualElement>("content");

        DropdownField dropdown = conent.Q<DropdownField>("comboBox");



        dropdown.choices = new List<string>
        {
            "Option 1",
            "Option 2",
            "Option 3",
            "Option 4"
        };

        // Установим текущее значение (по желанию)
        dropdown.value = "Option 1"; // Устанавливаем значение по умолчанию


        RegisterClickWindowEvent(_windowEle, null);
        OnCenterScreen(_root);
    }

    private void OnDestroy()
    {
        _instance = null;
    }

}
