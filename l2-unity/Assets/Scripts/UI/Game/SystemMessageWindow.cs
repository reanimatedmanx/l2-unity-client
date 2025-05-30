using System;
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
    private Button _ñancelButton;
    private Button _okButton;

    public event Action OnButtonOk;
    public event Action OnButtonClosed;

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

        _ñancelButton = _windowEle.Q<Button>("CancelButton");
        _okButton = _windowEle.Q<Button>("OkButton");
        _textLabel = _windowEle.Q<Label>("labelText");
        //RegisterCloseWindowEventByName("OkButton");
        _ñancelButton.RegisterCallback<ClickEvent>(ClickEventClosed);
        _okButton.RegisterCallback<ClickEvent>(ClickEventOk);
        RegisterClickWindowEvent(_windowEle, null);
        OnCenterScreen(_root);
    }


    private void ClickEventOk(ClickEvent evt)
    {
        OnButtonOk?.Invoke(); // Âûçûâàåì ñîáûòèå
        base.HideWindow();
    }

    private void ClickEventClosed(ClickEvent evt)
    {
        OnButtonClosed?.Invoke(); 
        base.HideWindow();
    }


    public void ShowWindow(string messageText)
    {
        OnCenterScreen(_root);
        _ñancelButton.style.display = DisplayStyle.None;
        _okButton.style.display = DisplayStyle.Flex;
        _textLabel.text = messageText;
        base.ShowWindow();
    }

    public void ShowWindowDialogYesOrNot(string messageText)
    {
        OnCenterScreen(_root);
        _ñancelButton.style.display = DisplayStyle.Flex;
        _okButton.style.display = DisplayStyle.Flex;
        _textLabel.text = messageText;
        base.ShowWindow();
    }

    private void OnDestroy()
    {
        _instance = null;
    }

}
