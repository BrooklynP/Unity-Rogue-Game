using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomClass {
    public RoomClass(string a_bHasNorthDoor, string a_bHasSouthDoor, string a_bHasWestDoor, string a_bHasEastDoor, float a_fX, float a_fY, int a_iGridX, int a_iGridY)  
        // "open means there is a door present without a room. connected means there is a door with a room connected. unknown means the code still needs to decide if an open door should be created
        //closed means that there should be a wall
    {
        #region HasDoorProbability
        //decides if undecided walls should have doors or not.
        if (a_bHasNorthDoor == "Unknown")
        {
            int iProbability = 1;
            if (Random.Range(0, DoorProbablityConstant) == iProbability)
            {
                bHasNorthDoor = "open";
            }
            else
            {
                bHasNorthDoor = "closed";
            }
        }
        if (a_bHasSouthDoor == "Unknown")
        {
            int iProbability = 1;
            if (Random.Range(0, DoorProbablityConstant) == iProbability)
            {
                bHasSouthDoor = "open";
            }
            else
            {
                bHasSouthDoor = "closed";
            }
        }
        if (a_bHasWestDoor == "Unknown")
        {
            int iProbability = 1;
            if (Random.Range(0, DoorProbablityConstant) == iProbability)
            {
                bHasWestDoor = "open";
            }
            else
            {
                bHasWestDoor = "closed";
            }
        }
        if (a_bHasEastDoor == "Unknown")
        {
            int iProbability = 1;
            if (Random.Range(0, DoorProbablityConstant) == iProbability)
            {
                bHasEastDoor = "open";
            }
            else
            {
                bHasEastDoor = "closed";
            }
        }
        #endregion
        if(a_bHasNorthDoor != "Unknown")
        {
            bHasNorthDoor = a_bHasNorthDoor;
        }
        if (a_bHasSouthDoor != "Unknown")
        {
            bHasSouthDoor = a_bHasSouthDoor;
        }
        if (a_bHasWestDoor != "Unknown")
        {
            bHasWestDoor = a_bHasWestDoor;
        }
        if (a_bHasEastDoor != "Unknown")
        {
            bHasEastDoor = a_bHasEastDoor;
        }
        fX = a_fX;
        fY = a_fY;
        GridX = a_iGridX;
        GridY = a_iGridY;
    }
    public string bHasNorthDoor;
    public string bHasSouthDoor;
    public string bHasWestDoor;
    public string bHasEastDoor;
    public float fX;
    public float fY;
    public int GridX;
    public int GridY;
    private const int DoorProbablityConstant = 3;
}
