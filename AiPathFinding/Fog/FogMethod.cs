namespace AiPathFinding.Fog
{
    /// <summary>
    /// Strategy how to pick a foggy node when stuck during pathfinding.
    /// </summary>
    public enum FogMethod
    {
        ClosestToPlayer, ClosestToTarget, MinClosestToPlayerPlusTarget, Count
    }
}
