using UnityEngine;
using UnityEngine.UIElements;

public abstract class L2PopupWindow : L2Window
{
    protected void RegisterCloseWindowEvent(string closeButtonClass)
    {
        Button closeButton = (Button)GetElementByClass(closeButtonClass);
        if (closeButton == null)
        {
            Debug.LogWarning($"Cant find close button with className: {closeButtonClass}.");
            return;
        }

        ButtonClickSoundManipulator buttonClickSoundManipulator = new ButtonClickSoundManipulator(closeButton);
        closeButton.AddManipulator(buttonClickSoundManipulator);

        closeButton.RegisterCallback<MouseUpEvent>(evt =>
        {
            AudioManager.Instance.PlayUISound("window_close");
            HideWindow();
        });
    }

    protected void RegisterCloseWindowEventByName(string closeButtonName)
    {
        Button closeButton = (Button)GetElementById(closeButtonName);
        if (closeButton == null)
        {
            Debug.LogWarning($"Cant find close button with className: {closeButtonName}.");
            return;
        }

        ButtonClickSoundManipulator buttonClickSoundManipulator = new ButtonClickSoundManipulator(closeButton);
        closeButton.AddManipulator(buttonClickSoundManipulator);

        closeButton.RegisterCallback<MouseUpEvent>(evt =>
        {
            AudioManager.Instance.PlayUISound("window_close");
            HideWindow();
        });
    }

    public void RegisterClickWindowEvent(VisualElement windowEle, VisualElement dragEle)
    {
        if (windowEle != null)
        {
            windowEle.RegisterCallback<MouseDownEvent>(evt =>
            {
                BringToFront();
            }, TrickleDown.TrickleDown);
        }

        if (dragEle != null)
        {
            dragEle.RegisterCallback<MouseDownEvent>(evt =>
            {
                BringToFront();
            }, TrickleDown.TrickleDown);
        }
    }

    public override void ShowWindow()
    {
        base.ShowWindow();
        BringToFront();
    }

    public override void HideWindow()
    {
        base.HideWindow();
    }

    public override void OnlyHideWindow()
    {
        base.OnlyHideWindow();
    }
    public override void BringToFront()
    {
        _windowEle.BringToFront();
    }

    public override void SendToBack()
    {
        _windowEle.SendToBack();
    }


    public void ShowWindowToCenter()
    {
        base.ShowWindow();
        OnCenterScreen(_root);
    }

    public void RefreshOpacity(float opacity)
    {
        _windowEle.style.opacity = opacity;
    }


}
