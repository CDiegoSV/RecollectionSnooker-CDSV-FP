using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SotomaYorch.RecollectionSnooker
{
    public class MobileInputHandler : MonoBehaviour
    {
        #region UnityMethods

        void Start()
        {
            InitializeMobileInputHandler();
        }

        void Update()
        {

        }

        #endregion

        #region LocalMethods

        protected void InitializeMobileInputHandler()
        {
            //UNITY_STANDALONE_WIN
            #if UNITY_ANDROID && !UNITY_EDITOR
                InputSystem.EnableDevice(UnityEngine.InputSystem.Gyroscope.current);
                InputSystem.EnableDevice(UnityEngine.InputSystem.LinearAccelerationSensor.current);
            #endif
        }

        protected virtual void HandleTouchInputAction(InputAction.CallbackContext value)
        { 
            if (value.performed)
            {
                //value.ReadValue<Type>();
            }
            else if (value.canceled)
            {

            }
        }

        protected virtual void HandleRotateInputAction(InputAction.CallbackContext value)
        {
            if (value.performed)
            {

            }
            else if (value.canceled)
            {

            }
        }

        protected virtual void HandleTiltInputAction(InputAction.CallbackContext value)
        {
            if (value.performed)
            {

            }
            else if (value.canceled)
            {

            }
        }

        protected virtual void HandleTranslateInputAction(InputAction.CallbackContext value)
        {
            if (value.performed)
            {

            }
            else if (value.canceled)
            {

            }
        }

        #endregion

        #region InputHandlingEvents

        public void OnTouchInputAction(InputAction.CallbackContext value)
        {
            HandleTouchInputAction(value);
        }

        public void OnRotateInputAction(InputAction.CallbackContext value)
        {
            HandleRotateInputAction(value);
        }

        public void OnTranslateInputAction(InputAction.CallbackContext value)
        {
            HandleTranslateInputAction(value);
        }

        public void OnTiltInputAction(InputAction.CallbackContext value)
        {
            HandleRotateInputAction(value);
        }

        #endregion

    }

}