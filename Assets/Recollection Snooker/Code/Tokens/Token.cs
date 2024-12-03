using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Unity.VisualScripting;

namespace Dante.RecollectionSnooker
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

        [SerializeField] protected RS_TokenFiniteStateMachine _tokenPhysicalFSM;
        //TODO:Assign remaining references to other Token child prefabs
        [SerializeField] protected CinemachineVirtualCameraBase _virtualCameraBase;
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



        private void OnTriggerEnter(Collider other)
        {

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
            if (_virtualCameraBase == null)
            {
                _virtualCameraBase = transform.GetChild(0).GetChild(0).GetComponent<CinemachineVirtualCameraBase>();
                //_freeLookCamera = transform.GetComponentInChildren<CinemachineFreeLook>();
            }
        }

        protected virtual void ValidateTrigger(Collider other)
        {
            switch (_gameReferee.GetGameState)
            { 
                case RS_GameStates.FLICK_TOKEN_BY_PLAYER:
                    ValidateTriggerWithFlag(other);
                    break;
                case RS_GameStates.CANNON_CARGO:
                case RS_GameStates.CANNON_BY_NAVIGATION:
                    ValidateTriggerWithMonsterPart(other);
                    break;
            }
        }

        protected virtual void ValidateTriggerWithFlag(Collider other)
        {
            if (this as Cargo || this as ShipPivot) //Polymorphism
            {
                if (other.gameObject.CompareTag("Flag")) //other = flag
                {
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
                        _flagTransformValues.forward * (Mathf.Abs(_contactedFlag.DeltaXDegrees) + 1f * 4f),
                        other.gameObject.transform.position
                        ); // other.contacts[0].point);


                    //tell the referee to suggest the jump to the cannon state
                    if (this as Cargo)
                    {
                        _gameReferee.GameStateMechanic(RS_GameStates.CANNON_CARGO);
                    }
                    else if (this as ShipPivot)
                    {
                        _gameReferee.GameStateMechanic(RS_GameStates.CANNON_BY_NAVIGATION);
                    }
                }
            }
        }

        protected virtual void ValidateTriggerWithMonsterPart(Collider other)
        {
            if(this as Cargo || this as ShipPivot)
            {
                if (other.gameObject.CompareTag("MonsterLimb"))
                {
                    _gameReferee.CargoCollidedWithMonsterPart = true;
                    UIManager.Instance.HeartLoss(_gameReferee.GetCurrentLifeOfThePlayer - 1);
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
            set { _tokenPhysicalFSM.IsStill = value; }
        }

        public bool IsAvalaibleForFlicking
        {
            get { return _gameplayAttributes.isAvailableForFlicking; }
            set { _gameplayAttributes.isAvailableForFlicking = value; }
        }

        public CinemachineVirtualCameraBase GetVirtualCamera
        {
            get { return _virtualCameraBase; }
        }

        #endregion
    }
}