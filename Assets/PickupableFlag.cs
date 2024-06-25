using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PickupableFlag : Pickupable
{
    public UnityEvent<int> OwnedTeamChanged;
    private int _ownedTeamIndex = -1;
    public int OwnedTeamIndex
    {
        get
        {
            return _ownedTeamIndex;
        }
        set
        {
            if (_ownedTeamIndex != value)
            {
                _ownedTeamIndex=value;
                OwnedTeamChanged.Invoke(value);
            }
        }
    }
}
