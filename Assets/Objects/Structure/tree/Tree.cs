using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tree : MonoBehaviour {

    private List<GameObject> _Branches = new List<GameObject>();
    public bool HasBranches
    {
        get
        {
            return _Branches.Count > 0;
        }
    }
    public void CutBranches()
    {
        foreach (GameObject _b in _Branches)
            Destroy(_b);
        _Branches.Clear();
        StartCoroutine(GrowBranches());
    }
    void Start()
    {
        StartCoroutine(GrowBranches());
    }
    private void CreateBranches()
    {
        for (int i = 0; i < Random.Range(1, 5); i++)
        {
            GameObject _b = new GameObject();
            _b.transform.SetParent(gameObject.transform, false);
            _Branches.Add(_b);
            LineRenderer _lr = _b.AddComponent<LineRenderer>();
            _lr.endColor = Color.black;
            _lr.startWidth = 0.15f;
            _lr.endWidth = 0.10f;
            _lr.material = gameObject.transform.GetChild(0).GetComponent<Renderer>().material;
            _lr.SetPosition(0, gameObject.transform.position + Vector3.up * (1.5f + Random.Range(-0.3f, 0.3f)));
            _lr.SetPosition(1, gameObject.transform.position + Vector3.up * (1.5f + Random.Range(-0.3f, 0.3f)) + Random.onUnitSphere);
        }
    }
    private IEnumerator GrowBranches()
    {
        yield return new WaitForSeconds(Settings.BranchesGrowTime + Random.Range(0,Settings.BranchesGrowTime/2));
        this.CreateBranches();
    }
}
