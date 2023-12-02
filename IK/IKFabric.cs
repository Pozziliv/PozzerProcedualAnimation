using System;
using UnityEngine;

public class IKFabric : MonoBehaviour
{
    [SerializeField] private int _chainLength = 2; //Bones count
    [SerializeField] private Transform _target;
    [SerializeField] private Transform _pole;

    [Header("Solver Parameters")]
    [SerializeField] private int _iterations = 10;
    [SerializeField] private float _delta = 0.001f;
    [SerializeField, Range(0,1)] private float _snapBackStrength = 1f;

    private float[] _bonesLength;
    private float _completeLength;
    private Transform[] _bones;
    private Vector3[] _positions;
    private Vector3[] _startDirectionSucc;
    private Quaternion[] _startRotationBone;
    private Quaternion _startRotationTarget;
    private Quaternion _startRotationRoot;

    private void Awake()
    {
        Init();
    }

    private void Init()
    {
        _bones = new Transform[_chainLength + 1]; //Anchors count
        _positions = new Vector3[_chainLength + 1]; //Anchors positions
        _bonesLength = new float[_chainLength];
        _startDirectionSucc = new Vector3[_chainLength + 1];
        _startRotationBone = new Quaternion[_chainLength + 1];

        if(_target == null)
        {
            _target = new GameObject(gameObject.name + "Target").transform;
            _target.position = transform.position;
        }
        _startRotationTarget = _target.rotation;

        _completeLength = 0; //Full length

        Transform current = transform;
        for(int i = _bones.Length - 1; i >= 0; i--) //Array forming from body to end of leg
        {
            _bones[i] = current;
            _startRotationBone[i] = current.rotation;

            if(i == _bones.Length - 1) //leg on ground
            {
                _startDirectionSucc[i] = current.forward;
            }
            else
            {
                _startDirectionSucc[i] = _bones[i+1].position - current.position;
                _bonesLength[i] = (_bones[i+1].position - current.position).magnitude;
                _completeLength += _bonesLength[i];
            }

            current = current.parent;
        }
    }

    private void LateUpdate()
    {
        ResolveIK();
    }

    private void ResolveIK()
    {
        if (_target == null)
            return;

        if(_bonesLength.Length != _chainLength)
            Init();

        for(int i =0; i<_bonesLength.Length; i++) //fill bones positions for changing them
            _positions[i] = _bones[i].position;

        var rootRot = (_bones[0].parent != null) ? _bones[0].parent.rotation : Quaternion.identity;
        var rootRotDiff = rootRot * Quaternion.Inverse(_startRotationRoot);

        if((_target.position - _bones[0].position).sqrMagnitude >= _completeLength * _completeLength) //very far
        {
            Vector3 direction = (_target.position - _positions[0]).normalized;

            for(int i = 1; i < _positions.Length; i++)
                _positions[i] = _positions[i-1] + direction * _bonesLength[i-1 ];
        }
        else
        {
            for (int i = 0; i < _positions.Length - 1; i++)
                _positions[i + 1] = Vector3.Lerp(_positions[i+1], _positions[i] + rootRotDiff * _startDirectionSucc[i], _snapBackStrength);

            for(int iteration = 0; iteration < _iterations; iteration++)
            {
                for(int i = _positions.Length-1; i > 0; i--)
                {
                    if(i == _positions.Length - 1)
                        _positions[i] = _target.position;
                    else
                        _positions[i] = _positions[i+1] + (_positions[i] - _positions[i+1]).normalized * _bonesLength[i];
                }

                for(int i = 1; i < _positions.Length;i++)
                    _positions[i] = _positions[i - 1] + (_positions[i] - _positions[i - 1]).normalized * _bonesLength[i - 1];

                if ((_positions[_positions.Length - 1] - _target.position).sqrMagnitude < _delta * _delta)
                    break;
            }
        }

        if(_pole != null)
        {
            for(int i = 1; i < _positions.Length-1; i++)
            {
                var plane = new Plane(_positions[i + 1] - _positions[i - 1], _positions[i - 1]);
                var projectedPole = plane.ClosestPointOnPlane(_pole.position);
                var projectedBone = plane.ClosestPointOnPlane(_positions[i]);
                var angle = Vector3.SignedAngle(projectedBone - _positions[i - 1], projectedPole - _positions[i - 1], plane.normal);
                _positions[i] = Quaternion.AngleAxis(angle, plane.normal) * (_positions[i] - _positions[i - 1]) + _positions[i - 1];
            }
        }

        for(int i = 0; i < _positions.Length; i++) //rotation
        {
            if(i == _positions.Length-1)
                _bones[i].rotation = _target.rotation * Quaternion.Inverse(_startRotationTarget) * _startRotationBone[i];
            else
                _bones[i].rotation = Quaternion.FromToRotation(_startDirectionSucc[i], _positions[i + 1] - _positions[i]) * _startRotationBone[i];
            _bones[i].position = _positions[i];
        }
    }
}
