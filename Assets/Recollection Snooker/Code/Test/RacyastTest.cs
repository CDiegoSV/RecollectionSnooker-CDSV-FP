using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RacyastTest : MonoBehaviour
{
    #region Knobs/Parameters

    public Transform originOfTheRay;
    public Transform targetOfTheRay;
    public string[] layerMask;

    #endregion

    #region RuntimeVariables

    [SerializeField] protected Ray _ray;
    protected RaycastHit _raycastHit;
    protected float _magnitudeOfTheRay;
    protected Color _colorOfTheRay;

    #endregion

    void Start()
    {
        //_ray = new Ray(); //to avoid "new" you can implement SerializeField
    }

    
    void FixedUpdate()
    {
        _ray.origin = originOfTheRay.transform.position;
        _ray.direction = (targetOfTheRay.transform.position - originOfTheRay.transform.position).normalized;
        _magnitudeOfTheRay = (targetOfTheRay.transform.position - originOfTheRay.transform.position).magnitude;

        if (Physics.Raycast(_ray, out _raycastHit, _magnitudeOfTheRay, LayerMask.GetMask(layerMask)))
        {
            Debug.Log("I hitted someone with the name: " + _raycastHit.collider.gameObject.name);
            _colorOfTheRay = Color.green;
        }
        else
        {
            _colorOfTheRay = Color.red;
        }

        Debug.DrawRay(
            _ray.origin,
            _ray.direction * _magnitudeOfTheRay,
            _colorOfTheRay,
            Time.fixedDeltaTime
            );
    }
}
