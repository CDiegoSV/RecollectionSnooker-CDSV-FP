using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Dante.RecollectionSnooker
{
    public class CinemachineMobileInputProvider : CinemachineInputProvider
    {
        #region References
        [SerializeField] protected Slider SliderForNoGyroscpeNAccelerometerY;
        [SerializeField] protected Slider SliderForNoGyroscpeNAccelerometerX;
        #endregion

        #region Parameters

        public bool enableCameraRig;

        #endregion

        #region RuntimeVariables
        protected float _currentSliderXValue;
        protected float _currentSliderYValue;

        protected bool _iSliderXAction;
        protected bool _iSliderYAction;

        protected float _previousSliderXValue;
        protected float _previousSliderYValue;

        protected float _deltaSliderXValue;
        protected float _deltaSliderYValue;
        #endregion

        #region UnityMethods
        private void OnDrawGizmos()
        {
            if (SliderForNoGyroscpeNAccelerometerX == null)
            {
                SliderForNoGyroscpeNAccelerometerX = GameObject.FindGameObjectWithTag("SliderX").GetComponent<Slider>();
            }
            if (SliderForNoGyroscpeNAccelerometerY == null)
            {
                SliderForNoGyroscpeNAccelerometerY = GameObject.FindGameObjectWithTag("SliderY").GetComponent<Slider>();
            }
        }
        private void FixedUpdate()
        {
            if (SliderForNoGyroscpeNAccelerometerX != null)
            {
                if (SliderForNoGyroscpeNAccelerometerX.value != _previousSliderXValue)
                {
                    _currentSliderXValue = SliderForNoGyroscpeNAccelerometerX.value;
                    _deltaSliderXValue = _currentSliderXValue - _previousSliderXValue;
                    _iSliderXAction = true;
                    _previousSliderXValue = _currentSliderXValue;
                }
            }
            if (SliderForNoGyroscpeNAccelerometerY != null)
            {
                if (SliderForNoGyroscpeNAccelerometerY.value != _previousSliderYValue)
                {
                    _currentSliderYValue = SliderForNoGyroscpeNAccelerometerY.value;
                    _deltaSliderYValue = _currentSliderYValue - _previousSliderYValue;
                    _iSliderYAction = true;
                    _previousSliderYValue = _currentSliderYValue;
                }
            }
        }
        #endregion

        #region PublicMethods

        public override float GetAxisValue(int axis)
        {
            if (enabled && enableCameraRig)
            {
                var action = ResolveForPlayer(axis, axis == 2 ? ZAxis : XYAxis);
                if (action != null)
                {
                    switch (axis)
                    {
                        case 0:
                            if (_iSliderYAction)
                            {
                                _iSliderYAction = false;
                                return _deltaSliderYValue;
                            }
                            else
                            {
                                return action.ReadValue<Vector3>().x;
                            }
                        case 1:
                            if (_iSliderXAction)
                            {
                                _iSliderXAction = false;
                                return _deltaSliderXValue;
                            }
                            else
                            {
                                return action.ReadValue<Vector3>().y;
                            }
                        case 2: return action.ReadValue<float>();
                    }
                }
            }
            return 0;
        }

        #endregion
    }
}

