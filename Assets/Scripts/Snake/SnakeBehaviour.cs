using System.Collections.Generic;
using UnityEngine;

public class SnakeBehaviour : MonoBehaviour
{
    private Vector2Int position;
    private Vector2Int direction = Vector2Int.up; // Movimiento inicial hacia arriba

    [SerializeField] private float moveInterval = 0.2f;
    private float moveTimer;
    private bool hasStarted = false; // Controla si la serpiente comenzó a moverse

    private List<Vector2Int> body = new List<Vector2Int>(); // Cuerpo de la serpiente
    [SerializeField] private GameObject snakePartPrefab; // Prefab para la parte de la serpiente

    private void Start()
    {
        // Obtiene la posición inicial de la serpiente desde el tablero
        position = new Vector2Int((int)transform.position.x, (int)transform.position.z);
        body.Add(position); // Añadimos la cabeza de la serpiente al cuerpo

        // Inicializamos la serpiente en el grid
        Board.Instance.SetCellType(position, CellType.Snake); // Coloca la cabeza en el grid
    }

    private void Update()
    {
        HandleInput();

        if (!hasStarted) return; // No moverse hasta que se presione una tecla

        moveTimer += Time.deltaTime;

        if (moveTimer >= moveInterval)
        {
            Move();
            moveTimer = 0;
        }
    }

    private void HandleInput()
    {
        // Revisar si se ha presionado alguna tecla de movimiento
        if (Input.GetKey(KeyCode.W))
        {
            direction = Vector2Int.up;
            hasStarted = true; // Inicia el movimiento
        }
        else if (Input.GetKey(KeyCode.S))
        {
            direction = Vector2Int.down;
            hasStarted = true;
        }
        else if (Input.GetKey(KeyCode.A))
        {
            direction = Vector2Int.left;
            hasStarted = true;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            direction = Vector2Int.right;
            hasStarted = true;
        }
    }

    private void Move()
    {
        Vector2Int newPosition = position + direction;

        // Obtener el tamaño del tablero desde el singleton
        Vector2Int boardSize = Board.Instance.GetBoardSize();

        // Verificar si la serpiente golpea las paredes
        if (newPosition.x < 0 || newPosition.x >= boardSize.x || newPosition.y < 0 || newPosition.y >= boardSize.y)
        {
            Debug.Log("Game Over: Snake hit the wall");
            return;
        }

        // Comprobar si la serpiente ha comido la comida
        if (Board.Instance.GetCellData(newPosition).cellType == CellType.Food)
        {
            Debug.Log("Snake ate the food!");
            Board.Instance.IncreaseScore(); // Aumentar el puntaje

            // Eliminar la comida del tablero y destruir la instancia
            Board.Instance.DestroyGridObject(newPosition);

            // Respawn de la comida
            Board.Instance.SpawnFoodAtRandomPosition();

            // Agregar una nueva parte a la serpiente (crece solo cuando come)
            Vector2Int newBodyPartPos = body[body.Count - 1]; // El cuerpo crece en la última parte
            body.Add(newBodyPartPos); // Añadimos una nueva parte al cuerpo
        }
        else
        {
            // Si no ha comido comida, mueve la cola y elimina la última parte
            Vector2Int tail = body[body.Count - 1]; // Última parte (cola)

            // Destruir el GameObject correspondiente a la cola
            Board.Instance.DestroyGridObject(tail);  // Llamada al método DestroyGridObject
            Board.Instance.SetCellType(tail, CellType.Empty);  // Elimina la cola de la grilla
            body.RemoveAt(body.Count - 1);  // Elimina la última parte del cuerpo (cola)
        }

        // Actualizar la grilla y posición de la cabeza
        Board.Instance.SetCellType(position, CellType.Empty);  // Elimina la cabeza vieja
        position = newPosition;  // Actualiza la nueva posición de la cabeza
        Board.Instance.SetCellType(position, CellType.Snake);  // Coloca la nueva cabeza

        // Actualizar el cuerpo de la serpiente
        body.Insert(0, position); // Coloca la nueva cabeza al inicio de la lista del cuerpo

        // Colocar las nuevas partes del cuerpo en la escena
        for (int i = 1; i < body.Count; i++)  // Las partes del cuerpo comienzan desde la segunda
        {
            // Actualizar las partes del cuerpo (sin necesidad de instanciarlas cada vez)
            Vector2Int partPos = body[i];
            Board.Instance.SetCellType(partPos, CellType.Snake); // Actualiza la celda correspondiente en el grid
        }

        // Actualizar la posición del objeto que representa la cabeza de la serpiente en la escena
        transform.position = new Vector3(position.x, 0, position.y);
    }
}
