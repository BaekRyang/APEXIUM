using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.PlayerLoop;

public abstract class DIMono : MonoBehaviour
{
    private InjectObj injectObj = new InjectObj();

    public void CheckAndInject()
    {
        injectObj.CheckAndInject(this);
    }
    
    private void Start()
    {
        CheckAndInject();
        Initialize();
    }

    public virtual void Initialize()
    {
    }
}