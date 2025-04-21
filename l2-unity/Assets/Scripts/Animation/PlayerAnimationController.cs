using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PlayerAnimationController : BaseAnimationController
{
    private static PlayerAnimationController _instance;
    public static PlayerAnimationController Instance { get { return _instance; } }

    public override void Initialize()
    {
        base.Initialize();

        if (_instance == null)
        {
            _instance = this;
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


    public void SetBool(string name, bool value, bool share)
    {
        if (_animator.GetBool(name) != value)
        {
            Debug.LogWarning($"Player Animatorr Cntoller: Set bool {name}={value}");

            SetBool(name, value);
        }
    }


}

