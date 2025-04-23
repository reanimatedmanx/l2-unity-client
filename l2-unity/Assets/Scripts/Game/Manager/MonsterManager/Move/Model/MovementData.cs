using UnityEditorInternal;
using UnityEngine;
using UnityEngine.InputSystem.XR;

public class MovementData
{
    private MonsterEntity _mEntity;
    private MovementTarget _movementTarget;

    private float _verticalVelocity = 0;
    private float _gravity = 28;
    private bool _isMove;
    private bool _isRotate = false;
    private Vector3 _lastPos;


    public MovementData(Entity mEntity , MovementTarget movementTarget)
    {
        _mEntity = (MonsterEntity)mEntity;
        _movementTarget = movementTarget;
        _isMove = true;
    }

    public bool IsEntity()
    {
        return _mEntity != null;
    }

    public void SetLastPosition(Vector3 lastPos)
    {
        _lastPos = lastPos;
    }

    public Vector3 GetLastPosition()
    {
        return _lastPos;
    }

    public MovementTarget GetMovementTarget()
    {
        return _movementTarget;
    }

    public Transform GetTransform()
    {
        return _mEntity.transform;
    }

    public float GetDistance()
    {
        return _movementTarget.GetDistance();
    }
    public bool IsMove()
    {
        return _isMove;
    }

    public void SetIsMove(bool isMove)
    {
        _isMove = isMove;
    }

    public float GetSpeed()
    {
        return (_mEntity.Running) ? _mEntity.Stats.UnitySpeedRun : _mEntity.Stats.UnitySpeedWalking;
    }

    public void Move(Vector3 direction , float speed)
    {
        direction = ApplyGravity(direction);
        _mEntity.GetCharacterController().Move(direction * speed * Time.deltaTime);
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        _mEntity.transform.rotation = Quaternion.Slerp(_mEntity.transform.rotation, lookRotation, Time.deltaTime * 5.0f);
    }
    private Vector3 ApplyGravity(Vector3 dir)
    {
        /* Handle gravity */
        if (_mEntity.GetCharacterController().isGrounded)
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

    public void OnFinish(Vector3 target)
    {
        _isMove = false;
        _mEntity.transform.position = new Vector3(target.x, 0, target.z); ;
        _mEntity.GetStateMachine().NotifyEvent(Event.ARRIVED);
    }

}
