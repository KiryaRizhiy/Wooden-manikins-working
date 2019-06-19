using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour {

    public bool InfopanelRised { get { return Infopanel.GetComponent<RectTransform>().rect.height < UISettings.InfopanelHeight; } }

    //Выпадашки строительного процесса. Динамически наполняются типами ресурсов, станков, зданий
    public Dropdown StructureCreationPanelResourceSelect { get { return StructureCreationPanelGameObject.transform.GetChild(1).GetComponent<Dropdown>(); } }
    public Dropdown StructureCreationPanelWorkbenchSelect { get { return StructureCreationPanelGameObject.transform.GetChild(4).GetComponent<Dropdown>(); } }
    public Dropdown StructurePlacingPanelBuildingSelect { get { return StructurePlacingPanelGameObject.transform.GetChild(0).GetComponent<Dropdown>(); } }

    //Подпанели и контролы модели процессов
    public GameObject ProcessCreationProductionSubpanel {get {return ProcessCreationPanelGameObject.transform.GetChild(1).GetChild(0).gameObject;}}
    public GameObject ProcessCreationMiningSubpanel {get {return ProcessCreationPanelGameObject.transform.GetChild(1).GetChild(1).gameObject;}}
    public GameObject ProcessCreationGatheringSubpanel {get {return ProcessCreationPanelGameObject.transform.GetChild(1).GetChild(2).gameObject;}}
    public Dropdown ProcessCreationProcessTypeSelect {get {return ProcessCreationPanelGameObject.transform.GetChild(0).GetComponent<Dropdown>();}}

    public Dropdown ProcessCreationProductionResourceSelect { get { return ProcessCreationProductionSubpanel.transform.GetChild(0).GetComponent<Dropdown>(); } }
    public Input ProcessCreationProductionLimitInput { get { return ProcessCreationProductionSubpanel.transform.GetChild(1).GetComponent<Input>(); } }

    public Dropdown ProcessCreationMiningMineSelect { get { return ProcessCreationMiningSubpanel.transform.GetChild(0).GetComponent<Dropdown>(); } }
    public Dropdown ProcessCreationMiningStoreSelect { get { return ProcessCreationMiningSubpanel.transform.GetChild(1).GetComponent<Dropdown>(); } }

    public Dropdown ProcessCreationGatheringZoneSelect { get { return ProcessCreationGatheringSubpanel.transform.GetChild(0).GetComponent<Dropdown>(); } }
    public InputField ProcessCreationGatheringLimitInput { get { return ProcessCreationGatheringSubpanel.transform.GetChild(1).GetComponent<InputField>(); } }
    public Dropdown ProcessCreationGatheringStoreSelect { get { return ProcessCreationGatheringSubpanel.transform.GetChild(2).GetComponent<Dropdown>(); } }

    //Выбор типа зоны
    public Dropdown ZoneTypeSelect { get { return ZoneCreationPanelGameObject.transform.GetChild(0).GetComponent<Dropdown>(); } }

    //Основные объекты игрового меню
    private static string scr = "UI";
    private GameObject MainMenuPanelGameObject { get { return Functions.GetAllChildren(Links.MainCanvas.gameObject)[0]; } }
    private GameObject StructureCreationPanelGameObject { get { return Functions.GetAllChildren(Links.MainCanvas.gameObject)[1]; } }
    private GameObject ProcessCreationPanelGameObject { get { return Functions.GetAllChildren(Links.MainCanvas.gameObject)[2]; } }
    private GameObject Infopanel { get { return Functions.GetAllChildren(Links.MainCanvas.gameObject)[3]; } }
    private GameObject StructurePlacingPanelGameObject { get { return Functions.GetAllChildren(Links.MainCanvas.gameObject)[4]; } }
    private GameObject ZoneCreationPanelGameObject { get { return Functions.GetAllChildren(Links.MainCanvas.gameObject)[5]; } }

    //Выпадашки главного меню
    private Dropdown StructureMenuSectionSelect { get { return Functions.GetAllChildren(MainMenuPanelGameObject)[0].GetComponent<Dropdown>(); } }
    private Dropdown ProcessesMenuSectionSelect { get { return Functions.GetAllChildren(MainMenuPanelGameObject)[1].GetComponent<Dropdown>(); } }
    private Dropdown MilitaryMenuSectionSelect { get { return Functions.GetAllChildren(MainMenuPanelGameObject)[2].GetComponent<Dropdown>(); } }
    private Dropdown ZoneMenuSectionSelect { get { return Functions.GetAllChildren(MainMenuPanelGameObject)[3].GetComponent<Dropdown>(); } }
    private Dropdown ProfessionMenuSectionSelect { get { return Functions.GetAllChildren(MainMenuPanelGameObject)[4].GetComponent<Dropdown>(); } }
    private GameObject MapExpandButton { get { return Functions.GetAllChildren(MainMenuPanelGameObject)[5]; } }

    private UIInfoFrame Frame;
    private Player_control_script _PlayerControl { get { return GetComponent<Player_control_script>(); } }
    private int MaxValueCount_Crutch = -1;

    //Оперативка фронта
    private Vector3 CoordinatesForMapExpansion;

    //Handles
    private void RiseInfopanel()
    {
        StartCoroutine(_RiseInfopanel());
    }
    public void HideInfopanel()
    {
        StartCoroutine(_HideInfopanel());
    }
    public void ShowSack(Sack Content)
    {
        StartCoroutine(_ShowSack(Content));
    }
    public void ShowInfo(string Header, object Value)
    {
        GameObject _go = new GameObject();
        _go.transform.SetParent(Infopanel.transform,false);
        UIInfoComponent _uic = _go.AddComponent<UIInfoComponent>();
        _uic.Content = new NameValue(Header,Value.ToString());
        _uic.Position = new Vector2(50, 50);
        _uic.Size = new Vector2(50, 200);
    }
    public void ClearInfopanel()
    {
        foreach (GameObject _o in Functions.GetAllChildren(Infopanel))
            Destroy(_o);
    }
    public void SwitchToStructureMenu(int Value)
    {
        if (Value != MaxValueCount_Crutch)
        {
            StructureMenuSectionSelect.Hide();
            switch (Value)
            {
                case 0://Создание нового здания
                    _PlayerControl._PopupMode = true;//Writing the code on the seashore in the Sudak towns, Chaikhana hotel
                    MainMenuPanelGameObject.SetActive(false);
                    StructureCreationPanelGameObject.SetActive(true);
                    Links.Builder.BuildingModeOn(Links.MainCamera.transform.position + 6 * Vector3.Normalize(Links.MainCamera.transform.forward));
                    break;
                case 1:
                    _PlayerControl.UpdateBuildingsList();
                    MainMenuPanelGameObject.SetActive(false);
                    StructurePlacingPanelGameObject.SetActive(true);
                    break;
                default:
                    Log.Warning(scr, "Trying to open unknown structure menu section " + Value);
                    break;
            }
            MaxValueCount_Crutch = StructureMenuSectionSelect.options.Count;
            StructureMenuSectionSelect.options.Add(new Dropdown.OptionData("Exit"));
            StructureMenuSectionSelect.value = MaxValueCount_Crutch;
            StructureMenuSectionSelect.options.RemoveAt(MaxValueCount_Crutch);
            MaxValueCount_Crutch = -1;
        }
    }
    public void SwitchToProcessMenu(int Value)
    {
        if (Value != MaxValueCount_Crutch)
        {
            ProcessesMenuSectionSelect.Hide();
            switch (Value)
            {
                case 0://Создание нового процесса
                    _PlayerControl._PopupMode = true;
                    MainMenuPanelGameObject.SetActive(false);
                    ProcessCreationPanelGameObject.SetActive(true);
                    Links.Processes.ValueChanged(0);
                    break;
                default:
                    Log.Warning(scr, "Trying to open unknown process menu section " + Value);
                    break;
            }
            MaxValueCount_Crutch = ProcessesMenuSectionSelect.options.Count;
            ProcessesMenuSectionSelect.options.Add(new Dropdown.OptionData("Exit"));
            ProcessesMenuSectionSelect.value = MaxValueCount_Crutch;
            ProcessesMenuSectionSelect.options.RemoveAt(MaxValueCount_Crutch);
            MaxValueCount_Crutch = -1;
        }
    }
    public void SwitchToZoneMenu(int Value)
    {
        if (Value != MaxValueCount_Crutch)
        {
            ZoneMenuSectionSelect.Hide();
            switch (Value)
            {
                case 0://Создание нового процесса
                    MainMenuPanelGameObject.SetActive(false);
                    ZoneCreationPanelGameObject.SetActive(true);
                    break;
                default:
                    Log.Warning(scr, "Trying to open unknown zone menu section " + Value);
                    break;
            }
            MaxValueCount_Crutch = ZoneMenuSectionSelect.options.Count;
            ZoneMenuSectionSelect.options.Add(new Dropdown.OptionData("Exit"));
            ZoneMenuSectionSelect.value = MaxValueCount_Crutch;
            ZoneMenuSectionSelect.options.RemoveAt(MaxValueCount_Crutch);
            MaxValueCount_Crutch = -1;
        }
    }
    public void SwitchToMainMenu()
    {
        _PlayerControl._PopupMode = false;
        StructureCreationPanelGameObject.SetActive(false);
        StructurePlacingPanelGameObject.SetActive(false);
        ProcessCreationPanelGameObject.SetActive(false);
        ZoneCreationPanelGameObject.SetActive(false);
        MainMenuPanelGameObject.SetActive(true);
        ProcessesMenuSectionSelect.Hide();
        StructureMenuSectionSelect.Hide();
    }
    public void ShowExpandButton(Vector3 FutureExpansionCoordinates)
    {
        CoordinatesForMapExpansion = FutureExpansionCoordinates;
        MapExpandButton.SetActive(true);
        StartCoroutine(_HideExpandButton());
    }
    public void ExpandMap()
    {
        Map.AddKernel(CoordinatesForMapExpansion);
    }

    //Engine
    private IEnumerator _ShowSack(Sack Content)
    {
        yield return StartCoroutine(_RiseInfopanel());
        Frame = new UIInfoFrame(Infopanel, Content);
    }
    private IEnumerator _RiseInfopanel()
    {
        Log.Notice(scr, "Start rolling up infopanel");
        RectTransform _InfopanelRectTransform = Infopanel.GetComponent<RectTransform>();
        while (_InfopanelRectTransform.sizeDelta.y < UISettings.InfopanelHeight)
        {
            _InfopanelRectTransform.sizeDelta += Vector2.up*Time.deltaTime * UISettings.InfopanelScrollSpeed;
            _InfopanelRectTransform.anchoredPosition += Vector2.up * Time.deltaTime * UISettings.InfopanelScrollSpeed / 2;
            yield return new WaitForEndOfFrame();
        }
        if (_InfopanelRectTransform.sizeDelta.y > UISettings.InfopanelHeight)
        {
            _InfopanelRectTransform.sizeDelta = Vector2.up * UISettings.InfopanelHeight;
            _InfopanelRectTransform.anchoredPosition = Vector2.up * UISettings.InfopanelHeight / 2;
        }
        Log.Notice(scr, "Infopannel rolled up");
    }
    private IEnumerator _HideInfopanel()
    {
        Log.Notice(scr, "Start rolling back infopanel");
        Frame.Destroy();
        RectTransform _InfopanelRectTransform = Infopanel.GetComponent<RectTransform>();
        while (_InfopanelRectTransform.rect.height >0 )
        {
            _InfopanelRectTransform.sizeDelta -= Vector2.up * Time.deltaTime * UISettings.InfopanelScrollSpeed;
            _InfopanelRectTransform.anchoredPosition -= Vector2.up * Time.deltaTime * UISettings.InfopanelScrollSpeed / 2;
            yield return new WaitForEndOfFrame();
        }
        if (_InfopanelRectTransform.rect.height < 0)
        {
            _InfopanelRectTransform.sizeDelta = Vector2.zero;
            _InfopanelRectTransform.anchoredPosition = Vector2.zero;
        }
        Log.Notice(scr, "Infopannel rolled back");
    }
    private IEnumerator _HideExpandButton()
    {
        yield return new WaitForSeconds(5f);
        MapExpandButton.SetActive(false);
    }
}