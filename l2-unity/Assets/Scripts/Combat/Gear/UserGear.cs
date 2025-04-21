using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.HID;
using static ModelTable;

public class UserGear : Gear
{
    protected SkinnedMeshSync _skinnedMeshSync;
    [Header("Armors")]
    [Header("Meta")]
    [SerializeField] private Armor _torsoMeta;
    [SerializeField] private Armor _fullarmorMeta;
    [SerializeField] private Armor _legsMeta;
    [SerializeField] private Armor _glovesMeta;
    [SerializeField] private Armor _bootsMeta;
    //private GameObject _face;
    [Header("Models")]
    [SerializeField] private GameObject _container;
    [SerializeField] private GameObject _torso;
    [SerializeField] private GameObject _fullarmor;
    [SerializeField] private GameObject _legs;
    [SerializeField] private GameObject _gloves;
    [SerializeField] private GameObject _boots;
    [SerializeField] private GameObject _bou;
    private GameObject _hair1;
    private GameObject _face;
    private GameObject _hair2;


    public override void Initialize(int ownderId, CharacterRaceAnimation raceId) {
        base.Initialize(ownderId, raceId);

        if(this is PlayerGear) {
            _container = this.gameObject;
        } else {
            _container = transform.GetChild(0).gameObject;
        }

        _skinnedMeshSync = _container.GetComponentInChildren<SkinnedMeshSync>();
    }



    protected override Transform GetLeftHandBone() {
        if (_leftHandBone == null) {
            _leftHandBone = transform.FindRecursive("Weapon_L_Bone");
        }
        return _leftHandBone;
    }

    protected override Transform GetRightHandBone() {
        if (_rightHandBone == null) {
            _rightHandBone = transform.FindRecursive("Weapon_R_Bone");
        }
        return _rightHandBone;
    }

    protected override Transform GetShieldBone() {
        if (_shieldBone == null) {
            _shieldBone = transform.FindRecursive("Shield_L_Bone");
        }
        return _shieldBone;
    }

    public void EquipArmor(int itemId, ItemSlot slot) {
        Armor armor = ItemTable.Instance.GetArmor(itemId);
        
        if (armor == null) {
            Debug.LogWarning($"Can't find armor {itemId} in ItemTable");
            return;
        }

        ModelTable.L2ArmorPiece armorPiece = ModelTable.Instance.GetArmorPiece(armor, _raceId);
        //Debug.Log("UserGear: EquipArmor Material " + armorPiece.material + " BaseArmor Model " + armorPiece.baseArmorModel);

        if (armorPiece == null) {
            Debug.LogWarning($"Can't find armor {itemId} for race {_raceId} in slot {slot} in ModelTable");
            return;
        }

        GameObject mesh = Instantiate(armorPiece.baseArmorModel);
        mesh.GetComponentInChildren<SkinnedMeshRenderer>().material = armorPiece.material;

        SetArmorPiece(armor, mesh, slot);
    }

    public void ChangeFace(GameObject facePiece)
    {
        if (_torso != null)
        {
            Destroy(_torso);
            _torsoMeta = null;
        }

        _torso = facePiece;
        var tr = _container.transform;
        var count = _container.transform.childCount;
        if(count >= 5)
        {
            var child1 = _container.transform.GetChild(0);
            var child2 = _container.transform.GetChild(1);
            var child3 = _container.transform.GetChild(2);
            var child4 = _container.transform.GetChild(3);
            var child5 = _container.transform.GetChild(4);
            Debug.Log("");
        }
        //var child1 = _container.transform.GetChild(2);
        //var mesh = child1.GetComponentInChildren<SkinnedMeshRenderer>();
       // _torso.transform.SetParent(tr, false);
        //_skinnedMeshSync.SyncMesh();
    }

    public void SetFace(GameObject facePiece)
    {
        _face = facePiece;
    }
    public void SetHair1(GameObject hair1Piece)
    {
        _hair1 = hair1Piece;
    }

    public void SetHair2(GameObject hair2Piece)
    {
        _hair2 = hair2Piece;
    }


    public void EquipHair(GameObject hair1Piece, GameObject hair2Piece)
    {
        EquipHairTest(hair1Piece, hair2Piece);
    }
    public void EquipHairTest(GameObject hair1Piece , GameObject hair2Piece)
    {
        if (_hair1 != null)
        {
            DestroyImmediate(_hair1);
            DestroyImmediate(_hair2);

            _hair1 = null;
            _hair2 = null;

        }
        var tr = _container.transform;
        _hair1 = hair1Piece;
        _hair1.transform.SetParent(tr, false);

        _hair2 = hair2Piece;
        _hair2.transform.SetParent(tr, false);

        _skinnedMeshSync.SyncMesh();
    }

    public void EquipHair2(GameObject hair2Piece)
    {
        //Debug.Log("Количество после до   удаления2");
        //Debug.Log(_container.transform.childCount);
        if (_hair2 != null)
        {
            Destroy(_hair2);
            //_torsoMeta = null;
        }
        var tr = _container.transform;
        _hair2 = hair2Piece;
        _hair2.transform.SetParent(tr, false);

        //Debug.Log("Количество после удаления2");
        //Debug.Log(_container.transform.childCount);
        _skinnedMeshSync.SyncMesh();
    }
    public void EquipFace(GameObject facePiece)
    {
        if (_face != null)
        {
            Destroy(_face);
            //_torsoMeta = null;
        }
        var tr = _container.transform;
        _face = facePiece;
        _face.transform.SetParent(tr, false);

       _skinnedMeshSync.SyncMesh();
    }


    private void SetArmorPiece(Armor armor, GameObject armorPiece, ItemSlot slot) {
        switch (slot) {
            case ItemSlot.chest:
                if (_torso != null) {
                    Destroy(_torso);
                    _torsoMeta = null;
                }
                if (_fullarmor != null) {
                    Destroy(_fullarmor);
                    _fullarmorMeta = null;
                    EquipArmor(ItemTable.NAKED_LEGS, ItemSlot.legs);
                }
                _torso = armorPiece;
                _torsoMeta = armor;
                var tr = _container.transform;
                _torso.transform.SetParent(tr, false);
                break;
            case ItemSlot.fullarmor:
                if (_torso != null) {
                    Destroy(_torso);
                    _torsoMeta = null;
                }
                if (_legs != null) {
                    Destroy(_legs);
                    _legsMeta = null;
                }
                _fullarmor = armorPiece;
                _fullarmorMeta = armor;
                _fullarmor.transform.SetParent(_container.transform, false);
                break;
            case ItemSlot.legs:
                if (_legs != null) {
                    Destroy(_legs);
                    _legsMeta = null;
                }
                if (_fullarmor != null) {
                    Destroy(_fullarmor);
                    _fullarmorMeta = null;
                    EquipArmor(ItemTable.NAKED_CHEST, ItemSlot.chest);
                }
                _legs = armorPiece;
                _legs.transform.SetParent(_container.transform, false);
                _legsMeta = armor;
                break;
            case ItemSlot.gloves:
                if (_gloves != null) {
                    Destroy(_gloves);
                    _glovesMeta = null;
                }
                _gloves = armorPiece;
                _gloves.transform.SetParent(_container.transform, false);
                _glovesMeta = armor;
                break;
            case ItemSlot.feet:
                if (_boots != null) {
                    Destroy(_boots);
                    _bootsMeta = null;
                }
                _boots = armorPiece;
                _boots.transform.SetParent(_container.transform, false);
                _bootsMeta = armor;
                break;
        }

        _skinnedMeshSync.SyncMesh();
    }
}
