using UnityEngine;



public class MoveMonster : MonoBehaviour
{
    //setting Rotate
    private Transform _targetObj;

    private float rotationSpeed = 40f; // Скорость поворота
    private bool _isRotate = false;
    public bool _isFollow = false;
    private bool _isMove = false;
    private Quaternion _lastRotation; // Хранит поворот 
    public float rotationThreshold = 0.01f; // Порог изменения для определения вращения

    //setting Follow
    private ModelMovePawn _targetModelObj;
    //private Vector3 _targetPosFollow;
    private Vector3 _targetMove; 
    private float _stopDist = 0.10f;
    private Transform _lookAtTarget;
    private float _finalAngle;
    private Vector3 _moveDirection;
    private Vector3 _moveTargetVector;
    private float _verticalVelocity = 0;
    private float _gravity = 28;
    private CharacterController _controller;
    private MonsterEntity _entity;
    private MonsterStateMachine _stateMachine;
    private Vector2 _axis;
    private Vector3 _flatTransformPos;
    private Vector3 _flatCurrentVectorPos;
    //private bool _running;
    private float _currentSpeed;
    //_measured
    private Vector3 _beginPos;
    private float _measuredSpeed;
    private Vector3 _lastPos;

    private  bool _isTeleport = false;
    private Vector3 _posTeleport;
    private bool _detectedIsMove = false;
    private Vector3 _lastPosSpecial = Vector3.zero;
    private DrawLine _debugLine;
    private float _monsterId = 0;
    void Start()
    {
        _controller = GetComponent<CharacterController>();
        _entity = GetComponent<MonsterEntity>();
        _stateMachine = GetComponent<MonsterStateMachine>();
        var _debugObject = GameObject.FindWithTag("DebugMove");
        _debugLine = _debugObject.GetComponent<DrawLine>();
    }

 
    public bool IsFollow()
    {
        return _isFollow;
    }

    public bool IsSpecialDetectedIsMoveObject()
    {
        if (_isFollow == true | _isMove == true) return true;

        if(transform.position == _lastPosSpecial)
        {
            return false;
        }

        _lastPosSpecial = transform.position;

        return _detectedIsMove;
    }

    public void SetFollow(bool follow)
    {
       _isFollow = follow;
    }


    private void FixedUpdate()
    {
            EventDead();
            if (_isRotate)
            {
                // Проверяем, изменился ли поворот
                if (IsRotating())
                {
                    Debug.Log("Персонаж поворачивается. " + transform.position);
                }
                else
                {
                    if (_targetObj != null)
                    {
                        RotateObject();
                    }

                }

                //_isRotate = false;
                _lastRotation = transform.rotation;
            }

            if (_isFollow)
            {
                ///if (_targetObjFollow == null)
                    //return;

                //Debug.Log("IS FOLLOW RUNNING!!!!!!!!!!!!!!!!!!!!!!! "  + transform.position);

                _flatTransformPos = new Vector3(transform.position.x, 0, transform.position.z);

                if (!_entity.IsDead())
                {
                    // Debug.Log("Monster Enter 2");
                    //var cVector = VectorUtils.To2D(_targetObjFollow.position);
                    var cVector = new Vector3(_targetModelObj.TarEntity().transform.position.x, 0, _targetModelObj.TarEntity().transform.position.z);
                   // Debug.Log("Follow 1111 To Monster position! Dist " + Vector3.Distance(cVector, _flatTransformPos) +  "  \\  " + _targetModelObj.Distance());
                   // Debug.Log(" test 1");
                    if (Vector2.Distance(cVector, _flatTransformPos) <= _targetModelObj.Distance())
                    {
                        //Debug.Log("Follow 1111 To Monster position! FINISH 1");
                        OnFinish(_targetModelObj.TarEntity().transform.position);
                        _isFollow = false;
                        Debug.Log(" test 3 ");
                    }

                    if (Vector2.Distance(cVector, _flatTransformPos) >= _targetModelObj.Distance())
                    {
                        //Debug.Log("Follow 1111 To Monster position! Return  2");
                        MoveToPoint(_targetModelObj.TarEntity().transform.position, true);
                       // Debug.Log(" test 2 ");
                    }
                }
            }
            else if (_isMove == true)
            {
                if (_targetMove == null)
                    return;
                    MoveToPoint(_targetMove , false);

            }

        
    }


    private void EventDead()
    {
        if (_entity.IsDead())
        {
            StopAllMove();
        }
    }
    private void StopAllMove()
    {
        _isMove = false;
        _isFollow = false;
        _isRotate = false;
    }

    public void CancelMove()
    {
        if (_isMove)
        {
            _isMove = false;
        }
    }

    public float stoppingDistance = 0.1f; // 

    private void MoveToPoint(Vector3 target , bool ignoreDist)
    {
        
        if (target != null)
        {
            var gravityOffTransform = new Vector3(transform.position.x, 0, transform.position.z);
            var gravityOffTarget = new Vector3(target.x, 0, target.z);
            float monsterSpeed = GetMonsterSpeed();

            float distance = VectorUtils.Distance2D(transform.position, target);

            if (distance >= stoppingDistance | ignoreDist == true)
            {


                Vector3 point = gravityOffTarget - gravityOffTransform;
                Vector3 direction = point.normalized;

                 direction = ApplyGravity(direction);

                 _detectedIsMove = true;
                
                _controller.Move(direction * monsterSpeed * Time.deltaTime);


                    Quaternion lookRotation = Quaternion.LookRotation(direction);
                    transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5.0f); // Плавный поворот
                

                if (_lastPos == transform.position)
                {
                        OnFinish(target);
                }
            }
            else
            {
                OnFinish(target);
            }

            _lastPos = transform.position;
        }
    }

    private void OnFinish(Vector3 target)
    {
       // Debug.Log("MoveMonster: Мы достигли точки назначения останавливаемя и ждем!!!! " + this.name);
        _isMove = false;

        //if (_isTeleport == false)
        //{
            var teset = new Vector3(target.x, 0, target.z);
            transform.position = teset;
        //}
        _detectedIsMove = false;
        _stateMachine.NotifyEvent(Event.ARRIVED);
        //Debug.Log("MoveTagetPosition END Vector3  Vector " + transform.position);
    }

    public bool IsMoving()
    {
        return _isMove;
    }

    public void TeleportToPosition(Vector3 position)
    {
        _isTeleport = true;
        _posTeleport = position;
    }

    public void SetAccessTeleport(bool isTeleport)
    {
        //this._isTeleport = isTeleport;
    }
    
    private float GetMonsterSpeed()
    {
        return (_entity.Running) ? _entity.Stats.UnitySpeedRun : _entity.Stats.UnitySpeedWalking;
    }
  

    private bool IsRotating()
    {
        float rotationDifference = Quaternion.Angle(_lastRotation, transform.rotation);
        return rotationDifference > rotationThreshold;
    }
    public bool IsRotate { get { return _isRotate; } }

    public void RotateInTargetObject(Transform targetObj)
    {
        _targetObj = targetObj;
        _isRotate = true;
        _lastRotation = transform.rotation;
    }

    public void StopRotateObject()
    {
        _isRotate = false;
    }

    public void MoveToPawn(ModelMovePawn targetObj)
    {
        _isFollow = true;
        _targetModelObj = targetObj;
        //ShowDrawLineDebug(transform.position, targetObj.TarObj());
    }



    public void MoveToTargetPosition(Vector3 targetVector)
    {
        _isMove = true;
        _targetMove = targetVector;
        _lastPos = transform.position;
        //Debug.Log("MoveTagetPosition BEGIN Vector3 " + targetVector);
    }

    public void RotateObject()
    {

       Vector3 direction = _targetObj.position - transform.position;
       direction.y = 0;

       if(_isTeleport == false)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
            
    
    }

 
    public bool IsTeleport()
    {
        return _isTeleport;
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

    private Vector3 ApplyGravity(Vector3 dir)
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
            _verticalVelocity -= _gravity * Time.deltaTime;
        }
        dir.y = _verticalVelocity;

        return dir;
    }

  

 

  


}
