using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEditor.Rendering.FilterWindow;

public class BufferPanel : L2Window
{

    private static BufferPanel _instance;
    private VisualTreeAsset _barSlotTemplate;
    private float _defaultLeftOffset = 200;
    private Dictionary<int, DataCell> _dictElement;
    public static BufferPanel Instance { get { return _instance; } }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            _dictElement = new Dictionary<int, DataCell>();
        }
        else
        {
            Destroy(this);
        }
    }

    protected override void LoadAssets()
    {
        _barSlotTemplate = LoadAsset("Data/UI/_Elements/Template/SlotBuffer");
        _windowTemplate = LoadAsset("Data/UI/_Elements/Game/Buffer/SkillTray");
    }

    protected override IEnumerator BuildWindow(VisualElement root)
    {
        InitWindow(root);

        var dragArea = GetElementByClass("drag-area");
        DragManipulator drag = new DragManipulator(dragArea, _windowEle);
        dragArea.AddManipulator(drag);
        CreateItems(_windowEle);
        yield return new WaitForEndOfFrame();
        MovePanelToChatPosition(_defaultLeftOffset);
    }


    public void AddDataPanel(int  skillId , int skillLevel)
    {
        int key = GetEmptyCell();
        DataCell data = _dictElement[key];
        data.SetBusy(true);
        data.SetIcon(skillId, skillLevel);
    }
    private void CreateItems(VisualElement root)
    {
        for(int i = 1; i < 13; i++)
        {
            VisualElement cell = GetElementById("SlotBuffer"+i);
            if (cell != null)
            {
                CreateCallBack(cell);
                SetTestData(cell, i);
            }

            _dictElement.Add(i, new DataCell(-1, cell));
        }
    }

    public int GetEmptyCell()
    {
        return _dictElement
            .FirstOrDefault(kvp => !kvp.Value.IsBusy())
            .Key;
    }

    private void CreateCallBack(VisualElement cell)
    {
        cell.RegisterCallback<MouseEnterEvent>(OnMouseEnter);
        cell.RegisterCallback<MouseLeaveEvent>(OnMouseLeave);
    }

    private void SetTestData(VisualElement cell , int i)
    {
        cell.userData = (int)i + 999;
    }

    private void OnMouseEnter(MouseEnterEvent evt)
    {
        Debug.Log("Мышь вошла в элемент!");
        // Получаем элемент под курсором мыши
        VisualElement hoveredElement = evt.target as VisualElement;

        if (hoveredElement != null)
        {
            int data = (int)hoveredElement.userData;
            Debug.Log("user Data");
            AddDataPanel(9, 1);
        }
    }

    private void OnMouseLeave(MouseLeaveEvent evt)
    {
        Debug.Log("Мышь покинула элемент!");
    }

    public void MovePanelToChatPosition(float sourceY)
    {
        _windowEle.style.left = sourceY;

    }

    private void OnDestroy()
    {
        _instance = null;
    }

}
