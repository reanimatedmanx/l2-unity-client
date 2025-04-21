using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClassIdTemplate
{
    private int _pId;
    private bool _mage;
    private InterludeRace _race;
    private ClassIdTemplate _pParent;

    public ClassIdTemplate(int pId , bool mage , InterludeRace race , ClassIdTemplate pParent)
    {
        this._pId = pId;
        this._mage = mage;
        this._race = race;
        this._pParent = pParent;
    }

    public int GetClassId()
    {
        return this._pId;
    }
}
