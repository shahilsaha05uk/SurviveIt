using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class TestState
{
    protected readonly testCube2 cube;

    public TestState(testCube2 cube)
    {
        this.cube = cube;
    }

    public virtual IEnumerator Start()
    {
        yield break;
    }

    public virtual IEnumerator Idle()
    {
        Debug.Log("Idle");
        yield break;
    }
    public virtual IEnumerator Walk()
    {
        Debug.Log("Walk");

        yield break;
    }
    public virtual IEnumerator Attack()
    {
        yield break;
    }
    public virtual IEnumerator Die()
    {
        yield break;
    }
}


public class BeginState : TestState
{
    public BeginState(testCube2 cube):base(cube)
    {

    }

    public override IEnumerator Start()
    {
        return base.Start();
    }

}

