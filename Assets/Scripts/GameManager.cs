using UnityEngine;
using Random = System.Random;

public class Board : MonoBehaviour
{
    public static Board Instance { get; private set; }

    [Header("Configuration")]
    [SerializeField] private Camera cam;
    [SerializeField] private Vector2Int size;
    [SerializeField] private int randomSeed;

    [Header("MapBuild")]
    [SerializeField] private GameObject floorPrefab;
    [SerializeField] private GameObject wallPrefab;
    [SerializeField] private GameObject snakePrefab;
    [SerializeField] private GameObject foodPrefab;

    //private Grid2D<CellType> grid;
    private Grid2D<CellData> grid;

    private Random random;

    private GameObject snakeInstance;

    private int score = 0;
    public int Score => score;

    private void Awake()
    {
        // Ensure only one instance exists
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        cam.transform.localPosition = new Vector3(size.x / 2, Mathf.Max(size.x, size.y), size.y / 2);
        random = new Random(randomSeed);
        GenerateBoard();
        DrawBoard();

        // Repetir la generación de comida cada 5 segundos
        //InvokeRepeating(nameof(SpawnFoodAtRandomPosition), 5f, 5f);
        SpawnFoodAtRandomPosition();
    }

    private void GenerateBoard()
    {
        grid = new Grid2D<CellData>(size, Vector2Int.zero);

        // Inicializar el grid
        for (int y = 0; y < size.y; y++)
        {
            for (int x = 0; x < size.x; x++)
            {
                // Inicializar el grid con celdas vacías y sin objetos asociados
                grid[new Vector2Int(x, y)] = new CellData(CellType.Empty, null);
            }
        }

        // Colocar la serpiente en una posición aleatoria
        Vector2Int snakePos = new Vector2Int(random.Next(0, size.x), random.Next(0, size.y));
        grid[snakePos] = new CellData(CellType.Snake, Instantiate(snakePrefab, new Vector3(snakePos.x, 0, snakePos.y), Quaternion.identity));

        // Colocar la comida en una posición aleatoria diferente
        Vector2Int foodPos;
        do
        {
            foodPos = new Vector2Int(random.Next(0, size.x), random.Next(0, size.y));
        }
        while (grid[foodPos].cellType != CellType.Empty);
        grid[foodPos] = new CellData(CellType.Food, Instantiate(foodPrefab, new Vector3(foodPos.x, 0.5f, foodPos.y), Quaternion.identity));
    }


    private void DrawBoard()
    {
        // Dibujar el área jugable con suelo
        for (int y = 0; y < size.y; y++)
        {
            for (int x = 0; x < size.x; x++)
            {
                Instantiate(floorPrefab, new Vector3(x, -1, y), Quaternion.identity, this.transform);
            }
        }

        // Dibujar las paredes externas
        for (int y = -1; y <= size.y; y++)
        {
            for (int x = -1; x <= size.x; x++)
            {
                if (x == -1 || x == size.x || y == -1 || y == size.y)
                {
                    Instantiate(wallPrefab, new Vector3(x, 0, y), Quaternion.identity, this.transform);
                }
            }
        }
    }

    public void SetCellData(Vector2Int position, CellData data)
    {
        grid[position] = data;
    }

    public CellData GetCellData(Vector2Int position)
    {
        return grid[position];
    }

    public void DestroyGridObject(Vector2Int position)
    {
        if (grid[position].gameObject != null)
        {
            Destroy(grid[position].gameObject); // Eliminar el objeto físico
        }
    }

    public void SpawnFoodAtRandomPosition()
    {
        Vector2Int foodPos;
        GameObject foodInstance;

        do
        {
            foodPos = new Vector2Int(random.Next(0, size.x), random.Next(0, size.y));
        } while (grid[foodPos].cellType != CellType.Empty); // Asegúrate de que la comida no se genere en una posición ocupada

        // Crear y colocar la comida en la nueva posición
        foodInstance = Instantiate(foodPrefab, new Vector3(foodPos.x, 0.5f, foodPos.y), Quaternion.identity);
        grid[foodPos] = new CellData(CellType.Food, foodInstance); // Asignar la comida al grid

        SetCellType(foodPos, CellType.Food); // Actualizar la celda como comida
    }

    // Public access to the grid
    public CellType GetCellType(Vector2Int position)
    {
        // Retorna el tipo de celda dentro de CellData
        return grid[position].cellType;
    }

    public void SetCellType(Vector2Int position, CellType type)
    {
        // Cambia el tipo de celda, manteniendo el GameObject
        CellData currentCell = grid[position];
        grid[position] = new CellData(type, currentCell.gameObject);
    }

    public Vector2Int GetBoardSize()
    {
        return size;
    }
    public void IncreaseScore()
    {
        score++;
    }
}
