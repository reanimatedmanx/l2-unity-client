using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class ClickManager : MonoBehaviour
{
    [SerializeField] private GameObject _locator;
    [SerializeField] private ObjectData _targetObjectData;
    [SerializeField] private ObjectData _hoverObjectData;

    public ObjectData HoverObjectData { get { return _hoverObjectData; } }

    private Vector3 _lastClickPosition = Vector3.zero;
    [SerializeField] private LayerMask _entityMask;
    [SerializeField] private LayerMask _clickThroughMask;
    private LayerMask _areaMask = 8192;

    private static ClickManager _instance;
    public static ClickManager Instance { get { return _instance; } }

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

    void OnDestroy()
    {
        _instance = null;
    }

    void Start()
    {
        _locator = GameObject.Find("Locator");
        HideLocator();
    }

    public void SetMasks(LayerMask entityMask, LayerMask clickThroughMask)
    {
        _entityMask = entityMask;
        _clickThroughMask = clickThroughMask;
    }

    void Update()
    {
        if (L2GameUI.Instance.MouseOverUI)
        {
            return;
        }

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 1000f, ~_clickThroughMask))
        {
            int hitLayer = hit.collider.gameObject.layer;
            if (_entityMask == (_entityMask | (1 << hitLayer)))
            {
                _hoverObjectData = new ObjectData(hit.transform.parent.gameObject);
            }
            else
            {
                _hoverObjectData = new ObjectData(hit.collider.gameObject);
            }

            if (InputManager.Instance.LeftClickDown &&
                !InputManager.Instance.RightClickHeld)
            {
                _targetObjectData = _hoverObjectData;

                if (_entityMask == (_entityMask | (1 << hitLayer)) && _targetObjectData.ObjectTag != "Player")
                {
                    OnClickOnEntity();
                }
                else if (_targetObjectData != null)
                {
                    OnClickToMove(hit);
                }
            }
        }
    }

   
    public void OnClickToMove(RaycastHit hit)
    {
        _lastClickPosition = hit.point;
        //  PlayerCombatController.Instance.RunningToTarget = false;

        StopFollow();
        SendPacketMoveToLocation(_lastClickPosition);
        //PlayerStateMachine.Instance.ChangeIntention(Intention.INTENTION_MOVE_TO, _lastClickPosition);

        TargetManager.Instance.ClearAttackTarget();
        //  PathFinderController.Instance.MoveTo(_lastClickPosition);
        float angle = Vector3.Angle(hit.normal, Vector3.up);
        if (angle < 85f)
        {
            PlaceLocator(_lastClickPosition);
        }
        else
        {
            HideLocator();
        }
    }

    private void StopFollow()
    {
        PlayerStateMachine.Instance.Follow = null;
        PlayerStateMachine.Instance.IsMoveToPawn = false;
    }

    private void SendPacketMoveToLocation(Vector3 _lastClickPosition)
    {
        MoveBackwardToLocation sendPaket = CreatorPacketsUser.CreateMoveToLocation(PlayerEntity.Instance.transform.position, _lastClickPosition);
        bool enable = GameClient.Instance.IsCryptEnabled();
        SendGameDataQueue.Instance().AddItem(sendPaket, enable, enable);
    }

    public void OnClickOnEntity()
    {
        Debug.Log("Hit entity");
        TargetData _target = new TargetData(_targetObjectData);
        var l2jpos = _target.Identity.GetL2jPos();
        ClickAction sendPaket = CreatorPacketsUser.CreateActiont(_target.Identity.Id , (int)l2jpos.x , (int)l2jpos.y , (int)l2jpos.z , 0);
        bool enable = GameClient.Instance.IsCryptEnabled();

        SendGameDataQueue.Instance().AddItem(sendPaket, enable, enable);
    }



    public void PlaceLocator(Vector3 position)
    {
        _locator.SetActive(true);
        _locator.gameObject.transform.position = position;
    }

    public void HideLocator()
    {
        _locator.SetActive(false);
    }
}
