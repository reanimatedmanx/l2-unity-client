using System;
using System.Collections;
using UnityEditor.PackageManager.UI;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using static UnityEditor.Rendering.FilterWindow;

public abstract class L2Window : MonoBehaviour
{
    protected VisualElement _root;
    protected VisualTreeAsset _windowTemplate;
    protected VisualElement _windowEle;
    protected bool _isWindowHidden = false;
    protected MouseOverDetectionManipulator _mouseOverDetection;

    void Start()
    {
        _isWindowHidden = false;
        LoadAssets();
    }

    protected abstract void LoadAssets();

    protected VisualTreeAsset LoadAsset(string assetPath)
    {
        VisualTreeAsset asset = Resources.Load<VisualTreeAsset>(assetPath);
        if (asset == null)
        {
            Debug.LogError($"Could not load {assetPath} template.");
        }

        return asset;
    }

    private bool _isCenter = false;
    private bool _isReg = false;
    public void OnCenterScreen(VisualElement root)
    {
        _isCenter = false;
        UpdateCenter(root);
    }



    private void UpdateCenter(VisualElement root)
    {
        if (_isReg == false)
        {
            _isReg = true;
            _windowEle.RegisterCallback<GeometryChangedEvent>(evt =>
            {
                if (_isCenter) return;

                _isCenter = true;

                float width = _windowEle.resolvedStyle.width;
                float height = _windowEle.resolvedStyle.height;

                float parentWidth = root.resolvedStyle.width;
                float parentHeight = root.resolvedStyle.height;

                _windowEle.style.left = (parentWidth - width) / 2;
                _windowEle.style.top = (parentHeight - height) / 2;

            });
        }
    
    }




    public void AddWindow(VisualElement root)
    {
        if (_windowTemplate == null)
        {
            return;
        }
        StartCoroutine(BuildWindow(root));
    }

    protected virtual void InitWindow(VisualElement root)
    {
        try
        {
            _root = root;
            _windowEle = _windowTemplate.Instantiate()[0];
            _mouseOverDetection = new MouseOverDetectionManipulator(_windowEle);
            _windowEle.AddManipulator(_mouseOverDetection);
            DisableEventOnOver(_windowEle);
            if (_isWindowHidden)
            {
                _mouseOverDetection.Disable();
            }

            root.Add(_windowEle);
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
        }
 
    }


    protected void DisableEventOnOver(VisualElement _windowEle)
    {
        var dropdowns = _windowEle.Query<DropdownField>().ToList();

        if(dropdowns != null)
        {
            foreach (var dropdown in dropdowns)
            {
                DisablePointerEvents(dropdown);
                dropdown.RegisterValueChangedCallback(OnDropdownValueChanged);
                dropdown.RegisterCallback<MouseEnterEvent>(OnMouseEnter);
            }
        }
    
    }
    private void OnDropdownValueChanged(ChangeEvent<string> evt)
    {
        SetEnableMouseOver(false);
    }
    private void OnMouseEnter(MouseEnterEvent evt)
    {
        SetEnableMouseOver(true);
    }

    private void DisablePointerEvents(VisualElement element)
    {
        element.RegisterCallback<PointerOverEvent>(evt => evt.StopPropagation());
        element.RegisterCallback<PointerOutEvent>(evt => evt.StopPropagation());
    }



    protected abstract IEnumerator BuildWindow(VisualElement root);

    public virtual void HideWindow()
    {
        try
        {
            _isWindowHidden = true;
            _windowEle.style.display = DisplayStyle.None;
            _mouseOverDetection.Disable();
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
        }

    }

    public virtual void OnlyHideWindow()
    {
        _windowEle.style.display = DisplayStyle.None;
    }

    public virtual void ShowWindow()
    {
        try
        {
            _isWindowHidden = false;
            _windowEle.style.display = DisplayStyle.Flex;
            _mouseOverDetection.Enable();
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
        }

    }

    public virtual void ToggleHideWindow()
    {
        if (_isWindowHidden)
        {
            ShowWindow();
        }
        else
        {
            HideWindow();
        }
    }

    protected Label GetLabelById(string id)
    {
        VisualElement e = GetElementById(id);
        if (e == null)
        {
            return null;
        }

        return (Label)e;
    }

    protected VisualElement GetElementById(string id)
    {
        var btn = _windowEle.Q<VisualElement>(id);
        if (btn == null)
        {
            Debug.LogError(id + " can't be found.");
            return null;
        }

        return btn;
    }

    public VisualElement GetElementByClass(string className)
    {
        var btn = _windowEle.Q<VisualElement>(null, className);
        if (btn == null)
        {
            Debug.LogError(className + " can't be found.");
            return null;
        }

        return btn;
    }


    public virtual void BringToFront()
    {
    }

    public virtual void SendToBack()
    {
    }

    public bool MouseOverThisWindow()
    {
        return _mouseOverDetection.MouseOver;
    }

    protected void SetEnableMouseOver(bool isBlock)
    {
        _mouseOverDetection.SetBlock(isBlock);
    }
}
