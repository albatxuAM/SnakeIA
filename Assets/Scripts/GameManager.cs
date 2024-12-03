using UnityEngine;
using Random = System.Random;

public class Board : MonoBehaviour
{
    [Header("Configuration")]
    [Space(1)]
    [SerializeField]
    private Camera cam;
    [SerializeField]
    private Vector2Int size;
    [SerializeField]
    int ramdomSeed;

    private Grid2D<CellType> grid;
    private Random random;


    [Header("Debug variables")]
    [Space(1)]
    [SerializeField]
    bool debug = true;
    [SerializeField]
    GameObject cubePrefab;
    [SerializeField]
    Material wallMaterial;
    [SerializeField]
    Material floorMaterial;
    [SerializeField]
    Material snakeMaterial;
    [SerializeField]
    Material appleMaterial;

    private void Start()
    {
        cam.transform.localPosition = new Vector3(size.x / 2, Mathf.Max(size.x, size.y), size.y / 2);
        //cam.transform.LookAt(new Vector3(size.x / 2, 0, size.y / 2));
        random = new Random(ramdomSeed);
        generateBoard();
        drawBoard();
    }

    private void generateBoard()
    {
        grid = new Grid2D<CellType>(size, Vector2Int.zero);

        //initialize board to empty cells
        for (int y = 0; y < size.y; y++)
        {
            for (int x = 0; x < size.x; x++)
            {
                Vector2Int pos = new Vector2Int(x, y);

                grid[pos] = CellType.Empty;

                //PlaceFloor(pos);
            }
        }

        // size - 1 so the snake does not appear in a border
        int randX = random.Next(0, size.x - 1);
        int randY = random.Next(0, size.y - 1);
        Vector2Int snakePos = new Vector2Int(randX, randY);
        grid[snakePos] = CellType.Snake;

        Vector2Int applePos = new Vector2Int(random.Next(0, size.x), random.Next(0, size.y));
        grid[applePos] = CellType.Apple;

        //PlaceSnake(new Vector2Int(randX, randY));
        //apear direction 
        //find nearest wall --> oposite to it

    }

    private void drawBoard()
    {
        for (int y = 0; y < size.y; y++)
        {
            for (int x = 0; x < size.x; x++)
            {
                Vector2Int pos = new Vector2Int(x, y);

                bool isAtMapEdge = pos.x == 0 || pos.x == size.x || pos.y == 0 || pos.y == size.y;

                switch (grid[pos])
                {
                    case CellType.Empty:
                        PlaceFloor(pos);
                        break;
                    case CellType.Apple:
                        PlaceApple(pos);
                        break;
                    case CellType.Snake:
                        PlaceSnake(pos);
                        break;
                }
            }
        }
    }
    void PlaceCube(Vector2Int location, Vector2Int size, Material material, int locationY = 0)
    {
        if (debug)
        {
            GameObject go = Instantiate(cubePrefab, new Vector3(location.x - 0.5f, locationY, location.y - 0.5f), Quaternion.identity);
            go.transform.SetParent(this.transform);
            go.GetComponent<Transform>().localScale = new Vector3(size.x, 1, size.y);
            go.GetComponent<MeshRenderer>().material = material;
        }
    }

    void PlaceFloor(Vector2Int location)
    {
        PlaceCube(location, new Vector2Int(1, 1), floorMaterial, -1);
    }
    private void PlaceWall(Vector2Int location)
    {
        PlaceCube(location, new Vector2Int(1, 1), wallMaterial);
    }
    private void PlaceSnake(Vector2Int location)
    {
        PlaceCube(location, new Vector2Int(1, 1), snakeMaterial);
    }
    private void PlaceApple(Vector2Int location)
    {
        PlaceCube(location, new Vector2Int(1, 1), appleMaterial);
    }
}
