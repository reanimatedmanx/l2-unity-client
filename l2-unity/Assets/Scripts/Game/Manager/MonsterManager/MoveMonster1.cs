using UnityEngine;



public class MoveMonster : MonoBehaviour
{
    //setting Rotate
    private Transform _targetRotation;


    private bool _isRotate = false;
    public bool _isFollow = false;
    private bool _isMove = false;
    private Quaternion _lastRotation; 
    public float rotationThreshold = 0.01f; 
    private MovementTarget _movementTarget;
    private float _verticalVelocity = 0;
    private float _gravity = 28;
    private CharacterController _controller;
    private MonsterEntity _entity;
    private MonsterStateMachine _stateMachine;
    private Vector3 _flatTransformPos;
    private Vector3 _lastPos;
    public float angleThreshold = 3f;


    void Start()
    {
        //_controller = GetComponent<CharacterController>();
       // _entity = GetComponent<MonsterEntity>();
       // _stateMachine = GetComponent<MonsterStateMachine>();
    }

 
    public bool IsFollow()
    {
        return _isFollow;
    }


    //backup
    private void Update()
    {
        //if (_isRotate & !_isMove)
        //{
            //if (!VectorUtils.IsFacingAttacker(transform, _targetRotation.position, angleThreshold))
           // {
                // Debug.Log("Rotate to object Attacker running : ");
             //   TurnTowardsAttacker(_targetRotation.position);
           // }
           // else
            //{
             //   //Debug.Log("Rotate to object Attacker stop: ");
                _isRotate = false;
           // }

       // }
    }
    //backUp
    private void FixedUpdate()
    {
            //EventDead();

           

            //if (_isFollow)
            //{


              //  _flatTransformPos = new Vector3(transform.position.x, 0, transform.position.z);

               /// if (!_entity.IsDead())
               /// {
                   // if (VectorUtils.Distance2D(_movementTarget.GetTarget(), _flatTransformPos) <= _movementTarget.GetDistance())
                    //{
                     //   OnFinish(_movementTarget.GetTarget());
                     //   _isFollow = false;
                    //}

                    //if (VectorUtils.Distance2D(_movementTarget.GetTarget(), _flatTransformPos) >= _movementTarget.GetDistance())
                    //{
                    //    MoveToPoint(_movementTarget.GetTarget(), true);
                    //}
                //}
            //}
            //else if (_isMove == true)
            //{
                
              //  if (_movementTarget.GetTarget() == null)
                 //   return;
                  //  MoveToPoint(_movementTarget.GetTarget(), false);

           // }

        
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
        _isMove = false;
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



                  if (_isMove)
                  {
                    _controller.Move(direction * monsterSpeed * Time.deltaTime);
                    Quaternion lookRotation = Quaternion.LookRotation(direction);
                    transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5.0f); 
                  }


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
        _isMove = false;
        transform.position = new Vector3(target.x, 0, target.z); ;
        _stateMachine.NotifyEvent(Event.ARRIVED);
    }

    public bool IsMoving()
    {
        return _isMove;
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

    public void RotateInTargetObject(Transform targetObj)
    {
        _targetRotation = targetObj;
        _isRotate = true;
        _lastRotation = transform.rotation;
    }

    public void StopRotateObject()
    {
        _isRotate = false;
    }

    private void TurnTowardsAttacker(Vector3 attackerPosition)
    {
        Vector3 directionToAttacker = (attackerPosition - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(directionToAttacker);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 25f);
    }

    public void MoveToPawn(MovementTarget movementTarget)
    {
        _isFollow = true;
        _movementTarget = movementTarget;
        //ShowDrawLineDebug(transform.position, targetObj.TarObj());
    }



    public void MoveToTargetPosition(MovementTarget movementTarget)
    {
        _isMove = true;
        _movementTarget = movementTarget;
        _lastPos = transform.position;
        //Debug.Log("MoveTagetPosition BEGIN Vector3 " + targetVector);
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
