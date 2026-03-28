using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level4Manager : MonoBehaviour
{
    public GameObject PhaseA;
    public GameObject PhaseB;
    public GameObject PhaseC;
    public GameObject PhaseD;
    public ItemDropNode RiverBlock1;
    public ItemDropNode RiverBlock2;
    private GameObject lastPhase;
    private PathNetwork pathNetwork;

    // Start is called before the first frame update
    void Start()
    {
        lastPhase = PhaseA;

        RiverBlock1.ItemPlaced += TriggerPhaseA;
        RiverBlock1.ItemRemoved += TriggerPhaseD;

        RiverBlock2.ItemPlaced += TriggerPhaseC;
        RiverBlock2.ItemRemoved += TriggerPhaseD;

        pathNetwork = PathNetwork.Instance;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void TogglePath(string pathName, bool status) {
        pathNetwork.SetPathFutureTraversable(pathNetwork.GetNamedPath(pathName), status);
        pathNetwork.SetPathPastTraversable(pathNetwork.GetNamedPath(pathName), status);
    }

    void TriggerPhaseD() {
        lastPhase.SetActive(false);
        PhaseD.SetActive(true);

        TogglePath("crossing1", false);

        lastPhase = PhaseD;
    }

    void TriggerPhaseC() {
        lastPhase.SetActive(false);
        PhaseC.SetActive(true);

        TogglePath("crossing2", true);

        lastPhase = PhaseC;
    }

    void TriggerPhaseA() {
        lastPhase.SetActive(false);
        PhaseA.SetActive(true);

        TogglePath("crossing1", true);

        lastPhase = PhaseA;
    }
}
