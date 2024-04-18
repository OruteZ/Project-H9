using UnityEngine.Events;

public static class PlayerEvents
{
    public static UnityEvent<Link> OnEnteredLinkinSight = new UnityEvent<Link>();
    public static UnityEvent<Tile> OnEnteredTileinSight = new UnityEvent<Tile>();
}
