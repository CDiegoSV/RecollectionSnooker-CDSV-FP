using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Dante.RecollectionSnooker
{
    public class RS_MobileInputHandler : MobileInputHandler
    {
        #region References

        [SerializeField] protected GameObject _goTouchCursor;
        [SerializeField] protected Camera _camera;
        [SerializeField] protected RS_GameReferee _gameReferee;

        [Header("Sliders")]
        [SerializeField] protected Slider _rotateXSlider;
        [SerializeField] protected Slider _rotateYSlider;

        #endregion

        #region RuntimeVariables

        protected RaycastHit _raycastHit;
        protected Token _chosenToken;
        protected Token _contactToken;


        protected float _currentSliderXValue;
        protected float _currentSliderYValue;
        protected float _previousSliderXValue;
        protected float _previousSliderYValue;
        protected float _deltaSliderXValue;
        protected float _deltaSliderYValue;

        #endregion

        #region UnityMethods

        void Start()
        {
            InitializeMobileInputHandler();
        }

        private void FixedUpdate()
        {
            
        }

        #endregion

        #region PublicMehtods
        public void HandleRotateXSlider()
        {
            switch (_gameReferee.GetGameState)
            {
                case RS_GameStates.FLICK_TOKEN_BY_PLAYER: //manage the flags
                    HandleRotationInFlickTokenByPlayerX(_rotateXSlider.value);
                    break;
            }
        }

        public void HandleRotateYSlider()
        {
            switch (_gameReferee.GetGameState)
            {
                case RS_GameStates.FLICK_TOKEN_BY_PLAYER: //manage the flags
                    HandleRotationInFlickTokenByPlayerY(_rotateYSlider.value);
                    break;
            }
        }
        #endregion

        #region LocalMethods

        protected override void HandleTouchInputAction(InputAction.CallbackContext value)
        {
            switch (_gameReferee.GetGameState)
            {
                case RS_GameStates.CHOOSE_TOKEN_BY_PLAYER:
                    HandleTouchInChooseTokenByPlayer(value);
                    break;
                case RS_GameStates.CONTACT_POINT_TOKEN_BY_PLAYER:
                    HandleTouchInContactPointTokenByPlayer(value);
                    break;
                case RS_GameStates.LOADING_AND_ORGANIZING_CARGO_BY_PLAYER:
                    HandleTouchInLoadingCargo(value);
                    break;
                    
            }
        }

        protected override void HandleRotateInputAction(InputAction.CallbackContext value)
        {
            switch (_gameReferee.GetGameState)
            {
                case RS_GameStates.FLICK_TOKEN_BY_PLAYER: //manage the flags
                    HandleRotationInFlickTokenByPlayer(value);
                    break;
            }
        }

        protected override void HandleTiltInputAction(InputAction.CallbackContext value)
        {
            Debug.Log("TIIIIIIIILTTTTTTTT");
            switch (_gameReferee.GetGameState)
            {
                case RS_GameStates.FLICK_TOKEN_BY_PLAYER: //manage the flags
                    HandleRotationInFlickTokenByPlayer(value);
                    break;
            }
        }

        protected override void HandleTranslateInputAction(InputAction.CallbackContext value)
        {
            if (value.performed)
            {
                
            }
            else if (value.canceled)
            {

            }
        }

        #endregion

        #region HandleTouchActions

        protected void HandleTouchInChooseTokenByPlayer(InputAction.CallbackContext value)
        {
            if (value.performed)
            {
                if (
                    Physics.Raycast(
                        _camera.ScreenPointToRay(value.ReadValue<Vector2>()),
                        out _raycastHit,
                        50.0f,
                        LayerMask.GetMask("Token")
                        )
                    )
                {
                    _chosenToken = _raycastHit.collider.gameObject.GetComponent<Token>(); //by polymorphisdm
                    if (_chosenToken != null && _chosenToken.IsAvalaibleForFlicking) {
                        _goTouchCursor.SetActive(true);
                        _goTouchCursor.transform.position = _raycastHit.point;
                        //set the parameter prior to the initialization method
                        //of the Contact Point Token by Player State
                        _gameReferee.SetInteractedToken = _chosenToken;
                        _gameReferee.GameStateMechanic(RS_GameStates.CONTACT_POINT_TOKEN_BY_PLAYER);
                    }
                }
                else
                {
                    _goTouchCursor.SetActive(false);
                }
            }
            else if (value.canceled)
            {
                _goTouchCursor.SetActive(false);
            }
        }

        protected void HandleTouchInLoadingCargo(InputAction.CallbackContext value)
        {
            if (value.performed)
            {
                if (
                    Physics.Raycast(
                        _camera.ScreenPointToRay(value.ReadValue<Vector2>()),
                        out _raycastHit,
                        50.0f,
                        LayerMask.GetMask("CargoZone")
                        )
                    )
                {
                    _goTouchCursor.SetActive(true);
                    _goTouchCursor.transform.position = _raycastHit.point;

                    _gameReferee.CargoToBeLoaded.gameObject.transform.position = Vector3.Lerp(_goTouchCursor.transform.position, _gameReferee.CargoToBeLoaded.gameObject.transform.position, 0.5f);
                }
                else
                {
                    _goTouchCursor.SetActive(false);
                }
            }
            else if(value.canceled)
            {
                _goTouchCursor.SetActive(false);
                _gameReferee.GameStateMechanic(RS_GameStates.SHIFT_MONSTER_PARTS);
            }
        }

        protected void HandleTouchInContactPointTokenByPlayer(InputAction.CallbackContext value)
        {
            if (value.performed)
            {
                if (
                    Physics.Raycast(
                        _camera.ScreenPointToRay(value.ReadValue<Vector2>()),
                        out _raycastHit,
                        50.0f,
                        LayerMask.GetMask("Token")
                        )
                    )
                {
                    _contactToken = _raycastHit.collider.gameObject.GetComponent<Token>();
                    //compare between the last state and this current state
                    //to check if they are the same ;)
                    if (_contactToken == _chosenToken) 
                    {
                        _goTouchCursor.SetActive(true);
                        _goTouchCursor.transform.position = _raycastHit.point;
                        _gameReferee.GameStateMechanic(RS_GameStates.FLICK_TOKEN_BY_PLAYER);
                    }
                }
                else
                {
                    _goTouchCursor.SetActive(false);
                }
            }
        }

        #endregion HandleTouchActions

        #region HandleRotationActions

        protected void HandleRotationInFlickTokenByPlayer(InputAction.CallbackContext value)
        {
            if (value.performed)
            {
                _gameReferee.GetCurrentFlag.transform.Rotate(
                new Vector3(
                        (value.ReadValue<Vector3>() != null ? 
                        value.ReadValue<Vector3>().x : 
                        value.ReadValue<Vector2>().x) * -4f, //X
                        0f,                             //Y
                        0f                              //Z
                    ),
                    Space.Self //localRotation
                );
            }
            else if (value.canceled)
            {

            }
        }

        protected void HandleRotationInFlickTokenByPlayerX(float value)
        {
            _currentSliderXValue = value;
            _deltaSliderXValue = _currentSliderXValue - _previousSliderXValue;
            _gameReferee.GetCurrentFlag.transform.Rotate(
            new Vector3(
                    0f,                             //Y
                    _deltaSliderXValue * -4f,                    //X
                    0f                              //Z
                ),
                Space.Self //localRotation
            );
            _previousSliderXValue = _currentSliderXValue;
        }

        protected void HandleRotationInFlickTokenByPlayerY(float value)
        {
            //_gameReferee.GetCurrentFlag.transform.Rotate(
            //new Vector3(
            //        value * -4f,                             //Y
            //        0f,                    //X
            //        0f                              //Z
            //    ),
            //    Space.Self //localRotation
            //);
            _currentSliderYValue = value;
            _deltaSliderYValue = _currentSliderYValue - _previousSliderYValue;
            _gameReferee.GetCurrentFlag.transform.Rotate(
            new Vector3(
                    _deltaSliderYValue * -4f,                    //X
                    0f,                             //Y
                    0f                              //Z
                ),
                Space.Self //localRotation
            );
            _previousSliderYValue = _currentSliderYValue;
        }

        #endregion HandleRotationActions
    }
}