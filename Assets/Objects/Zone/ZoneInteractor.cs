using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoneInteractor : MonoBehaviour {

    private ZoneController.Zone ConnectedZone;

    void Click()
    {
        ConnectedZone.ShowZoneDetails();
    }

    void UnPick()
    {
        Links.Interface.HideInfopanel();
    }

    public void Connect(ZoneController.Zone _z)
    {
        ConnectedZone = _z;
    }
}
