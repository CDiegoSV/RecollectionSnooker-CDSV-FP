using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;

namespace Dante.RecollectionSnooker
{
    public class TestMobileInputHandler : MobileInputHandler
    {
        #region References

        [SerializeField] protected GameObject _goLiteralInputCube; //RAW data
        [SerializeField] protected GameObject _goFollowInputCube;  //Lerps
        [SerializeField] protected GameObject _goTouchCursor;
        [SerializeField] protected Camera _camera;
        [SerializeField] protected CinemachineFreeLook _cmFreeLook;

        #endregion

        #region LocalVariables

        protected RaycastHit _raycastHit;

        #endregion

        #region UnityMethods

        void Start()
        {
            InitializeMobileInputHandler();
        }

        private void FixedUpdate()
        {
            _goFollowInputCube.transform.rotation =
                Quaternion.Lerp(
                    _goFollowInputCube.transform.rotation,
                    _goLiteralInputCube.transform.rotation,
                    0.5f
                    );
        }

        #endregion

        #region LocalMethods

        protected override void HandleTouchInputAction(InputAction.CallbackContext value)
        {
            if (value.performed) //The touch value is modified
            {
                if (
                    Physics.Raycast(
                        _camera.ScreenPointToRay(value.ReadValue<Vector2>()),
                        //ray constructed from (origin) the camera
                        //and the "direction" is to the touch input
                        out _raycastHit, //who was targeted with the ray
                        20.0f, //in meters, distance of the projected ray
                        LayerMask.GetMask("Floor", "Ball", "Island", "Token") 
                            //will ignore other colliders or triggers with different layers
                        )
                    )
                {
                    //BINGO!!! we hitted something with the layer "Floor"
                    _goTouchCursor.transform.position = _raycastHit.point; //we place the cursor

                    Debug.Log("TestMobileInputHandler - HandleTouchInputAction(): " +
                        LayerMask.LayerToName(_raycastHit.collider.gameObject.layer));
                    if (_raycastHit.collider.gameObject.layer == LayerMask.NameToLayer("Token"))
                    {
                        //we hitted a token :D
                        _cmFreeLook.m_Follow = _raycastHit.collider.gameObject.transform;
                        _cmFreeLook.m_LookAt = _raycastHit.collider.gameObject.transform;
                    }
                }
            }
            else if (value.canceled)
            {

            }
        }

        protected override void HandleRotateInputAction(InputAction.CallbackContext value)
        {
            if (value.performed)
            {
                _goLiteralInputCube.transform.Rotate(value.ReadValue<Vector3>() * 1.0f);
            }
            else if (value.canceled)
            {

            }
        }

        protected override void HandleTranslateInputAction(InputAction.CallbackContext value)
        {
            if (value.performed)
            {
                _goLiteralInputCube.transform.Translate(value.ReadValue<Vector3>() * 1.0f);
            }
            else if (value.canceled)
            {

            }
        }

        #endregion

    }
}