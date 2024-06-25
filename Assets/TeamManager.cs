using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TeamManager : MonoBehaviour
{
    public bool UseTeams = true;
    public bool FairTeams = false;
    public int TeamCount = 2;
    public List<Team> TeamList = new List<Team>();
    public UnityEvent OnTeamList_Changed;

    private GameManager _manager;

    private void Awake()
    {
        _manager = GetComponent<GameManager>();
        for (int i = 0; i < TeamCount; i++)
        {
            TeamList.Add(new Team("Untitled Team"));
        }
    }

    public int RegisterCharacterToAnyTeam(CharacterController controller)
    {
        int tIndex = 0;
        if (FairTeams)
        {
            int _low = int.MaxValue;
            // find team with lowest amount of characters
            for (int i = 0; i < TeamCount; i++)
            {
                if (TeamList[i].Characters.Count < _low)
                {
                    _low = TeamList[i].Characters.Count;
                    tIndex = i;
                }
            }
        }
        else
        {
            // get random team
            tIndex = Random.Range(0, TeamCount);
        }

        TeamList[tIndex].Characters.Add(controller);
        OnTeamList_Changed.Invoke();

        return tIndex;
    }

    public void RegisterCharacterToSpecificTeam(CharacterController controller, int tIndex)
    {
        TeamList[tIndex].Characters.Add(controller);
        OnTeamList_Changed.Invoke();
    }

    public List<PickupableFlag> GetTargetFlags(int RequestingCharacterTeamIndex)
    {
        List<PickupableFlag> flags = new List<PickupableFlag>();
        foreach(PickupableFlag f in _manager.Flags)
            if (f.OwnedTeamIndex !=  RequestingCharacterTeamIndex)
                flags.Add(f);
        return flags;
    }

    private void Start()
    {
        
    }
}

[SerializeField]
public struct Team
{
    public string Name;
    public Color TeamColour;
    public List<CharacterController> Characters;

    public Team(string name)
    {
        Name = name;
        TeamColour = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
        Characters = new List<CharacterController>();
    }
}

public static class CoolThings
{ 
    public static List<Transform> GetTransformsFromFlags(List<PickupableFlag> Flags)
    {
        List<Transform> transforms = new List<Transform>();
        foreach (PickupableFlag flag in Flags)
            transforms.Add(flag.transform);
        return transforms;
    }
}