using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Game
{
    [SerializeField]
    private string id;
    public string GetId()
    {
        return id;
    }
    public Game()
    {
        id = Guid.NewGuid().ToString();
    }
}