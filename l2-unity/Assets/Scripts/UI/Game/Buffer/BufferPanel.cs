using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class BufferPanel : L2Window
{

    private static BufferPanel _instance;
    private VisualTreeAsset _barSlotTemplate;
    private float _defaultLeftOffset = 200;
    private Dictionary<int, DataCell> _dictElement;
    private int[] _deathPenalty;
    private int[] _weightPenalty;
    public static BufferPanel Instance { get { return _instance; } }
    public BLink _bLink;
    public VisualElement _content;
    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            _dictElement = new Dictionary<int, DataCell>();
            _bLink = new BLink();
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
        _content = GetElementById("content");
        
        DragManipulator drag = new DragManipulator(dragArea, _windowEle);
        dragArea.AddManipulator(drag);
        CreateItems();

        yield return new WaitForEndOfFrame();

        _content.style.opacity = 0;
        MovePanelToChatPosition(_defaultLeftOffset);
    }


   


    public void RefreshPenalty(EtcStatusUpdate etcStatusUpdatePacket)
    {
        _deathPenalty = etcStatusUpdatePacket.DeathPenalty;
        _weightPenalty = etcStatusUpdatePacket.WeightPenalty;

        AddDataCell(_deathPenalty[0], _deathPenalty[1]);
        AddDataCell(_weightPenalty[0], _weightPenalty[1]);

        DeleteDataCell(_deathPenalty[0], _deathPenalty[1]);
        DeleteDataCell(_weightPenalty[0], _weightPenalty[1]);
    }

    public void DeleteDataCell(int skillId, int skillLevel)
    {
        if(skillLevel == 0)
        {
            int key = FilterBySkillId(skillId);

            if(key != 0)
            {
                DataCell data = _dictElement[key];
                data.RefreshData(-1, false, 0);
                data.ShowCell(false);
            }
        }
    }
    public void AddDataCell(int  skillId , int skillLevel)
    {
        if (skillLevel == 0) return;
        if(_content.style.opacity == 0) _content.style.opacity = 1;

        int key = GetEmptyCell();
        DataCell data = _dictElement[key];
        data.RefreshData(skillId, true, skillLevel);
        data.ShowCell(true);

        //RemoveElementAfterDelay();
    }

    public void AddDataCellToTime(int skillId, int skillLevel , int time)
    {
        if(SkillgrpTable.Instance.GetSkill(skillId, skillLevel) != null)
        {
            if (skillLevel == 0) return;
            if (_content.style.opacity == 0) _content.style.opacity = 1;

            int key = GetEmptyCell();
            DataCell data = _dictElement[key];
            data.RefreshData(skillId, true, skillLevel);
            data.ShowCell(true);
        }
        else
        {
            Debug.LogWarning("Buffpanel: Не критическая ошибка, мы не нашли skill id при попытке обновить herb effect!!!! skillID " + skillId + " level " + skillLevel);
        }


        //RemoveElementAfterDelay();
    }


    private void CreateItems()
    {
        for(int i = 1; i < 13; i++)
        {
            VisualElement cell = GetElementById("SlotBuffer"+i);
            if (cell != null)
            {
                //CreateCallBack(cell);
                SetTestData(cell, i);
            }

            cell.style.opacity = 0;
            _dictElement.Add(i, new DataCell(-1, cell));

        }
    }

    public int GetEmptyCell()
    {
        return _dictElement
            .FirstOrDefault(kvp => !kvp.Value.IsBusy())
            .Key;
    }

    public int GetTestCell()
    {
        return _dictElement
            .FirstOrDefault(kvp => kvp.Value.IsBusy())
            .Key;
    }

    public int FilterBySkillId(int skillId)
    {
        return _dictElement
            .FirstOrDefault(kvp => kvp.Value.GetSkillId() == skillId)
            .Key;
    }

    public void RemoveElementAfterDelay()
    {
        if (_bLink != null)
        {
            int element = GetTestCell();

            if (element != 0)
            {
                DataCell cell = _dictElement[element];
                _bLink.StartBlinking(this, cell.GetElement(), 0.5f);
            }

        }
    }

    //private void CreateCallBack(VisualElement cell)
    /////{
    //cell.RegisterCallback<MouseEnterEvent>(OnMouseEnter);
    //cell.RegisterCallback<MouseLeaveEvent>(OnMouseLeave);
    //}

    private void SetTestData(VisualElement cell , int i)
    {
        cell.userData = (int)i + 999;
    }

   // private void OnMouseEnter(MouseEnterEvent evt)
   // {
       // Debug.Log("Мышь вошла в элемент!");
        // Получаем элемент под курсором мыши
        //VisualElement hoveredElement = evt.target as VisualElement;

        //if (hoveredElement != null)
        //{
         //   int data = (int)hoveredElement.userData;
         //   Debug.Log("user Data");
        //    AddDataPanel(9, 1);
       // }
   // }


    public void MovePanelToChatPosition(float sourceY)
    {
        _windowEle.style.top = 3;
        _windowEle.style.left = sourceY;

    }

    private void OnDestroy()
    {
        _instance = null;
    }

}
