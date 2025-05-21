using System.Collections;
using System.Reflection.Emit;
using UnityEngine;
using UnityEngine.UIElements;
using Label = UnityEngine.UIElements.Label;

public class SystemMessageWindow : L2PopupWindow
{
    private static SystemMessageWindow _instance;
    public static SystemMessageWindow Instance { get { return _instance; } }
    private Label _textLabel;

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
        _windowTemplate = LoadAsset("Data/UI/_Elements/Game/SystemMessageWindow");
    }


    protected override IEnumerator BuildWindow(VisualElement root)
    {
        InitWindow(root);
        yield return new WaitForEndOfFrame();

        //Button okButton = _windowEle.Q<Button>("OkButton");
        _textLabel = _windowEle.Q<Label>("labelText");
        RegisterCloseWindowEventByName("OkButton");
        RegisterClickWindowEvent(_windowEle, null);
        OnCenterScreen(_root);
    }


    public void ShowWindow(string messageText)
    {
        OnCenterScreen(_root);
        _textLabel.text = messageText;
        base.ShowWindow();
    }

    private void OnDestroy()
    {
        _instance = null;
    }

}
