using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    public List<PickupableFlag> Flags = new List<PickupableFlag>();
    public UnityEvent AnyFlagOwnerChanged = new UnityEvent();

    private void Awake()
    {
        Flags.AddRange(FindObjectsByType<PickupableFlag>(FindObjectsSortMode.None));
        foreach (var flag in Flags)
            flag.OwnedTeamChanged.AddListener(FlagChanged);
            
    }

    private void FlagChanged(int newTeam)
    {
        AnyFlagOwnerChanged.Invoke();
    }
}