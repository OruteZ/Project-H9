using UnityEngine;
using UnityEngine.Events;

public static class PlayerEvents
{
    public static UnityEvent<Vector3Int> OnMovedPlayer = new UnityEvent<Vector3Int>();
    public static UnityEvent<Link> OnEnteredLinkinSight = new UnityEvent<Link>();
    public static UnityEvent<Tile> OnEnteredTileinSight = new UnityEvent<Tile>();
}
