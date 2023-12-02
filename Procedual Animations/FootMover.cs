using UnityEngine;

public class FootMover : MonoBehaviour
{
    public Vector3 NewTarget { get; set; }

    [SerializeField] private Transform _targetPoint;
    [SerializeField] private LayerMask _groundLayers;
    [SerializeField] private float _distance;
    [SerializeField] private float _maxHeightDistance;

    [SerializeField] private float _speed = 5;
    [SerializeField] private float _amplutide = 0.4f;
    [SerializeField] private float _offset = 0.4f;
    [SerializeField] private AnimationCurve _transitionCurve;

    private Vector3 _lastTargetPoint;
    private float currentTime = 1f;
    private bool _allowToGo = false;
    public bool Grounded { get; set; }
    public bool ReadyToGo { get; set; }

    private void Start()
    {
        NewTarget = _targetPoint.position;
        _lastTargetPoint = _targetPoint.position;
        Grounded = true;
        ReadyToGo = false;
    }

    private void Update()
    {
        RaycastHit hit;

        if (Physics.Raycast(transform.position, -transform.up, out hit, _maxHeightDistance, _groundLayers))
        {

            if (Vector3.Distance(hit.point, NewTarget) > _distance)
            {
                currentTime = 0;
                Vector3 offsetVector = (hit.point - _targetPoint.position).normalized;
                NewTarget = hit.point + new Vector3(offsetVector.x, 0, offsetVector.z) * _offset;
                _lastTargetPoint = _targetPoint.position;
                ReadyToGo = true;
            }

            if (currentTime < 1 && _allowToGo)
            {
                Vector3 footPosition = Vector3.Lerp(_lastTargetPoint, NewTarget, _transitionCurve.Evaluate(currentTime));
                footPosition.y = Mathf.Lerp(footPosition.y, NewTarget.y, _transitionCurve.Evaluate(currentTime)) + (Mathf.Sin(currentTime * Mathf.PI) * _amplutide);
                _targetPoint.position = footPosition;
                currentTime += Time.deltaTime * _speed;
            }
            if (currentTime >= 1)
            {
                _allowToGo = false;
                Grounded = true;
            }
        }
    }

    public void Go()
    {
        _allowToGo = true;
        Grounded = false;
        ReadyToGo = false;
    }
}
