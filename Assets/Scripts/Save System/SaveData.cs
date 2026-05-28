using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SaveData
{
    public string currentScene;

    public SerializableVector3 playerPosition;

    public int coins;
    public int deaths;

    public List<string> triggers = new List<string>();
    public List<string> openedDoors = new List<string>();
    public List<string> collectedItems = new List<string>();
}