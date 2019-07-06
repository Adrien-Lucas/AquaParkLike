using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI : MonoBehaviour
{
    [SerializeField] private Vector2 deviationChangeTimeRange = new Vector2();
    [SerializeField] private float dragControl = 1;

    private Movements _movements;
    private float _timer;
    private float _currentDeviationDuration;
    private float _targetDeviation;
    private float _marginFromEjection;
    private float _maxDeviation;

    void Start()
    {
        _movements = GetComponent<Movements>();
        _maxDeviation = _movements.ejectionThresold - _marginFromEjection;
    }
    // Update is called once per frame
    void Update()
    {
        if (_timer > _currentDeviationDuration)
        {
            _currentDeviationDuration = Random.Range(deviationChangeTimeRange.x, deviationChangeTimeRange.y);

            _targetDeviation = Random.Range(-_maxDeviation, _maxDeviation);
            _timer = 0;
        }

        _movements.deviation = Mathf.Lerp(_movements.deviation, _targetDeviation, dragControl * Time.deltaTime);

        _timer += Time.deltaTime;
    }
}
