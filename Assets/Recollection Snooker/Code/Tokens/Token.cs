using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Unity.VisualScripting;

namespace SotomaYorch.RecollectionSnooker
{
    #region Enums


    #endregion

    #region Structs

    [System.Serializable]
    public struct GameplayAttributes
    {
        public bool isAvailableForFlicking;
        public bool isOutOfTheBoard;
        public bool isBeingDragged;
    }

    #endregion

    [RequireComponent(typeof(RS_TokenFiniteStateMachine))]
    public class Token : MonoBehaviour
    {
        #region Knobs


        #endregion

        #region References

        [SerializeField,HideInInspector] protected RS_TokenFiniteStateMachine _tokenPhysicalFSM;
        //TODO:Assign remaining references to other Token child prefabs
        [SerializeField] protected CinemachineFreeLook _freeLookCamera;
        [SerializeField] protected RS_GameReferee _gameReferee;
        [SerializeField] protected Transform _flagTransformValues;

        #endregion

        #region RuntimeVariables

        [Header("Runtime Variables")]
        [SerializeField] protected GameplayAttributes _gameplayAttributes;
        protected Transform _flagTransform;
        protected Flag _contactedFlag;

        #endregion

        #region UnityMethods

        void Start()
        {
            InitializeToken();
        }

        void FixedUpdate()
        {
            
        }

        private void OnCollisionEnter(Collision other)
        {
            _gameReferee.DebugInMobile(gameObject.name + 
                " OnCollisionEnter() - Detected collision with " + 
                other.gameObject.name);
            //ValidateCollision(other);
        }

        private void OnTriggerEnter(Collider other)
        {
            _gameReferee.DebugInMobile(gameObject.name +
                " OnTriggerEnter() - Detected collision with " + 
                other.gameObject.name);
            ValidateTrigger(other);
        }

        //void UpdateInEditor()
        private void OnDrawGizmos()
        {
            #if UNITY_EDITOR
            ValidateReferences();
            #endif
        }

        #endregion

        #region RuntimeMethods

        protected virtual void ValidateReferences()
        {
            if (_gameReferee == null)
            {
                _gameReferee = GameObject.FindAnyObjectByType<RS_GameReferee>();
            }
            if (_flagTransformValues == null)
            {
                _flagTransformValues = transform.GetChild(1).transform;
            }
            if (_freeLookCamera == null)
            {
                _freeLookCamera = transform.GetChild(0).GetChild(0).GetComponent<CinemachineFreeLook>();
                //_freeLookCamera = transform.GetComponentInChildren<CinemachineFreeLook>();
            }
        }

        protected virtual void ValidateTrigger(Collider other)
        {
            _gameReferee.DebugInMobile(" Validate Collision " + other.gameObject.name);
            switch (_gameReferee.GetGameState)
            { 
                case RS_GameStates.FLICK_TOKEN_BY_PLAYER:
                    _gameReferee.DebugInMobile("FLICK_TOKEN_BY_PLAYER " + other.gameObject.name);
                    ValidateTriggerWithFlag(other);
                    break;
            }
        }

        protected virtual void ValidateTriggerWithFlag(Collider other)
        {
            if (this as Cargo || this as ShipPivot) //Polymorphism
            {
                _gameReferee.DebugInMobile("I am a Cargo or ShipPivot " + other.gameObject.name);
                if (other.gameObject.CompareTag("Flag")) //other = flag
                {
                    _gameReferee.DebugInMobile("I detected a flag " + other.gameObject.name);
                    //Obtain the push direction
                    //by obtaining the rotation in the X axis
                    _flagTransform = other.gameObject.transform; //pointer refrence
                    //_flagTransformValues = _flagTransform; //NO USE: pointer reference
                    _flagTransformValues.forward = _flagTransform.forward; //copy values by Vector3
                    _flagTransformValues.position = _flagTransform.position; //copy values by Vector3
                    //and adding -90° to obtained direction
                    _flagTransformValues.Rotate(
                        _flagTransformValues.right, //my own X axis
                        -90f,
                        Space.Self //localRotation
                        );

                    _contactedFlag = _flagTransform.gameObject.GetComponent<Flag>();
                    //TODO: Project a Raycast from the tip of the flag to the Token,
                    //to obtain the point of contact
                    //other.contacts[0].point it gives us the specefic point of contact
                    _tokenPhysicalFSM.ThrowTokenAtSpecificPosition(
                        _flagTransformValues.forward * (Mathf.Abs(_contactedFlag.DeltaXDegrees) + 1f),
                        other.gameObject.transform.position
                        ); // other.contacts[0].point);

                    //tell the referee to suggest the jump to the cannon state
                    if (this as Cargo)
                    {
                        _gameReferee.GameStateMechanic(RS_GameStates.CANNON_CARGO);
                        _gameReferee.DebugInMobile("CANNON " + other.gameObject.name);
                    }
                    else //if (this as ShipPivot)
                    {
                        _gameReferee.GameStateMechanic(RS_GameStates.CANNON_BY_NAVIGATION);
                    }
                }
            }
        }

        protected virtual void InitializeToken()
        {
            //if by any circumstance, the reference is lost,
            //we retreive it
            if (_tokenPhysicalFSM == null)
            {
                _tokenPhysicalFSM = GetComponent<RS_TokenFiniteStateMachine>();
            }
            ValidateReferences();
        }

        #endregion

        #region PublicMethods

        public void StateMechanic(TokenStateMechanic value)
        {
            _tokenPhysicalFSM.StateMechanic(value);
        }

        //Broker
        public void SetHighlight(bool value)
        {
            _tokenPhysicalFSM.SetHighlight(value);
        }

        #endregion

        #region GettersSetters

        public bool IsStill
        {
            get { return _tokenPhysicalFSM.IsStill; }
        }

        public bool IsAvalaibleForFlicking
        {
            get { return _gameplayAttributes.isAvailableForFlicking; }
            set { _gameplayAttributes.isAvailableForFlicking = value; }
        }

        public CinemachineFreeLook GetFreeLookCamera
        {
            get { return _freeLookCamera; }
        }

        #endregion
    }
}