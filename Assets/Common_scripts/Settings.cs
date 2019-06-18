using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Settings : MonoBehaviour {

    //public int _WorldHeight;
    public GameObject _BrickPrefab, _UnitPrefab, _TreePrefab, _PlanePrefab,_ZoneRodPrefab,_DigTool,_CraftTool,_PunchTool;
    public Font _MenuTextFont;
    public Material _Material;

    //Parameters
    public static int WorldWidth { get; private set; }
    //public static int WorldHeight { get; private set; }
    public static int WorldLength { get; private set; }
    public static string WorkbenchesStoragePath { get; private set; }
    public static string BuildingsStoragePath { get; private set; }
    public static string TexturesFolderPath { get; private set; }
    public static List<string> ComplexObjectsTags { get; private set; }
    public static string GameSavingPath { get { return "/SavedGames/Game.svg"; } }
    public static string MapSavingPath { get { return "/SavedGames/Map.svg"; } }
    
    public static byte KernelSize { get; private set; }
    public static byte MapBlurParam { get; private set; }
    public static float MapBlurCoeffitient { get; private set; }
    public static float BranchesGrowTime { get; private set; }
    public static byte MapHeight { get; private set; }
    public static byte MapSoilLevel { get; private set; }
    public static int WaterLevel { get; private set; }
    public static byte MapLandscapeScale { get; private set; }
    public static int BasicMapRadius { get; private set; }
    public static int MaxMapRadius { get; private set; }
    public static int NoizeShift { get; private set; }
    public static Font MenuTextFont { get; private set; }
    public static Material ResourceBasicMaterial { get; private set; }

    //Constants
    public static List<Vector3> Vector3Neighbours { get; private set; }
    public static List<Vector2> Vector2Neighbours { get; private set; }

    //Objects
    public static GameObject BrickPrefab { get; private set; }
    public static GameObject UnitPrefab { get; private set; }
    public static GameObject TreePrefab { get; private set; }
    public static GameObject PlanePrefab { get; private set; }
    public static GameObject ZoneRod { get; private set; }
    public static GameObject DigToolPrefab { get; private set; }
    public static GameObject CraftToolPrefab { get; private set; }
    public static GameObject PunchToolPrefab { get; private set; }
    
    //Technical
    private bool ParamsNotSet = true;
    private string scr = "Settings";
    public static GameLaunchMode LaunchMode = GameLaunchMode.NewGame;
    public static Game CurrentGame;

    public void SetParams()
    {
        if (ParamsNotSet)
        {
            //Debug.Log("Unit type fullname is " + typeof(Unit).FullName);
            //Debug.Log("Tree type fullname is " + typeof(Tree).FullName);
            //WorldWidth = _WorldWidth;
            //WorldHeight = _WorldHeight;
            //WorldLength = _WorldLength;
            BrickPrefab = _BrickPrefab;
            UnitPrefab = _UnitPrefab;
            TreePrefab = _TreePrefab;
            DigToolPrefab = _DigTool;
            CraftToolPrefab = _CraftTool;
            PunchToolPrefab = _PunchTool;
            ZoneRod = Instantiate(_ZoneRodPrefab);
            ZoneRod.SetActive(false);
            WorkbenchesStoragePath = "C:/GD/You and world/v.0.0.1/First/Configs/Workbenches.xml";
            BuildingsStoragePath = "C:/GD/You and world/v.0.0.1/First/Configs/Buildings.xml";
            TexturesFolderPath = "C:/GD/GDart/Textures/Resources/";
            ComplexObjectsTags = new List<string>(){"WorkbenchBrick","BuildingBrick","Tree","Unit"};
            Vector3Neighbours = new List<Vector3>() { Vector3.up, Vector3.down, Vector3.left, Vector3.right, Vector3.forward, Vector3.back };
            Vector2Neighbours = new List<Vector2>() { Vector2.left, Vector2.right, Vector2.up, Vector2.down };
            BranchesGrowTime = 15f;
            KernelSize = 27;
            MapBlurParam = 0;
            MapBlurCoeffitient = 0.06f;
            MapHeight = 20;
            WaterLevel = 5;
            MapSoilLevel = 18;
            MapLandscapeScale = 1;
            BasicMapRadius = 1;
            MaxMapRadius = 1000;
            NoizeShift = UnityEngine.Random.Range(0, MaxMapRadius * KernelSize);
            MenuTextFont = _MenuTextFont;
            ResourceBasicMaterial = _Material;

            Log.Notice(scr,"Settings set");
            ZoneController.Initialize();
            Resources.UploadResources();
            Workbenches.UploadTemplates();
            Structures.Initialize();
            //Links.WorldBuilder.BuildTheWorld();
            if (LaunchMode == GameLaunchMode.NewGame)
            {
                CurrentGame = new Game();
                StartCoroutine(new Map().InitializeMap());
            }
            else
                Debug.Log("Game is loading");
            //foreach (Kernel _k in Map.Kernels)
            //    StartCoroutine(MapGenerator.LandscapeGenerator(_k));
        }
        else
            Debug.LogError("Settings are already set!");
    }

    //Subclasses
    public class ResourceGeneration
    {
        /// <summary>
        /// Количество залежей ресурсов на один стержень
        /// </summary>
        public static int DepositsQuantityParameter { get {return UnityEngine.Random.Range(9,15);}}
        //public static List<ushort> ResorucesForWorldBuilding { get { return new List<ushort>() { 1, 2, 4, 5 }; }}
    }
}
public enum GameLaunchMode {NewGame,LoadGame}