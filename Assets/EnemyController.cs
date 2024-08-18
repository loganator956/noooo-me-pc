using CharacterSystems.Movement;
using JetBrains.Annotations;
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
        _agentTransform.SetParent(transform);
        _teamManager.RegisterCharacterToSpecificTeam(GetComponent<CharacterController>(), TeamIndex);
        _agentTransform.position = transform.position - Vector3.up * -0.5f;
        _agent.speed = 12f;
    }

    private float _pathTCountdown = 8f;
    private List<Vector3> _pathPoints = new List<Vector3>();

    // Update is called once per frame
    void Update()
    {
        _pathTCountdown -= Time.deltaTime;
        if (_pathTCountdown < 0 && _pathPoints.Count > 0)
        {
            _pathPoints = GeneratePath(Target);
        }

        _movement.CameraForward = transform.forward;
        //float distance = (_agentTransform.position - transform.position).magnitude;
        //_agent.enabled = distance < MAX_AGENT_DISTANCE;

        if (_pathPoints.Count > 0)
        {
            PathAge += Time.deltaTime;
            Vector3 nextPoint = _pathPoints[0];
            nextPoint.y = transform.position.y;
            Vector3 deltaToNextPoint = nextPoint - transform.position;
            float distanceToNextPoint = deltaToNextPoint.magnitude;
            Vector3 directionToNextPoint = deltaToNextPoint.normalized;
            Vector3 localDirectionToNextPoint = transform.InverseTransformDirection(directionToNextPoint);
            _movement.Move(new Vector2(localDirectionToNextPoint.x * Mathf.Clamp(distanceToNextPoint / _movement.WalkSpeed, -1f, 1f), localDirectionToNextPoint.z * Mathf.Clamp(distanceToNextPoint / _movement.WalkSpeed, -1f, 1f)));
            if (distanceToNextPoint < 1f)
                _pathPoints.RemoveAt(0);
        }
        else
        {
            _movement.Move(Vector2.zero);
        }
    }

    public bool HasPath
    {
        get
        {
            return _pathPoints.Count > 0;
        }
    }

    public List<Vector3> GeneratePath(Vector3 Destination)
    {
        _pathTCountdown = 4;
        var path = new List<Vector3>();
        _agent.enabled = true;
        _agentTransform.position = transform.position;
        if (_agent.SetDestination(Destination))
        {
            foreach (var corner in _agent.path.corners)
            {
                path.Add(corner);
            }
            path.Add(_agent.destination);
        }
        _agent.enabled = false;
        return path;
    }

    private Vector3 _target;
    public Vector3 Target
    {
        get { return _target; }
        set
        {
            _target = value;
            if (_target != transform.position)
                _pathPoints = GeneratePath(Target);
            else
                _pathPoints.Clear();
            PathAge = 0;
        }
    }

    public float PathAge = 0;
}