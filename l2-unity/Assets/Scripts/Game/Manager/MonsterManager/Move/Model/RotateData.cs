using UnityEngine;

public class RotateData
{
    private bool _isRotate;
    private PlayerEntity _pEntity;
    private MonsterEntity _mEntity;
    private float _defaultAngle = 3f;
    public RotateData(Entity pEntity , Entity mEntity)
    {
        _pEntity = (PlayerEntity)pEntity;
        _mEntity = (MonsterEntity)mEntity;
        _isRotate = true;
    }

    public RotateData(Entity pEntity, Entity mEntity , float defaultAngle)
    {
        _pEntity = (PlayerEntity)pEntity;
        _mEntity = (MonsterEntity)mEntity;
        _isRotate = true;
        _defaultAngle = defaultAngle;
    }

    public float GetAngleThreshold()
    {
        return _defaultAngle;
    }

    public bool IsRotate()
    {
        return _isRotate;
    }

    public Transform GetTargetTransform()
    {
        return _pEntity.transform;
    }

    public Transform GetMonsterTransform()
    {
        return _mEntity.transform;
    }
    public bool IsEntityTarget()
    {
        return _pEntity != null;
    }
    public bool IsEntityMonster()
    {
        return _mEntity != null;
    }

    public void SetRotate(bool isRotate)
    {
        _isRotate = isRotate;
    }
}
