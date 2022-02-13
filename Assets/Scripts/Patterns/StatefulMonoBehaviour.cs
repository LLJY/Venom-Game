
using System;
using UniRx;
using UnityEngine;

public abstract class StatefulMonoBehaviour<T>: MonoBehaviour
    where T: StatefulMonoBehaviour<T>
{
    public ReactiveProperty<State<T>> CurrentStateRx = new ReactiveProperty<State<T>>();

    public bool enableStatefulMb = true;
    // make use of CurrentState as a shorthand to access Rx Values from the reactive state
    public State<T> CurrentState
    {
        get => CurrentStateRx.Value;
        set
        {
            CurrentState?.CleanUp();
            CurrentStateRx.Value = value;
            CurrentState?.Prepare();
        }
    }
    // /// <summary>
    // /// The idea is that certain functions may be required to run in any state, such as camera follow,
    // /// implement such methods by implementing an EveryUpdate function to avoid a situation where we would need
    // /// each state to implement an any state class in order to get this desired behaviour
    // /// </summary>
    // public abstract void EveryUpdate();
    // public abstract void EveryLateUpdate();
    // public abstract void EveryFixedUpdate();
    public virtual void Awake()
    {
        // run stateful MBs using a manager
        enabled = false;
    }

    public virtual void Update()
    {
        if (enableStatefulMb)
        {
            CurrentState?.Update();
        }

    }
    public virtual void LateUpdate()
    {
        if (enableStatefulMb)
        {
            CurrentState?.LateUpdate();
        }

    }
    public virtual void FixedUpdate()
    {
        if (enableStatefulMb)
        {
            CurrentState?.FixedUpdate();

        }
    }
    
}
