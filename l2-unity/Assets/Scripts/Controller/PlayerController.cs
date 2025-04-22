
using System;
using System.Runtime.ConstrainedExecution;
using System.Security.Cryptography;
using System.Threading;
using System.Timers;
using TMPro;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.EventSystems.EventTrigger;



public class PlayerController : MonoBehaviour
{
    /* Components */
    private CharacterController _controller;
    /*Rotate*/
    private float _finalAngle;

    /* Movement */
     private Vector3 _moveDirection;
     private bool _running = true;

    private Vector3 _currentPos;
    private Vector3 _lastPos;
    private Vector2 _axis;
    //private 
    /* Gravity */
    private float _verticalVelocity = 0;
    [SerializeField] private float _jumpForce = 10;
    [SerializeField] private float _gravity = 28;
    //private int _indexMetr = 1;
    private float _indexDistance = -1;

    /* Target */
    private MovementTarget _movementTarget;

     private bool _isMove = false;
     private Transform _lookAtTarget;

    private GearEffects _gearEffects;


    public void UpdateRunSpeed(float defaultRunSpeed)
    {
        _calcSpeed.UpdateSpeedRun(defaultRunSpeed);
    }

    public void UpdateWalkSpeed(float defaultWalkSpeed)
    {
        _calcSpeed.UpdateSpeedWalk(defaultWalkSpeed);
    }

    public bool RunningToDestination { get { return _isMove; } }
    public bool Running { get { return _running; } set { _running = value; } }
    public Vector3 MoveDirection { get { return _moveDirection; } }

    private static PlayerController _instance;
    public static PlayerController Instance { get { return _instance; } }

    private Entity _monster;
    private bool _isRotate = false;
    private bool _isFinish = true;
    private bool _turnsAround = true;

    private int _countTrigger = 0;
    private int _ppCountTrigger = 0;
    private float _elapsedTime = 0;

    
    private bool _isFirst = false;
  
    private bool _behindPlayer = false;
    private bool _switchWalkToRun = false;
    private bool _isRotateAttacker = false;
    private Vector3 _attackerPosition;
    public float angleThreshold = 3f;

    //newObject

    private PlayerPositionSender _pp_sender;
    private ICalcSpeed _calcSpeed;
    private PeriodicTimer _timer;
    private PeriodicTimer _pp_timer;
    public void Initialize()
    {
        if (_instance == null)
        {
            _instance = this;


            _calcSpeed = new PlayerMovementSpeedCalculator();
            _timer = new PeriodicTimer();
            _pp_timer = new PeriodicTimer();
            _pp_sender = new PlayerPositionSender();
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
        _controller = GetComponent<CharacterController>();
        _gearEffects = GetComponent<GearEffects>();
    }


    public void Update()
    {
        if (_isRotateAttacker & !_isMove)
        {
            if (!IsFacingAttacker(_attackerPosition))
            {
                Debug.Log("Rotate to object Attacker running : ");
                TurnTowardsAttacker(_attackerPosition);
            }
            else
            {
                Debug.Log("Rotate to object Attacker stop: ");
                _isRotateAttacker = false;
            }

        }
    }
    public void FixedUpdate()
    {
      

        if (_isMove)
        {
            Vector3 characterPosition = VectorUtils.To2D(transform.position);
            Vector3 targetPosition = VectorUtils.To2D(_movementTarget.GetTarget());

            if(VectorUtils.Distance2D(characterPosition, targetPosition) > _movementTarget.GetDistance())
            {

                _elapsedTime = Time.time;
                _countTrigger = _timer.GetTriggerCount(_elapsedTime, _countTrigger);
                SwitchWalkToRun(ref _switchWalkToRun, _countTrigger);

                

                float finalAngle = GetFinalAngle(targetPosition, characterPosition);

                float maxAngle = VectorUtils.GetMaxAngle(VectorUtils.To2D(_movementTarget.GetTarget()), transform);
                float speed = GetMovementSpeed(targetPosition, transform, maxAngle, _countTrigger);
                Vector3 newPosition = GetNewPosition(targetPosition, characterPosition, speed);
                Vector3 newPositionPlusGravity = ApplyGravity(newPosition , _elapsedTime);

                Quaternion targetRotation = GetNewRotate(finalAngle);
                float r_speed = _calcSpeed.GetSpeedRotate(_behindPlayer);
                transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation , r_speed);
                _controller.Move(newPositionPlusGravity * Time.deltaTime);


                _isFirst = false;
                _ppCountTrigger = _pp_timer.GetTriggerCount(_elapsedTime, _ppCountTrigger);
                _pp_sender.SendServerValidPositionEvery1ms(transform.position , _ppCountTrigger);
            }
            else
            {
                SetPositionServer(_movementTarget, targetPosition);
                PlayerStateMachine.Instance.NotifyEvent(Event.ARRIVED);
                StopMove();
            }
           

        }
    }

    public void RotateToAttacker(Vector3 attackerPosition)
    {
        _isRotateAttacker = true;
        _attackerPosition = attackerPosition;
    }

    private void TurnTowardsAttacker(Vector3 attackerPosition)
    {
        Vector3 directionToAttacker = (_attackerPosition - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(directionToAttacker);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
    }

    private bool IsFacingAttacker(Vector3 attackerPosition)
    {
        Vector3 directionToAttacker = (attackerPosition - transform.position).normalized;
        Vector3 forwardDirection = transform.forward;
        float angle = Vector3.Angle(forwardDirection, directionToAttacker);
        Debug.Log("Rotate to object Attacker running angle : " + angle);
        return angle < angleThreshold;
    }


    private void SetPositionServer(MovementTarget _movementTarget , Vector3 targetPosition)
    {
        if (_movementTarget.GetDistance() == 0.1f)
        {
            //apply static gravity
            var _targetPosition = new Vector3(targetPosition.x, transform.position.y, targetPosition.z);
            transform.position = _targetPosition;
            Debug.Log("PlayerController>Arrived: Refresh Player Position to Server Position");
        }
    }
    private void SwitchWalkToRun(ref bool switchWalkToRun , int countTrigger)
    {
        if (countTrigger > 3)
        {
            if (!switchWalkToRun)
            {
                switchWalkToRun = true;
                PlayerStateMachine.Instance.ChangeState(PlayerState.RUNNING);
                PlayerStateMachine.Instance.NotifyEvent(Event.MOVE_TO);
            }
        }

    }
    private float GetMovementSpeed(Vector3 targetPosition , Transform playerTransfrom , float maxAngle , float countTrigger)
    {
       // Debug.Log("Trigger count event " + countTrigger);

        if (countTrigger <= 3) return _calcSpeed.GetSpeed(false);

        if (VectorUtils.InFaceProcent(targetPosition, playerTransfrom, maxAngle) <= 50)
        {
            return _calcSpeed.CalculateInitialSpeed();
        }
        else
        {
            return _calcSpeed.GetSpeed(true);
        }
    }



  

    protected float GetFinalAngle(Vector3 targetPosition, Vector3 currentPosition)
    {
        Vector3 relativeDirection = targetPosition - currentPosition;
        float angleInRadians = Mathf.Atan2(relativeDirection.x, relativeDirection.z);
        float angleInDegrees = Mathf.Rad2Deg * angleInRadians;
        angleInDegrees = (angleInDegrees + 360) % 360; // Нормализация угла
        return angleInDegrees;
    }

    private Quaternion GetNewRotate(float finalAngle)
    {
        return Quaternion.Euler(Vector3.up * finalAngle);
    }

 
    private Vector3 GetNewPosition(Vector3 targetPosition, Vector3 characterPosition , float currentSpeed)
    {

        Vector3 relativeDirection = targetPosition - characterPosition;
        return relativeDirection.normalized * currentSpeed;
    }



 

    public void MoveToPoint(MovementTarget movementTarget)
    {
        ActivateIfNotMoving(_isMove);
        _timer.UpdateStartTime(_elapsedTime);
        _pp_timer.UpdateStartTime(_elapsedTime , 1);
        _movementTarget = movementTarget;
        _isMove = true;
        _behindPlayer = VectorUtils.IsTargetBehindPlayer(movementTarget.GetTarget(), transform);
        _ppCountTrigger = 0;
    }

    public void StopMove()
    {
        _isMove = false;
        ClickManager.Instance.HideLocator();
        _countTrigger = 0;
        _ppCountTrigger = 0;
        _switchWalkToRun = false;
        _pp_sender.SendServerArrivedPosition(transform.position);

    }

    private void ActivateIfNotMoving(bool is_move)
    {
         if (!is_move)
         {
            _switchWalkToRun = false;
            _isFirst = true;
         }
    }

  

    public bool IsFirstRun()
    {
        return _isFirst;
    }


    public bool IsTurnsAround()
    {
        return _turnsAround;
    }
    

    public void StartRotateFollow(Entity monster)
    {
        _monster = monster;
        _isRotate = true;
        _turnsAround = true;
    }


    //public void SetDestinationVector(Vector3 position, float distance)
    //{

    // if (!_runningToDestination)
    // {
    //   _switchWalkToRun = false;
    //    _isFirst = true;
    // }

    // _targetPositionVector = position;
    // _runningToDestination = true;
    //_stopAtRange = distance;
    // var _target2D = VectorUtils.To2D(position);
    //_isFinish = false;
    // _indexDistance = Vector3.Distance(_flatTransformPos, _target2D) - 1;
    //_startTime = _elapsedTime;
    //_maxAngle = VectorUtils.GetMaxAngle(_target2D, transform);
    //_behindPlayer = VectorUtils.IsTargetBehindPlayer(position, transform);
    //Debug.Log("Точка находится за спиной? " + VectorUtils.IsTargetBehindPlayer(position, transform));
    // }





    //public void ResetDestination()
    //{
    //  _moveTimeStamp = 0;
    //  _runningToDestination = false;
    //  _targetPositionVector = _flatTransformPos;
    //  _isFirst = true;
    // _switchWalkToRun = false;
    //  ClickManager.Instance.HideLocator();
    // }



    //private void MeasureSpeed()
    //{
    //    _currentPos = transform.position;
    //    _measuredSpeed = (_currentPos - _lastPos).magnitude / Time.deltaTime;
    //    _lastPos = _currentPos;
   // }


 

    public void ThinkMoveToPawn( ModelMovePawn model)
    {
        //PlayerStateMachine.Instance.IsMoveToPawn = true;
        
        PlayerStateMachine.Instance.ChangeIntention(Intention.INTENTION_MOVE_TO_PAWN, model);
    }

    public void InitMoveToPawn(MoveToPawn moveToPawnPacket)
    {
        
        if (PlayerEntity.Instance.TargetId == moveToPawnPacket.TarObjid)
        {
            Entity tarPos = World.Instance.GetEntityNoLockSync(moveToPawnPacket.TarObjid);
            DebugLineDraw.ShowDrawLineDebug(moveToPawnPacket.ObjId, moveToPawnPacket.ObjPos, tarPos.transform.position, Color.green);
            if (PlayerEntity.Instance.Target != null) ThinkMoveToPawn(new ModelMovePawn(PlayerEntity.Instance, tarPos,  moveToPawnPacket.Distance));
        }
        else
        {
            //ObjectData data = TargetManager.Instance.GetTargetByIdUseLocator(moveToPawnPacket.TarObjid);
            //if (data != null) Follow(data.ObjectTransform , data.ObjectTransform.position);
            //if (data != null) ThinkMoveToPawn(data.ObjectTransform, new ModelMovePawn(moveToPawnPacket.ObjPos , data.ObjectTransform, moveToPawnPacket.Distance));
        }
    }

    
    private Vector3 ApplyGravity(Vector3 dir , float elapsedTime)
    {
        /* Handle gravity */
        if (_controller.isGrounded)
        {
            if (_verticalVelocity < -1.25f)
            {
                _verticalVelocity = -1.25f;
            }
        }
        else
        {
            _verticalVelocity -= _gravity * elapsedTime;
        }
        dir.y = _verticalVelocity;

        return dir;
    }

  



 
    public void StartLookAt(Transform target)
    {
        UpdateFinalAngleToLookAt(target);

        // Wait for a small delay to lock on to target
        _lookAtTarget = target;
    }

    public void StopLookAt()
    {
        UpdateFinalAngleToLookAt(_lookAtTarget);
        _lookAtTarget = null;
    }

    public void UpdateFinalAngleToLookAt(Transform target)
    {
        if (target == null)
        {
            return;
        }

        float angle = Mathf.Atan2(target.position.x - transform.position.x, target.position.z - transform.position.z) * Mathf.Rad2Deg;
        angle = Mathf.Round(angle / 45f);
        angle *= 45f;
        _finalAngle = angle;
    }



    public Vector3 GetPlayerPosition()
    {
        return transform.position;
    }

    public Vector3 GetBodyPosition()
    {
        Transform tr = _gearEffects.GetBodyCenter();
        return tr.position;
    }


    public Vector3 GetCollisionSelf(Transform target)
    {
        float height = target.GetComponent<Entity>().Appearance.CollisionHeight;
        float radius = target.GetComponent<Entity>().Appearance.CollisionRadius;

        float heightC = VectorUtils.ConvertL2jDataOffset(height);
        float radiusC = VectorUtils.ConvertL2jDataOffsetRadius(radius);
        return VectorUtils.GetCollisionEffect(transform, target, heightC, radiusC);
    }

}
