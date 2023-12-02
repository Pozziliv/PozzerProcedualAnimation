using System.Collections.Generic;
using UnityEngine;

public class BodyHeight : MonoBehaviour
{
    [SerializeField] private List<FootMover> _targetFootPoints;
    [SerializeField] private float _offset;


    private void Update()
    {
        CalculateHeight();
    }

    public void CalculateHeight()
    {
        float sum = 0f;

        for (int i = 0; i < _targetFootPoints.Count; i++)
        {
            sum += _targetFootPoints[i].NewTarget.y;
        }


        float newHeight = sum / _targetFootPoints.Count;

        transform.position = new Vector3(transform.position.x, newHeight + _offset, transform.position.z);
    }
}
