using UnityEngine;
using System.Collections;

public class Objective : MonoBehaviour
{

    public string objective = null;
    public int objectiveOrder = 0;

    private bool _objectiveSet = false;

    public string getObjective()
    {
        return objective;
    }

    public int getOrder()
    {
        return objectiveOrder;
    }

    public bool isObjectiveSet()
    {
        return _objectiveSet;
    }

    [RPC]
    public void setObjective()
    {
        _objectiveSet = true;
    }
}
