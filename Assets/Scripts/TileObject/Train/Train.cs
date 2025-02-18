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

    private int _requestedDestination = -1;

    //(40) -> 18 -> 1-18 -> ? -> 122-107 -> 18

    private Vector3 _targetForward;


    public Transform[] wagons;
    private float[] _wagonDistances = { 2.0f, 3.6f, 5.0f };
    private float _sumOfDistances;
    public List<int> _prevPositions = new();
    private int _historySize = 65;

    public Transform[] trainWheels;

    private void Start()
    {
        if (_currentWaypoint != -1)
        {
            transform.position = _waypoints.transform.GetChild(_currentWaypoint).transform.position;
            transform.forward = (_waypoints.transform.GetChild(_currentWaypoint + 1).transform.position - _waypoints.transform.GetChild(_currentWaypoint).transform.position).normalized;
            for (int i = 0; i < wagons.Length; i++)
            {
                wagons[i].position = transform.position;
                wagons[i].forward = transform.forward;
            }
        }
        if (_prevTargetWaypoint == -1) _prevTargetWaypoint = _targetWaypoint;

        _sumOfDistances = 0;
        for (int i = 0; i < _wagonDistances.Length; i++) _sumOfDistances += _wagonDistances[i];
        for (int i = 0; i < _historySize * wagons.Length; i++)
        {
            _prevPositions.Insert(0, _currentWaypoint);
        }
    }
    float moveSpeed = 5;
    float rotateSpeed = 10;

    void Update()
    {
        //check current position
        float minDist = float.MaxValue;
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

        //camera tracking (if player is board)
        if (_requestedDestination != -1)
        {
            CameraManager.instance.worldCamera.SetPosition(wagons[wagons.Length - 1].transform.position);

            IEnumerable<Tile> allTile = FieldSystem.tileSystem.GetAllTiles();
            minDist = float.MaxValue;
            Vector3Int curPos = Vector3Int.zero;
            foreach (var t in allTile) 
            {
                float d = Vector3.Distance(transform.position, t.transform.position);
                if (d < minDist)
                {
                    minDist = d;
                    curPos = t.hexPosition;
                }
            }

            FieldSystem.unitSystem.GetPlayer().hexPosition = curPos;
        }

        //compare current and target positon
        if (_targetWaypoint != -1 && _currentWaypoint != _targetWaypoint)
        {
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
            _prevPositions.Insert(0, _currentWaypoint);
            if (_prevPositions.Count > _historySize * wagons.Length)
            {
                _prevPositions.RemoveAt(_prevPositions.Count - 1);
            }
        }
        else if (_currentWaypoint == _targetWaypoint) 
        {
            ArriveDestination();
        }



        //wagon move
        float cumulativeDist = 0;
        for (int i = 0; i < wagons.Length; i++)
        {
            cumulativeDist += _wagonDistances[i];
            int index = (int)((cumulativeDist / _sumOfDistances) * (_prevPositions.Count - 1));

            Vector3 pos = wagons[i].position;
            Vector3 nPos = _waypoints.transform.GetChild(_prevPositions[index]).transform.position;
            Vector3 mDir = (nPos - pos).normalized;
            if (Vector3.Distance(pos, nPos) > 1.0f)
            {
                //pos += mDir * moveSpeed * 1.1f * Time.deltaTime;
                //wagons[i].position = pos;
                if (LerpCalculation.CalculateLerpValue(ref pos, pos + mDir * moveSpeed * 1.01f)) wagons[i].position = pos;
            }
            Vector3 fd2 = wagons[i].forward;
            if (LerpCalculation.CalculateLerpValue(ref fd2, mDir, rotateSpeed, 0.001f)) wagons[i].forward = fd2;
        }

        //rotate wheel
        for (int i = 0; i < trainWheels.Length; i++)
        {
            trainWheels[i].Rotate(new Vector3(2, 0, 0));
        }
    }

    public void SetTrainDestination(int destIdx) 
    {
        _requestedDestination = destIdx;
        _targetWaypoint = destIdx;
    }
    private void ArriveDestination() 
    {
        if (_requestedDestination == -1) return;

        _requestedDestination = -1;
        UIManager.instance.gameSystemUI.townUI.GetOffTrain();
    }
}
