using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine;
//using UnityEngine.j
public static class SaveLoad
{
    //private SaveLoadInstance _inst = new SaveLoadInstance();
    //public static List<Game> SavedGames { get { return _inst.SavedGames; } }
    ////Сохранение карты мира
    //private static List<Map> SavedMaps = new List<Map>(); //Список сохраенных карт
    ////Сохранение игры
    //public static List<Game> SavedGames = new List<Game>(); //Список сохраненных игр
    public static void Save()
    {
        SaveLoadInstance _inst = LoadAllGames();
        if (_inst.SavedGames.Find(x => x.GetId() == Settings.CurrentGame.GetId()) != null)//Если такая карта уже сохранена
            _inst.SavedGames[_inst.SavedGames.FindIndex(x => x.GetId() == Settings.CurrentGame.GetId())] = Settings.CurrentGame;
        else
            _inst.SavedGames.Add(Settings.CurrentGame);
        //BinaryFormatter bf = new BinaryFormatter();
        string json = JsonUtility.ToJson(_inst);
        FileStream file = File.Create(Application.persistentDataPath + Settings.GameSavingPath);
        StreamWriter write = new StreamWriter(file);
        Debug.Log(json);
        write.Write(json);
        write.Close();
        //bf.Serialize(file, SaveLoad.SavedGames);
        file.Close();
        Debug.Log("Game " + Settings.CurrentGame.GetId() + " saved" + _inst.SavedGames);
        //SaveMap();
    }
    public static SaveLoadInstance LoadAllGames()
    {
        if (File.Exists(Application.persistentDataPath + Settings.GameSavingPath))
        {
            //BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + Settings.GameSavingPath, FileMode.Open);
            //SaveLoad.SavedGames = (List<Game>)bf.Deserialize(file);
            StreamReader read = new StreamReader(file);
            string json = read.ReadToEnd();
            Debug.Log(typeof(string).Assembly.ImageRuntimeVersion);
            Debug.Log(json);
            Debug.Log(JsonUtility.FromJson<SaveLoadInstance>(json).SavedGames.Count);
            read.Close();
            file.Close();
            return JsonUtility.FromJson<SaveLoadInstance>(json);
        }
        else return new SaveLoadInstance();
    }
    public static void LoadGame(int GameIndex)
    {
        SaveLoadInstance _inst = new SaveLoadInstance();
        Settings.CurrentGame = _inst.SavedGames[GameIndex];
        LoadAllMaps();
        Map.CurrentMap = _inst.SavedMaps[GameIndex];
        _inst.SavedMaps.Clear();
        _inst.SavedGames.Clear();
    }
    public static void SaveMap()
    {
        SaveLoadInstance _inst = new SaveLoadInstance();
        if (_inst.SavedMaps.Find(x => x.MapName == Map.CurrentMap.MapName) != null)//Если такая карта уже сохранена
            _inst.SavedMaps[_inst.SavedMaps.FindIndex(x => x.MapName == Map.CurrentMap.MapName)] = Map.CurrentMap; //Пересохраняем
        else
            _inst.SavedMaps.Add(Map.CurrentMap);
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + Settings.MapSavingPath);
        bf.Serialize(file, _inst.SavedMaps);
        file.Close();
        Debug.Log("Map " + Map.CurrentMap.MapName + " saved");
    }
    public static void LoadAllMaps()
    {
        SaveLoadInstance _inst = new SaveLoadInstance();
        if (File.Exists(Application.persistentDataPath + Settings.MapSavingPath))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + Settings.MapSavingPath, FileMode.Open);
            _inst.SavedMaps = (List<Map>)bf.Deserialize(file);
            file.Close();
        }
    }
    [System.Serializable]
    public class SaveLoadInstance
    {
        //Сохранение карты мира
        public List<Map> SavedMaps = new List<Map>(); //Список сохраенных карт
        //Сохранение игры
        [SerializeField]
        public List<Game> SavedGames = new List<Game>(); //Список сохраненных игр
        public SaveLoadInstance(List<Game> games = null)
        {
            if (games != null)
                SavedGames = games;
            else
                SavedGames = new List<Game>();
        }
    }
}
