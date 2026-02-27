using Godot;

public static class LabTools
{
    public static bool VectorsWithinAngle(Vector2 aNormalized, Vector2 bNormalized, float rad)
    {
        // Dot(a, b) = cos(angleDeg)
        // cos is a decreasing function on [0, pi]
        // therefore if cos of angle(a, b) is greater, the angle is smaller
        return aNormalized.Dot(bNormalized) >= Mathf.Cos(rad);
    }

    public static bool VectorsWithinAngle(Vector3 aNormalized, Vector3 bNormalized, float rad)
    {
        // Dot(a, b) = cos(angleDeg)
        // cos is a decreasing function on [0, pi]
        // therefore if cos of angle(a, b) is greater, the angle is smaller
        return aNormalized.Dot(bNormalized) >= Mathf.Cos(rad);
    }

}
