using UnityEngine;

public class MovementTarget
{
    private object _objectPoint;
    private float _distance;

    public MovementTarget(object objectPoint , float distance)
    {
        _objectPoint = objectPoint;
        _distance = distance;
    }

    public MovementTarget(Entity entity, float distance)
    {
        _objectPoint = entity.transform;
        _distance = distance;
    }

    public float GetDistance()
    {
        return _distance;
    }
    public Vector3 GetTarget()
    {
        switch (_objectPoint)
        {
            case Transform transform:
                return transform.position;
            case Vector3 vector:
                return vector;
            default:
                Debug.LogError("Invalid object type");
                return Vector3.zero;
        }
    }
}