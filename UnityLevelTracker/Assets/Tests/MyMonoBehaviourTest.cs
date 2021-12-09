using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class MyMonoBehaviourTest : MonoBehaviour, IMonoBehaviourTest
{
    private int _frameCount;
    public bool IsTestFinished
    {
        get { return _frameCount > 10; }
    }

    private void Update()
    {
        _frameCount++;
    }
}