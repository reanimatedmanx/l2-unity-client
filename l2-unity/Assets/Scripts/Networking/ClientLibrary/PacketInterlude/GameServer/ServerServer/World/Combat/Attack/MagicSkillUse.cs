using System;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class MagicSkillUse : ServerPacket
{
    private int _attackerObjId;
    private int _targetObjId;
    private Vector3 _attackerPos;
    private Vector3 _targetPos;
    private int _aX;
    private int _aY;
    private int _aZ;

    private int _tX;
    private int _tY;
    private int _tZ;

    private int _skillId;
    private int _skilllvl;
    private int _hittime;
    private int _reusedelay;
    private int _critical;
    private Entity attacker;

    public void SetAttacker(Entity entity)
    {
        attacker = entity;
    }
    public Entity EntityAttacker { get => attacker; }
    public int SkillId { get => _skillId; }

    public int SkillLvl { get => _skilllvl; }

    public int HitTime { get => _hittime; }

    public int Reusedelay { get => _reusedelay; }

    public int AttackerObjId { get => _attackerObjId; }

    public int TargetId { get => _targetObjId; }

    public Vector3 AttackerPos { get => _attackerPos; }
    public Vector3 TargetPos { get => _targetPos; }

    public MagicSkillUse(byte[] d) : base(d)
    {
        Parse();
    }

    public override void Parse()
    {
        _attackerObjId = ReadI();
        _targetObjId = ReadI();

        _skillId = ReadI();
        Debug.Log("MagicSkillUse->skillID " + _skillId);
        _skilllvl = ReadI();
        _hittime = ReadI();
        Debug.Log("MagicSkillUse->hittime " + _hittime);
        _reusedelay = ReadI();

        _aX = ReadI();
        _aY = ReadI();
        _aZ = ReadI();
        _attackerPos = VectorUtils.ConvertPosToUnity(new Vector3(_aX, _aY, _aZ));
        
        _critical = ReadI();

        if(_critical == 1)
        {
            ReadSh();
        }

        _tX = ReadI();
        _tY = ReadI();
        _tZ = ReadI();
        _targetPos = VectorUtils.ConvertPosToUnity(new Vector3(_tX, _tY, _tZ));

        //DebugInfo(_aX, _aY, _aZ, _tX, _tY, _tZ);
    }

    private void DebugInfo(int ax , int ay , int az , int tx , int ty , int tz)
    {
        var atak = new Vector3(ax , ay , az);
        var tar = new Vector3(tx, ty, tz);
        float distance = Vector3.Distance(atak, tar);
        Debug.Log("DISTANCE TO SERVER L2J " + distance);
    }
}
