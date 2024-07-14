using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class EnemyBrain : MonoBehaviour
{
    private EnemyController controller;
    private VelocityRotator velocityRotator;
    private TeamManager _teamManager;

    private int TeamIndex { get { return controller.TeamIndex; } }


    public float OpponentSpottingDistance = 15f;


    public enum EnemyStatus
    {
        Idle, GettingFlag, AttackingOpponents, DefendingAllyFlagHolder
    }

    public EnemyStatus Status = EnemyStatus.Idle;

    private Transform CurrentEnemyTarget = null;

    private void Awake()
    {
        controller = GetComponent<EnemyController>();
        velocityRotator = GetComponent<VelocityRotator>();
        _teamManager = FindAnyObjectByType<TeamManager>();
    }

    // Start is called before the first frame update
    void Start()
    {
        Status = EnemyStatus.GettingFlag;
    }

    // Update is called once per frame
    void Update()
    {
        bool enableVelocityRotator = true;
        

        switch (Status)
        {
            case EnemyStatus.Idle:
                // do nothing?
                break;
            case EnemyStatus.GettingFlag:
                // all enemies begin with this
                // can then attack opponents if an opponent appears within range
                // and can become defending ally flag holder if an ally picks up flag
                if (CheckForNearbyOpponents(out _))
                    Status = EnemyStatus.AttackingOpponents;
                break;
            case EnemyStatus.AttackingOpponents:
                // attack nearest enemy until it dies/gets out of range
                // return to getting flag
                if (CurrentEnemyTarget == null)
                {
                    List<Transform> nearby;
                    if (CheckForNearbyOpponents(out nearby))
                    {
                        // find nearest enemy
                        // TODO: Implement finding nearest one that can be shot
                        Transform nearest = nearby[0];
                        float nearestDistance = (nearby[0].position - transform.position).magnitude;
                        foreach (Transform opp in nearby)
                        {
                            float dist = (opp.position - transform.position).magnitude;
                            if (dist < nearestDistance)
                            {
                                nearestDistance = dist;
                                nearest = opp;
                            }
                        }
                        CurrentEnemyTarget = nearest;
                    }
                    else
                    {
                        // could not find any nearby opponents, move back to getting flag
                        Status = EnemyStatus.GettingFlag;
                    }
                }
                else
                {
                    // check can see the enemy?
                    // if can, aim and shoot (disable the velocity rotator)
                    // TODO: Should make it so each update has bool like 'disableRotator' which gets changed to true in here and at end of update function it'll apply that. therefore don't need to worry about re-enabling it. it'll automatically enable once not disabling it
                    // else get closer
                    RaycastHit hit;
                    if (Physics.Raycast(transform.position, (CurrentEnemyTarget.position - transform.position).normalized, out hit, OpponentSpottingDistance))
                    {
                        if (hit.transform == CurrentEnemyTarget || hit.transform.parent.parent == CurrentEnemyTarget)
                        {
                            enableVelocityRotator = false;
                            // TODO: Aim at player
                            float angle = Vector3.SignedAngle(transform.forward, (CurrentEnemyTarget.position - transform.position).normalized, transform.up);
                            transform.Rotate(transform.up, angle * Time.deltaTime * 4);
                        }
                    }
                    else
                    {
                        // TODO: Get closer to opponent
                    }
                }
                break;
            case EnemyStatus.DefendingAllyFlagHolder:
                // come from getting flag
                // return to getting flag once flag is no longer held by ally
                break;
        }
        velocityRotator.enabled = enableVelocityRotator;
    }

    private bool CheckForNearbyOpponents(out List<Transform> OpponentsInRange)
    {
        List<Transform> opponents = _teamManager.GetAllOpponents(TeamIndex);
        List<Transform> opponentsInRange = new List<Transform>();
        foreach(Transform t in opponents)
        {
            if (Mathf.Abs((t.position - transform.position).magnitude) < OpponentSpottingDistance)
            { opponentsInRange.Add(t);}
        }
        OpponentsInRange = opponentsInRange;
        return OpponentsInRange.Count > 0;
    }
}
