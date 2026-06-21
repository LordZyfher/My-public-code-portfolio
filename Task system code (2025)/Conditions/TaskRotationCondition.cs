using System;

[Serializable]
public class RotationCondition : TaskCondition
{
    public TransformTracker TransformTracker;
    public float TargetAngle;

    public enum Axis { Pitch, Yaw, Tilt }
    public Axis axis;

    public bool Additive = true;

    public enum Directions { Both, RightOrUp, LeftOrDown }
    public Directions Direction;

    public bool AutoEnableTracking = true;

    public override bool IsMet()
    {
        float rightOrUp = 0;
        float leftOrDown = 0;
        switch (axis)
        {
            case Axis.Pitch:
                if (AutoEnableTracking && !TransformTracker.TrackPitch) TransformTracker.SetTrackPitch(true);

                rightOrUp = TransformTracker.pitchUp;
                leftOrDown = TransformTracker.pitchDown;
                break;

            case Axis.Yaw:
                if (AutoEnableTracking && !TransformTracker.TrackYaw) TransformTracker.SetTrackYaw(true);

                rightOrUp = TransformTracker.rotationRight;
                leftOrDown = TransformTracker.rotationLeft;
                break;

            case Axis.Tilt:
                if (AutoEnableTracking && !TransformTracker.TrackTilt) TransformTracker.SetTrackTilt(true);

                rightOrUp = TransformTracker.tiltRight;
                leftOrDown = TransformTracker.tiltLeft;
                break;
        }

        return CalculateResult(rightOrUp, leftOrDown) > TargetAngle;
    }

    private float CalculateResult(float rightOrUp, float leftOrDown)
    {
        switch (Direction)
        {
            case Directions.Both:
                if (Additive) return rightOrUp + leftOrDown;
                else return Math.Abs(rightOrUp - leftOrDown);

            case Directions.RightOrUp:
                if (Additive) return rightOrUp;
                else return rightOrUp - leftOrDown;

            case Directions.LeftOrDown:
                if (Additive) return leftOrDown;
                else return leftOrDown - rightOrUp;
            default:
                return 0;
        }
    }

}