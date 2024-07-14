using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamRegistering : MonoBehaviour
{
    private void Start()
    {
        FindAnyObjectByType<TeamManager>().RegisterCharacterToSpecificTeam(GetComponent<CharacterController>(), 0);
    }
}
