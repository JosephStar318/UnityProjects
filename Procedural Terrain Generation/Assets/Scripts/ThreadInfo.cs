using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ThreadInfo<T>
{
    public Action<T> callback;
    public T param;

    public ThreadInfo(Action<T> callback, T param)
    {
        this.callback = callback;
        this.param = param;
    }
}
