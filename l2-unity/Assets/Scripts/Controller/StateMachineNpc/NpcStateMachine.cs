using System;
using UnityEngine;

public class NpcStateMachine : MonoBehaviour
{

    [SerializeField] private bool _enableLogs;
    [SerializeField] private NpcState _currentState;
    [SerializeField] private NpcIntention _currentIntention;
 
    public NpcIntention Intention { get { return _currentIntention; } }
    public Transform Follow { get; set; }
    public NpcState State { get { return _currentState; } }
    private NpcBase _stateInstance;
    private NpcIntentionBase _intentionInstance;
    private int _npcObjectId;
    private int _npcId;
    private GameObject _go;
    private MoveNpc _moveNpc;
    private PlayerEntity _target;
    private Entity _entity;

    private GravityNpc _gravityNpc;

    private void Start()
    {
        //_waitingForServerReply = false;
        this.enabled = false;

        InitializeState();
        InitializeIntention();
        ChangeState(NpcState.IDLE);
        _gravityNpc.Sync();
    }

    public PlayerEntity GetTarget() { return _target; }

    public void SetTarget(PlayerEntity target)
    {
        _target = target;
    }
    public GameObject NpcObject { get { return _go; } }

    public MoveNpc MoveNpc { get { return _moveNpc; } }

    public Entity Entity { get { return _entity; } }

    public virtual void Initialize(int npcObjectId,
        int npcId,
        GameObject go,
        MoveNpc moveNpc,
        Entity entity,
        GravityNpc gravityNpc)
    {
        _npcObjectId = npcObjectId;
        _npcId = npcId;
        _go = go;
        _moveNpc = moveNpc;
        _entity = entity;
        _gravityNpc = gravityNpc;
    }

    public void ChangeState(NpcState newState)
    {
        if (_enableLogs) Debug.Log("[NPC :StateMachine][STATE] " + newState);
        _stateInstance?.Exit();
        _currentState = newState;
        InitializeState();
        _stateInstance?.Enter();
    }

    public void ChangeIntention(NpcIntention intention)
    {
        ChangeIntention(intention, null);
    }


    // Update is called once per frame
    private void Update()
    {
        _stateInstance?.Update();
        _intentionInstance?.Update();
    }


    public void ChangeIntention(NpcIntention newIntention, object arg0)
    {
        if (_enableLogs) Debug.Log("[NPC  :StateMachine][INTENTION] " + newIntention);
        _intentionInstance?.Exit();
        _currentIntention = newIntention;
        InitializeIntention();
        _intentionInstance?.Enter(arg0);
    }


    private void InitializeState()
    {
        _stateInstance = _currentState switch
        {
            NpcState.IDLE => new IdleNpcState(this),
            NpcState.RUNNING => new RunningNpcState(this),
            NpcState.WALKING => new WalkingNpcState(this),
            _ => throw new ArgumentException("Invalid state")
        };
    }


    private void InitializeIntention()
    {
        _intentionInstance = _currentIntention switch
        {

            NpcIntention.INTENTION_IDLE => new IdleNpcIntention(this),
            NpcIntention.INTENTION_MOVE_TO => new MoveToNpcIntention(this),
            _ => throw new ArgumentException("Invalid intention")
        };
    }


    public void NotifyEvent(Event evt)
    {
        if (_enableLogs) Debug.Log("[Monster: StateMachine][EVENT] " + evt);

        //if (_stateInstance.GetType() == typeof(AttackinMonsterState))
       // {
         //   var state = (AttackinMonsterState)_stateInstance;
         //   state.HandleEvent(evt);
       // }
       // else
        //{
            _stateInstance?.HandleEvent(evt);
       // }

    }

    public override string ToString()
    {
        return "NPC ObjID: " + _npcObjectId + "  You Npc Id: " + _npcId;
    }

}
