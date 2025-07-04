using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEditor.ShaderGraph.Internal.KeywordDependentCollection;

public class SkillLearnWindow : L2PopupWindow
{
    private static SkillLearnWindow _instance;
    protected VisualTreeAsset _rowsTemplate;
    private ICreator _creatorWindow;
    private Label _spLabel;
    public static SkillLearnWindow Instance { get { return _instance; } }
    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            _creatorWindow = new CreatorVerticalScrollWindows();
        }
        else
        {
            Destroy(this);
        }
    }

    protected override void LoadAssets()
    {
        _windowTemplate = LoadAsset("Data/UI/_Elements/Game/SkillLearn/Learn/SkillLearnWindow");
        _rowsTemplate = LoadAsset("Data/UI/_Elements/Template/Skills/ItemSkillRow");
    }

    protected override IEnumerator BuildWindow(VisualElement root)
    {
        InitWindow(root);
        yield return new WaitForEndOfFrame();

        var dragArea = GetElementByClass("drag-area");
        DragManipulator drag = new DragManipulator(dragArea, _windowEle);

        var content = (VisualElement)GetElementById("content");
        _spLabel = (Label)GetElementById("spCount");


        _creatorWindow.InitTabs(new string[] { "ALL", "Other" });
        _creatorWindow.CreateTabs(content, null, _rowsTemplate);
        
        RegisterCloseWindowEvent("btn-close-frame");
        RegisterClickWindowEvent(_windowEle, dragArea);
        OnCenterScreen(_root);

    }

    public void AddData(List<OtherModel> list)
    {
        _creatorWindow.AddOtherData(list);
    }


    public void ShowWindow()
    {
        UserInfo user = StorageNpc.getInstance().GetFirstUser();
        _spLabel.text = user.PlayerInfoInterlude.Stats.Sp.ToString();
        base.ShowWindow();
    }
}
