using System;
using System.Collections;
using System.Collections.Generic;

using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

public class HtmlWindow : L2PopupWindow
{
   

    private static HtmlWindow _instance;
    private VisualElement _content;
    private Dictionary<string, string> _actionsHtml = new Dictionary<string, string>();
    public static HtmlWindow Instance
    {
        get { return _instance; }
    }
    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            _actionsHtml = new Dictionary<string, string>();
        }
        else
        {
            Destroy(this);
        }
    }

    private void OnDestroy()
    {
        _instance = null;
    }

    protected override void LoadAssets()
    {
        _windowTemplate = LoadAsset("Data/UI/_Elements/Game/HtmlWindow");
    }

    protected override void InitWindow(VisualElement root)
    {
        base.InitWindow(root);

        var dragArea = GetElementByClass("drag-area");
        _content = GetElementByClass("content");
        DragManipulator drag = new DragManipulator(dragArea, _windowEle);
        dragArea.AddManipulator(drag);

        RegisterCloseWindowEvent("btn-close-frame");
        RegisterClickWindowEvent(_windowEle, dragArea);
        OnCenterScreen(root);
    }
    private TextElement _latTextElement;
    private TextElement _fontColorFirst;
    public void InjectToWindow(List<IElementsUI> elements)
    {
        _actionsHtml.Clear();
        _content.Clear();

        foreach (IElementsUI element in elements)
        {
            if(element.GetType() == typeof(ParseLabel))
            {

                ParseLabel parce = (ParseLabel)element;
                TextElement label = new TextElement();
                
                //Else last element font color
                if (_fontColorFirst != null)
                {
                    //#DCD9DC - normal color hex color
                    _fontColorFirst.text = _fontColorFirst.text + "<color=#DCD9DC>" + parce.Text() + "</color>";
                }
                else
                {
                    label.text = "<line-height=130%>" + parce.Text();
                    label.AddToClassList("html_normal_text");
                    _content.Add(label);
                    _latTextElement = label;
                }

            }
            else if(element.GetType() == typeof(ParseBr))
            {
                Label label = new Label("\n\n");
                
                label.AddToClassList("html_br");
                _content.Add(label);
                //no concat font color
                _latTextElement = null;
                _fontColorFirst = null;
            }
            else if (element.GetType() == typeof(ParseHref))
            {
                ParseHref parce = (ParseHref)element;
                Label label = new Label("<u>"+parce.Name+ "</u>");
                label.name = parce.Name;
                label.AddToClassList("html_link");
                _content.Add(label);
                _actionsHtml.Add(parce.Name, parce.Action);

                label.RegisterCallback<ClickEvent>(evt => OnLabelClick(label));
            }else if (element.GetType() == typeof(ParseFontColor))
            {
                ParseFontColor parce = (ParseFontColor)element;

                if(_latTextElement != null)
                {
                    ConcatText(parce);
                }
                else
                {
                    AddNewFontColot(parce);
                }

            }
        }
    }

   

    private void AddNewFontColot(ParseFontColor parce)
    {
        TextElement label1 = new TextElement();
        label1.text = "<line-height=130%>" + parce.Text;
        label1.AddToClassList("html_normal_text");
        Color color = parce.GetColor();
        label1.style.color = color;
        _latTextElement = label1;
        _content.Add(label1);
        _fontColorFirst = label1;
    }

    private void ConcatText(ParseFontColor parce)
    {
        _latTextElement.text = _latTextElement.text + "<color=#"+parce.ToHex()+">" + parce.Text + "</color>";
        _fontColorFirst = _latTextElement;
    }

    private void OnLabelClick(Label clickedLabel)
    {
        string labelName = clickedLabel.name;
        string action = _actionsHtml[labelName];
        if (!string.IsNullOrEmpty(action))
        {
            HideWindow();
            SendSenver(action);
        }
        
       // Debug.Log($"Вы нажали на Label с именем: {labelName}  , {action}");
    }

    public async Task SendSenver(string command )
    {
        RequestBypassToServer sendPaket = CreatorPacketsUser.CreateByPassPacket(command);
        bool enable = GameClient.Instance.IsCryptEnabled();
        SendGameDataQueue.Instance().AddItem(sendPaket, enable, enable);
    }
    protected override IEnumerator BuildWindow(VisualElement root)
    {
        InitWindow(root);

        yield return new WaitForEndOfFrame();

        var dragArea = GetElementByClass("drag-area");
      
    }

    public void Test2()
    {
        StartCoroutine(CoroutineShow());
    }
    private IEnumerator CoroutineShow()
    {
        yield return new WaitForEndOfFrame();
        _windowEle.style.display = DisplayStyle.Flex;
        _mouseOverDetection.Enable();
        BringToFront();
    }


}
