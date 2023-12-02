using System.Collections.Generic;
using UnityEngine;

public class BodyMoverController : MonoBehaviour
{
    [SerializeField] private List<FootMover> _firstLegs = new List<FootMover>();
    [SerializeField] private List<FootMover> _secondLegs = new List<FootMover>();

    private List<List<FootMover>> _legs = new List<List<FootMover>>();
    private int _index = 0;

    private void Start()
    {
        _legs.Add(_firstLegs);
        _legs.Add(_secondLegs);
    }

    private void Update()
    {
        if (_legs[_index][0].ReadyToGo && _legs[_index][0].Grounded && _legs[Mathf.Abs((_index - 1) % _legs.Count)][0].Grounded)
        {
            foreach(var leg in _legs[_index])
            {
                leg.Go();
            }
            _index = (_index + 1) % _legs.Count;
        }
    }
}
