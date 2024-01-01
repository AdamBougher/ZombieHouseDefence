using UnityEngine;

public static class vectorExtension 
{
    public static Vector3 WithAxis(this Vector3 vector, Axis axis, float value)
    {
        return new Vector3(
            axis == Axis.x ? value : vector.x,
            axis == Axis.y ? value : vector.y,
            axis == Axis.z ? value : vector.z
        );
    }
}

public enum Axis
{
    x,y,z
}