using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogSettings : MonoBehaviour {

    public bool Unit;
    public bool UnitActions;
    public bool UnitRoute;
    public bool UnitRouteWalking;
    public bool UnitActionsDetails;
    public bool UnitParameters;
    public bool PlayerControlScript;
    public bool PlayerControlScriptMouseClicks;
    public bool ProcessManager;
    public bool ProcessManagerSteps;
    public bool ProcessManagerUI;
    public bool ProcessManagerActionConstructor;
    public bool ProcessManagerActionConstructorDetails;

    public bool Basic;
    public bool ZoneController;
    public bool PhantomConstruction;
    public bool Resources;
    public bool ResourcesDetails;
    public bool Sack;
    public bool Brick;

    public bool Default { get { return true; } }

    public bool CameraMoveScript;
    public bool WorldBuilder;
    public bool LightScript;
    public bool UI;
    public bool UIInfoFrame;

    public bool Buildings;
    public bool Settings;
    public bool Links;
    public bool Functions;
    public bool Kernel;
    public bool Map;
    public bool MapGenerator;
}
