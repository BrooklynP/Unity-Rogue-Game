using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEditor.AI;

public class LevelGenerator : MonoBehaviour
{
    private const int c_iGridWith = 11;
    private const int c_iGridHeight = 11;
    private const int maxAI = 1;

    public Transform StandardRoom4Doors;
    public Transform StandardDoorHorizontal;
    public Transform StandardDoorVertical;
    GameManagerScript GameManagerScript;

    //having 2 seperate lists makes it easier to iterate through rooms during generation in a single dimensional list,
    //but the 2 dimensional list makes it easier to reference which rooms are nearby in the 3D spac
    private List<RoomClass> Rooms = new List<RoomClass>();
    private RoomClass[,] Grid = new RoomClass[c_iGridHeight, c_iGridWith];

    private int iCurrentNumberOfRooms = 0;
    private int iCurrentNumberOfEnemies;

    [SerializeField]
    int iNumberOfRooms; //Number of rooms desired for level.
    [SerializeField]
    Transform LevelHolder; //Store all level objects under one parent for management
    [SerializeField]
    GameObject AI;

    float aiX;
    float aiY;

    //The Position data for the next AI spawned, overwritten every time a new AI is instantiated.
    public float NextAIXCoord
    {
        get { return aiX; }
        set { aiX = value; }
    }
    public float NextAIYCoord
    {
        get { return aiY; }
        set { aiY = value; }
    }


    void Start()
    {
        GameManagerScript = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
        GameObject.Instantiate(StandardRoom4Doors, new Vector3(0, 0, 0), Quaternion.identity, LevelHolder);  //Ensures there will always be a 4 door room in the center to start generation and play from
        Rooms.Add(new RoomClass("open", "open", "open", "open", 0.0f, 0.0f, 5, 5));
        iCurrentNumberOfRooms++;
        iCurrentNumberOfEnemies = 0;

        int iNumberOfIterations = 0; // used to stop loop going on forever if all doors are closed and max room limit hasnt been reached
        while (iCurrentNumberOfRooms < iNumberOfRooms)
        {
            iNumberOfIterations++;
            if (iNumberOfIterations > 50)
            {
                CreateDoors();
                LevelHolder.GetComponentInParent<NavMeshSurface>().BuildNavMesh();
                SpawnAI();
                return;
            }
            for (int room = 0; room < Rooms.Count; room++)
            {
                if (iCurrentNumberOfRooms > iNumberOfRooms)
                {
                    CreateDoors();
                    LevelHolder.GetComponentInParent<NavMeshSurface>().BuildNavMesh();
                    SpawnAI();
                    return;
                }
                if (Rooms[room].GridY + 1 < 11)
                {
                    if (Rooms[room].bHasNorthDoor == "open" && Grid[Rooms[room].GridY + 1, Rooms[room].GridX] == null)
                    {
                        GameObject.Instantiate(StandardRoom4Doors, new Vector3(Rooms[room].fX, 0, Rooms[room].fY + 10), Quaternion.identity, LevelHolder);
                        Rooms.Add(new RoomClass("Unknown", "connected", "Unknown", "Unknown", Rooms[room].fX, Rooms[room].fY + 10, Rooms[room].GridX, Rooms[room].GridY + 1));
                        Rooms[room].bHasNorthDoor = "connected";
                        Grid[Rooms[room].GridY + 1, Rooms[room].GridX] = Rooms[room];
                        iCurrentNumberOfRooms++;
                        //Debug.LogFormat("Room:{0},{1} North Door Connected to Room:{2},{3} South Door", Rooms[room].GridX, Rooms[room].GridY, Rooms[room].GridX, Rooms[room].GridY + 1);
                        //Debug.LogFormat("{0},{1},N:{2},S:{3},E:{4},W:{5}", Rooms[Rooms.Count - 1].GridX, Rooms[Rooms.Count - 1].GridY, Rooms[Rooms.Count - 1].bHasNorthDoor, Rooms[Rooms.Count - 1].bHasSouthDoor, Rooms[Rooms.Count - 1].bHasEastDoor, Rooms[Rooms.Count - 1].bHasWestDoor);
                    }
                }
                if (Rooms[room].GridY - 1 >= 0)
                {
                    if (Rooms[room].bHasSouthDoor == "open" && Grid[Rooms[room].GridY - 1, Rooms[room].GridX] == null)
                    {
                        GameObject.Instantiate(StandardRoom4Doors, new Vector3(Rooms[room].fX, 0, Rooms[room].fY - 10), Quaternion.identity, LevelHolder);
                        Rooms.Add(new RoomClass("connected", "Unknown", "Unknown", "Unknown", Rooms[room].fX, Rooms[room].fY - 10, Rooms[room].GridX, Rooms[room].GridY - 1));
                        Rooms[room].bHasSouthDoor = "connected";
                        Grid[Rooms[room].GridY - 1, Rooms[room].GridX] = Rooms[room];
                        iCurrentNumberOfRooms++;
                        //Debug.LogFormat("Room:{0},{1} South Door Connected to Room:{2},{3} North Door", Rooms[room].GridX, Rooms[room].GridY, Rooms[room].GridX, Rooms[room].GridY - 1);
                        //Debug.LogFormat("{0},{1},N:{2},S:{3},E:{4},W:{5}", Rooms[Rooms.Count - 1].GridX, Rooms[Rooms.Count - 1].GridY, Rooms[Rooms.Count - 1].bHasNorthDoor, Rooms[Rooms.Count - 1].bHasSouthDoor, Rooms[Rooms.Count - 1].bHasEastDoor, Rooms[Rooms.Count - 1].bHasWestDoor);
                    }
                }
                if (Rooms[room].GridX + 1 < 11)
                {
                    if (Rooms[room].bHasEastDoor == "open" && Grid[Rooms[room].GridY, Rooms[room].GridX + 1] == null)
                    {
                        GameObject.Instantiate(StandardRoom4Doors, new Vector3(Rooms[room].fX + 10, 0, Rooms[room].fY), Quaternion.identity, LevelHolder);
                        Rooms.Add(new RoomClass("Unknown", "Unknown", "connected", "Unknown", Rooms[room].fX + 10, Rooms[room].fY, Rooms[room].GridX + 1, Rooms[room].GridY));
                        Rooms[room].bHasEastDoor = "connected";
                        Grid[Rooms[room].GridY, Rooms[room].GridX + 1] = Rooms[room];
                        iCurrentNumberOfRooms++;
                        //Debug.LogFormat("Room:{0},{1} East Door Connected to Room:{2},{3} West Door", Rooms[room].GridX, Rooms[room].GridY, Rooms[room].GridX + 1, Rooms[room].GridY);
                        //Debug.LogFormat("{0},{1},N:{2},S:{3},E:{4},W:{5}", Rooms[Rooms.Count - 1].GridX, Rooms[Rooms.Count - 1].GridY, Rooms[Rooms.Count - 1].bHasNorthDoor, Rooms[Rooms.Count - 1].bHasSouthDoor, Rooms[Rooms.Count - 1].bHasEastDoor, Rooms[Rooms.Count - 1].bHasWestDoor);
                    }
                }
                if (Rooms[room].GridX - 1 >= 0)
                {
                    if (Rooms[room].bHasWestDoor == "open" && Grid[Rooms[room].GridY, Rooms[room].GridX - 1] == null)
                    {
                        GameObject.Instantiate(StandardRoom4Doors, new Vector3(Rooms[room].fX - 10, 0, Rooms[room].fY), Quaternion.identity, LevelHolder);
                        Rooms.Add(new RoomClass("Unknown", "Unknown", "Unknown", "connected", Rooms[room].fX - 10, Rooms[room].fY, Rooms[room].GridX - 1, Rooms[room].GridY));
                        Rooms[room].bHasWestDoor = "connected";
                        Grid[Rooms[room].GridY, Rooms[room].GridX - 1] = Rooms[room];
                        iCurrentNumberOfRooms++;
                        //Debug.LogFormat("Room:{0},{1} West Door Connected to Room:{2},{3} East Door", Rooms[room].GridX, Rooms[room].GridY, Rooms[room].GridX - 1, Rooms[room].GridY);
                        //Debug.LogFormat("{0},{1},N:{2},S:{3},E:{4},W:{5}", Rooms[Rooms.Count - 1].GridX, Rooms[Rooms.Count - 1].GridY, Rooms[Rooms.Count - 1].bHasNorthDoor, Rooms[Rooms.Count - 1].bHasSouthDoor, Rooms[Rooms.Count - 1].bHasEastDoor, Rooms[Rooms.Count - 1].bHasWestDoor);
                    }
                }
            }
        }
    }
    private void CreateDoors()
    {
        foreach (RoomClass room in Rooms)
        {
            //Debug.LogFormat("{0},{1},N:{2},S:{3},E:{4},W:{5}", room.GridX, room.GridY, room.bHasNorthDoor, room.bHasSouthDoor, room.bHasEastDoor, room.bHasWestDoor);
            //fills in open doors that havent had rooms attached so that they become straight walls
            if (room.bHasNorthDoor == "open" || room.bHasNorthDoor == "closed")
            {
                GameObject.Instantiate(StandardDoorHorizontal, new Vector3(room.fX, 2.5f, room.fY + 4.5f), Quaternion.identity, LevelHolder);
            }
            if (room.bHasSouthDoor == "open" || room.bHasSouthDoor == "closed")
            {
                GameObject.Instantiate(StandardDoorHorizontal, new Vector3(room.fX, 2.5f, room.fY - 4.5f), Quaternion.identity, LevelHolder);
            }
            if (room.bHasEastDoor == "open" || room.bHasEastDoor == "closed")
            {
                GameObject.Instantiate(StandardDoorVertical, new Vector3(room.fX + 4.5f, 2.5f, room.fY), Quaternion.identity, LevelHolder);
            }
            if (room.bHasWestDoor == "open" || room.bHasWestDoor == "closed")
            {
                GameObject.Instantiate(StandardDoorVertical, new Vector3(room.fX - 4.5f, 2.5f, room.fY), Quaternion.identity, LevelHolder);
            }
        }
    }
    private void SpawnAI()
    {
        while (iCurrentNumberOfEnemies < maxAI)
        {
            //stores a random position in a seperate public file so that the enemy AI class can acsess it to positon itself.
            Instantiate(AI);
            iCurrentNumberOfEnemies++;
            GameManagerScript.IncreaseEnemyCount();
        }
    }
    public Vector3 ReturnRandomVector()
    {
        Vector3 Position;

        int RandomIndex = Random.Range(1, Rooms.Count);
        NextAIXCoord = Rooms[RandomIndex].fX;
        NextAIYCoord = Rooms[RandomIndex].fY;

        Position = new Vector3(NextAIXCoord, 0, NextAIYCoord);

        return Position;
    }
}
