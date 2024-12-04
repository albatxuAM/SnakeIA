using UnityEngine;

public class FoodBehaviour : MonoBehaviour
{
    public void Respawn(Vector2Int newPosition)
    {
        transform.position = new Vector3(newPosition.x, 0.5f, newPosition.y);
    }
}
