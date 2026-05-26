using System.Collections.Generic;
using System;
using UnityEngine;
using System.Linq;

[Serializable]
public abstract class MoveTrackCondition : TaskCondition
{
    public TransformTracker TransformTracker;
    public enum MoveDirection { RightLeft, UpDown, FwdBwd }
    public MoveDirection MoveDir;
    public enum Directions { Both, RightUpOrFwd, LeftDownOrBwd }
    public Directions Direction;

    public bool AutoEnableTracking = true;

    public override bool IsMet() { return false; }

    protected Vector2 GetMovementValues()
    {
        Vector2 values = new();
        switch (MoveDir)
        {
            case MoveDirection.RightLeft:
                if (AutoEnableTracking && !TransformTracker.TrackSideways) TransformTracker.SetTrackSideways(true);

                values.x = TransformTracker.transformRight;
                values.y = TransformTracker.transformLeft;
                return values;

            case MoveDirection.UpDown:
                if (AutoEnableTracking && !TransformTracker.TrackVertical) TransformTracker.SetTrackVertical(true);

                values.x = TransformTracker.transformUp;
                values.y = TransformTracker.transformDown;
                return values;

            case MoveDirection.FwdBwd:
                if (AutoEnableTracking && !TransformTracker.TrackForwardBack) TransformTracker.SetTrackForwardBack(true);

                values.x = TransformTracker.transformForward;
                values.y = TransformTracker.transformBackward;
                return values;
        }
        return values;
    }
}


[Serializable]
public class MoveCondition : MoveTrackCondition
{
    public float TargetDistance;

    public bool Additive = true;

    public override bool IsMet()
    {
        Vector2 pos = GetMovementValues();

        return CalculateResult(pos) > TargetDistance;
    }

    private float CalculateResult(Vector2 pos)
    {
        switch (Direction)
        {
            case Directions.Both:
                if (Additive) return pos.x + pos.y;
                else return Math.Abs(pos.x - pos.y);

            case Directions.RightUpOrFwd:
                if (Additive) return pos.x;
                else return pos.x - pos.y;

            case Directions.LeftDownOrBwd:
                if (Additive) return pos.y;
                else return pos.y - pos.x;
            default:
                return 0;
        }
    }
}

[Serializable]
public class VelocityCondition : MoveTrackCondition
{
    public float TargetVelocity;

    [Tooltip("if true: once the velocity difference with the target exceeds the velocity at the start of this task the condition is met.")]
    public bool isVelocityDifference;
    [Min(1)]public byte sampleCount = 4;

    //value exists to not be dependent on .deltaTime, which only works well if this code is called every update.
    private float lastTime;

    private float startVelocity = 0;
    private Vector2 startPos;
    private Vector2 lastPos;
    private List<float> velocitySamples = new();

    private bool valuesInitialized = false;
    private bool startVelocityIsSet = false;

    public override bool IsMet()
    {
        if (!valuesInitialized)
        {
            lastTime = Time.time;

            startPos = GetMovementValues();
            lastPos = startPos;

            valuesInitialized = true;
            velocitySamples = new();

            return false;
        }

        //sampling until sufficient results are in. Sampling speed increases depending on the sample count increasing.
        if (Time.time - lastTime >= 0.5f / sampleCount)
        {
            Vector2 values = GetMovementValues();
            velocitySamples.Add(GetVelocity(values, lastPos, lastTime));
            lastTime = Time.time;
            lastPos = values;
            return false;
        }

        if (velocitySamples.Count < sampleCount)
            return false;

        if (!startVelocityIsSet && isVelocityDifference)
        {
            startVelocity = velocitySamples.Average();
            startVelocityIsSet = true;
            return false;
        }

        //set velocity and if this method runs again, start the sampling again.
        float velocity = velocitySamples.Average();
        velocitySamples.Clear();

        //allows some debugging of the values
        bool result = velocity > TargetVelocity;
        if (isVelocityDifference)
        {
            float diff = Mathf.Abs(velocity - startVelocity);
           // Debug.Log($"result = {result}, velocity {Mathf.Round(velocity)}, velocityDifference {Mathf.Round(diff)}. Target = {TargetVelocity}");
            result = diff >= TargetVelocity;
        }
        else
        {
          //  Debug.Log($"result = {result}, velocity {Mathf.Round(velocity)}. Target = {TargetVelocity}");
        }

        return result;
    }

    private float GetVelocity(Vector2 current, Vector2 last, float lastTime)
    {
        float deltaTime = Time.time - lastTime;
        if (deltaTime <= 0f) return 0f;

        float delta = 0f;

        switch (Direction)
        {
            case Directions.Both:
                delta = ((current.x - last.x) - (current.y - last.y));
                break;
            case Directions.RightUpOrFwd:
                delta = current.x - last.x;
                break;
            case Directions.LeftDownOrBwd:
                delta = current.y - last.y;
                break;
        }

        return delta / deltaTime;
    }

}

