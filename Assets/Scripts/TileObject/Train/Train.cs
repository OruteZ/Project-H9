using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Train : MonoBehaviour
{
    [SerializeField] private GameObject _waypoints;
    public int _currentWaypoint = -1;
    public int _nextWaypoint;
    public int _targetWaypoint = -1;
    private int _prevTargetWaypoint = -1;

    //(40) -> 18 -> 1-18 -> ? -> 122-107 -> 18

    private Vector3 _targetForward;


    public Transform[] wagons;
    private float[] _wagonDistances = { 3.0f, 3.7f, 5.3f };
    private float _sumOfDistances;
    private List<Vector3> _prevPositions = new();
    private List<Quaternion> _prevRotations = new();
    private int _historySize = 65;

    public Transform[] trainWheels;

    private void Start()
    {
        if (_currentWaypoint != -1)
        {
            transform.position = _waypoints.transform.GetChild(_currentWaypoint).transform.position;
            transform.forward = (_waypoints.transform.GetChild(_currentWaypoint + 1).transform.position - _waypoints.transform.GetChild(_currentWaypoint).transform.position).normalized;
        }
        if (_prevTargetWaypoint == -1) _prevTargetWaypoint = _targetWaypoint;

        _sumOfDistances = 0;
        for (int i = 0; i < _wagonDistances.Length; i++) _sumOfDistances += _wagonDistances[i];
    }
    float moveSpeed = 5;
    float rotateSpeed = 10;

    void Update()
    {
        //check current position
        float minDist = 100;
        int minPos = -1;
        for (int i = 0; i < _waypoints.transform.childCount; i++) 
        {
            float curDist = Vector3.Distance(transform.position, _waypoints.transform.GetChild(i).transform.position);
            if (curDist < minDist) 
            {
                minDist = curDist;
                minPos = i;
            }
        }
        if (minPos == -1) 
        {
            Debug.LogError("train position error");
            return;
        }
        if (minDist < 0.5)
        {
            _currentWaypoint = minPos;
        }

        //compare current and target positon
        if (_targetWaypoint == -1 || _currentWaypoint == _targetWaypoint) return;

        //set next position
        if (_currentWaypoint < _targetWaypoint) _nextWaypoint = _currentWaypoint + 1;
        else _nextWaypoint = _currentWaypoint - 1;

        //move
        Vector3 curPosition = transform.position;
        Vector3 nextPosition = _waypoints.transform.GetChild(_nextWaypoint).transform.position;
        Vector3 moveDir = (nextPosition - curPosition).normalized;

        curPosition += moveDir * moveSpeed * Time.deltaTime;
        transform.position = curPosition;

        if (_prevTargetWaypoint != _targetWaypoint) 
        {
            if ((_currentWaypoint - _targetWaypoint) * (_currentWaypoint - _prevTargetWaypoint) <= 0)
            {
                transform.forward *= -1;
            }
            _prevTargetWaypoint = _targetWaypoint;
        }

        _targetForward = moveDir;
        Vector3 fd = transform.forward;
        LerpCalculation.CalculateLerpValue(ref fd, _targetForward, rotateSpeed);
        transform.forward = fd;


        //record transform
        _prevPositions.Insert(0, transform.position);
        _prevRotations.Insert(0, transform.rotation);
        if (_prevPositions.Count > _historySize * wagons.Length)
        {
            _prevPositions.RemoveAt(_prevPositions.Count - 1);
            _prevRotations.RemoveAt(_prevRotations.Count - 1);
        }
        //wagon move
        float cumulativeDist = 0;
        for (int i = 0; i < wagons.Length; i++)
        {
            cumulativeDist+= _wagonDistances[i];
            int index = (int)Mathf.Min(_historySize * wagons.Length * (cumulativeDist / _sumOfDistances), _prevPositions.Count - 1);

            Vector3 pos = wagons[i].position;
            Quaternion rot = wagons[i].rotation;

            LerpCalculation.CalculateLerpValue(ref pos, _prevPositions[index], moveSpeed * 5);
            LerpCalculation.CalculateLerpValue(ref rot, _prevRotations[index], rotateSpeed * 5);
            wagons[i].position = pos;
            wagons[i].rotation = rot;
        }

        //rotate wheel
        for (int i = 0; i < trainWheels.Length; i++)
        {
            trainWheels[i].Rotate(new Vector3(2, 0, 0));
        }
    }
}
