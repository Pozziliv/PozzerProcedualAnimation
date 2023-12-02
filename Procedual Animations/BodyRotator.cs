using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyRotator : MonoBehaviour
{
    [SerializeField] private Transform _leftForwardFoot;
    [SerializeField] private Transform _leftBackFoot;
    [SerializeField] private Transform _rightForwardFoot;
    [SerializeField] private Transform _rightBackFoot;
    [SerializeField] private float _rotateSpeed;

    [SerializeField] private float _koef;

    private float _forwardRotation;
    private float _lateralRotation;

    private void Update()
    {
        Rotate();
    }
    private void Rotate() 
    { 
        _forwardRotation = (_leftForwardFoot.transform.position.y + _rightForwardFoot.transform.position.y) - (_leftBackFoot.transform.position.y + _rightBackFoot.transform.position.y);
        _lateralRotation = ((_leftForwardFoot.transform.position.y + _leftBackFoot.transform.position.y) - (_rightForwardFoot.transform.position.y + _rightBackFoot.transform.position.y));
        if (Mathf.Abs(_forwardRotation) < 1.5f)
        {
            _forwardRotation = 0.0f;
        }
        if (Mathf.Abs(_lateralRotation) < 1.5f)
        {
            _lateralRotation = 0.0f;
        }

        Vector3 newRotation = new Vector3(_forwardRotation * -_koef, transform.eulerAngles.y, _lateralRotation * -_koef); //Умножаем наклоны на коэффицент. 

        float currentLateralRotationMoving = Mathf.LerpAngle(transform.eulerAngles.x, newRotation.x, _rotateSpeed); //Добиваемся плавного наклона с помощью линейной интерполяции

        float currentForwardRotationMoving = Mathf.LerpAngle(transform.eulerAngles.z, newRotation.z, _rotateSpeed);

        transform.rotation = Quaternion.Euler(new Vector3(currentLateralRotationMoving, transform.eulerAngles.y, currentForwardRotationMoving));
    }
}
