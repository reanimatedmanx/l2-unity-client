using NUnit.Framework;
using System.Collections.Generic;
using System.Threading;
using UnityEditorInternal;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;
using static UnityEngine.Rendering.DebugUI;
using static UnityEngine.UI.CanvasScaler;

public class PositionValidationController : MonoBehaviour
{
    //<197 unit - он передвигается пешком(игнорирует если он двигается)
    //>197 unit он прыгает останавливая движение и возвращая его когда будет перемещен

    private List<ValidateLocation> _validateList;
    private List<ValidateLocation> _validateRemove;
    //197 unit | 3.743f metr
    private float _trigger = 3.743f;

    private static PositionValidationController _instance;
    public static PositionValidationController Instance { get { return _instance; } }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            _validateList = new List<ValidateLocation>();
            _validateRemove = new List<ValidateLocation>();
        }
        else
        {
            Destroy(this);
        }
    }

    
    void Update()
    {
        if (_validateList.Count == 0) return;

        for (int i = 0; i < _validateList.Count; i++)
        {
            ValidateLocation validateLocation = _validateList[i];

            if (validateLocation != null)
            {
                Entity entity = World.Instance.GetEntityNoLockSync(validateLocation.ObjectId);

                if(entity != null & !entity.IsDead())
                {
                    Vector3 activePosition = entity.transform.position;
                    Vector3 newPosition = validateLocation.Position;
                    float distance = VectorUtils.Distance2D(activePosition, newPosition);


                    if (distance > 0.15f && distance < _trigger)
                    {
                        StartWalk(entity, newPosition);
                        _validateRemove.Add(validateLocation);
                    }
                    else if (distance > _trigger)
                    {
                        Jump(entity, newPosition);
                        _validateRemove.Add(validateLocation);
                    }

                }
                else
                {
                    _validateRemove.Add(validateLocation);
                }
  

            }
        }

        _validateList.RemoveAll(n => _validateRemove.Contains(n));

        _validateRemove.Clear();
    }

    private void Jump(Entity entity, Vector3 newPosition)
    {
        if (entity.GetType() == typeof(MonsterEntity))
        {
            MonsterEntity monsterEntity = (MonsterEntity)entity;
            monsterEntity.HideObject();
            //set gravity
            monsterEntity.transform.position = new Vector3(newPosition.x , monsterEntity.transform.position.y , newPosition.z);
            monsterEntity.ShowObject();
            var stateMachine = monsterEntity.GetStateMachine();
            ReStartAnimation(stateMachine);
        }
    }


    private void ReStartAnimation(MonsterStateMachine stateMachine)
    {
        if (stateMachine.MoveMonster.IsMoving())
        {
            if (stateMachine.State == MonsterState.WALKING) stateMachine.ChangeState(MonsterState.WALKING);
            if (stateMachine.State == MonsterState.RUNNING) stateMachine.ChangeState(MonsterState.RUNNING);
            stateMachine.NotifyEvent(Event.MOVE_TO);
        }
    }
    private void StartWalk(Entity entity , Vector3 newPosition)
    {
        if (entity.GetType() == typeof(MonsterEntity))
        {
            MonsterEntity monsterEntity = (MonsterEntity)entity;
            var stateMachine = monsterEntity.GetStateMachine();
            if (stateMachine == null || (stateMachine.State == MonsterState.WALKING || stateMachine.State == MonsterState.RUNNING)) return;
            stateMachine.ChangeIntention(MonsterIntention.INTENTION_MOVE_TO , newPosition);
        }
    }
    public void AddValidateLocation(ValidateLocation validateLocation)
    {
        if (!_validateList.Contains(validateLocation))
        {
            _validateList.Add(validateLocation);
        }
    }

    void OnDestroy()
    {
        _instance = null;
    }
}
