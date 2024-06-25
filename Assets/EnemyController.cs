using CharacterSystems.Movement;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class EnemyController : MonoBehaviour
{
    private NavMeshAgent _agent;
    private Transform _agentTransform;

    private const float MAX_AGENT_DISTANCE = 3f;

    private List<Transform> _enemyTransforms = new List<Transform>();

    private TeamManager _teamManager;
    private GameManager _gameManager;

    private List<PickupableFlag> _flagTargets = new List<PickupableFlag>();

    private CharacterMovement3D _movement;

    public int TeamIndex { private set; get; }

    private void Awake()
    {
        _teamManager = FindAnyObjectByType<TeamManager>();
        _gameManager = FindAnyObjectByType<GameManager>();
        TeamIndex = 1; 
        _movement = GetComponent<CharacterMovement3D>();
    }

    public float RotateySpeed = 20f;

    // Start is called before the first frame update
    void Start()
    {
        GameObject agentObject = new GameObject("Enemy Navigation Agent");
        _agent = agentObject.AddComponent<NavMeshAgent>();
        _agentTransform = agentObject.transform;
        _teamManager.RegisterCharacterToSpecificTeam(GetComponent<CharacterController>(), TeamIndex);
        _agentTransform.position = transform.position - Vector3.up * -0.5f;
        _agent.speed = 12f;
        _gameManager.AnyFlagOwnerChanged.AddListener(UpdateFlagCache);

        UpdateFlagCache();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 delta = _agentTransform.position - transform.position;
        if (delta.magnitude > MAX_AGENT_DISTANCE)
        {
            _agent.enabled = false;
        }
        else
        {
            _agent.enabled = true;
        }

        // set target:
        // if can see enemy, target enemy
        // else set target as me PC
        Transform enemyTransform = null;
        if (_agent.enabled)
        {
            if (CheckVisibleEnemy(out enemyTransform))// TODO: make this only happen periodically, as it triggers recalculatio of tstuff or wait until it arrive or something :?
            {
                // get target as nearest enemy
            }
            else
            {
                _agent.SetDestination(GetClosestFlag().position);
            }
        }

        Vector3 transformedDelta = transform.InverseTransformPoint(_agentTransform.position);
        transformedDelta.y = 0;

        float forwardsInput = transformedDelta.normalized.z;
        float rightInput = transformedDelta.normalized.x;

        //_movement.Move(new Vector2(rightInput, forwardsInput));
    }

    private bool CheckVisibleEnemy(out Transform enemyTransform)
    {
        enemyTransform = null;
        Debug.LogWarning("Not implemented :(");
        return false;
    }

    private Transform GetClosestFlag()
    {
        float shortestDist = float.MaxValue;
        int closestIndex = 0;
        for (int i = 0; i < _flagTargets.Count; i++)
        {
            float dist = (_flagTargets[i].transform.position - transform.position).magnitude;
            if (dist < shortestDist)
            {
                closestIndex = i;
                shortestDist = dist;
            }
        }
        return _flagTargets[closestIndex].transform;
    }

    private void UpdateFlagCache()
    {
        _flagTargets.Clear();
        _flagTargets.AddRange(_teamManager.GetTargetFlags(TeamIndex));
    }

    public List<CharacterController> GetNearbyCharacters()
    {
        return null;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        if (_agentTransform == null)
            return;
        Gizmos.DrawLine(transform.position, transform.position + transform.InverseTransformDirection(_agentTransform.position - transform.position));
    }
}