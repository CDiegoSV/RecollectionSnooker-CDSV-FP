using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dante.RecollectionSnooker
{
    public class Flag : MonoBehaviour
    {
        #region RuntimeVariables

        protected Vector3 _previousLocalRotation;
        protected Vector3 _actualLocalRotation;

        #endregion

        #region UnityMethods

        void Start()
        {
            //duplication values within Vectors
            _previousLocalRotation = transform.localRotation.eulerAngles;
        }

        void Update()
        {
            _actualLocalRotation = transform.localRotation.eulerAngles;
            //if (_actualLocalRotation.x > 120f)
            //{
            //    _actualLocalRotation.x = 120f;
            //    transform.localRotation = Quaternion.Euler(_actualLocalRotation);
            //}
        }

        private void LateUpdate()
        {
            _previousLocalRotation = _actualLocalRotation;
        }

        #endregion

        #region Getters

        public float DeltaXDegrees
        {
            get { return _actualLocalRotation.x - _previousLocalRotation.x; }
        }

        #endregion
    }
}