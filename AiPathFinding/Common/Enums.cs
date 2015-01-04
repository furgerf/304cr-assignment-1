namespace AiPathFinding.Common
{

    #region Enums related to algorithms

    /// <summary>
    /// Names of all path finding algorithms that can be applied.
    /// </summary>
    public enum PathFindName
    {
        Dijkstra,
        AStar,
        Count
    }

    /// <summary>
    /// Names of all fog exploration algorithms that can be applied.
    /// </summary>
    public enum FogExploreName
    {
        MinCost,
        MinDistanceToTarget,
        MinCostPlusDistanceToTarget,
        Count
    }

    /// <summary>
    /// Strategy how to pick a foggy node when stuck during pathfinding.
    /// </summary>
    public enum FogMethod
    {
        ClosestToPlayer,
        ClosestToTarget,
        MinClosestToPlayerPlusTarget,
        Count
    }

    #endregion

    #region Enums relating to the model

    /// <summary>
    /// Four directions.
    /// </summary>
    public enum Direction
    {
        East,
        South,
        West,
        North
    }

    /// <summary>
    /// Types of entities.
    /// </summary>
    public enum EntityType
    {
        Player,
        Target,
        Count
    }

    /// <summary>
    /// Possible terrains for a node.
    /// </summary>
    public enum Terrain
    {
        Street,
        Plains,
        Forest,
        Hill,
        Mountain,
        Count
    }

    #endregion
}
