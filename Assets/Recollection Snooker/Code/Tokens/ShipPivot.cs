using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dante.RecollectionSnooker
{
    #region Enums


    #endregion

    #region Structs


    #endregion

    public class ShipPivot : Token
    {
        #region Knobs


        #endregion

        #region References

        [SerializeField] private CinemachineVirtualCameraBase _anchorShipCamera;

        [SerializeField] private GameObject anchorZone;



        #endregion

        #region RuntimeVariables

        #endregion

        #region UnityMethods

        void Start()
        {
            base.InitializeToken();
        }

        void Update()
        {

        }

        private void OnCollisionEnter(Collision other)
        {
            if (_gameReferee.GetGameState == RS_GameStates.CANNON_CARGO ||
               _gameReferee.GetGameState == RS_GameStates.CANNON_BY_NAVIGATION)
            {
                if (!other.gameObject.CompareTag("Floor") && !other.gameObject.CompareTag("Flag") && _gameReferee.GetInteractedToken == this)
                {
                    _gameReferee.AddCargoToTheCannonCameraTargetGroup = other.gameObject.transform;
                }
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            ValidateTrigger(other);
        }

        private void OnDrawGizmos()
        {
            #if UNITY_EDITOR
            ValidateReferences();
            #endif
        }

        #endregion

        #region RuntimeMethods


        #endregion

        #region PublicMethods

        #endregion

        #region GettersSetters

        public bool SetAnchorZoneActive
        {
            set { anchorZone.transform.position = new Vector3(transform.position.x, 0.125f, transform.position.z); anchorZone.SetActive(value); }
        }

        public CinemachineVirtualCameraBase GetAnchorShipCamera
        {
            get { return _anchorShipCamera; }
        }

        #endregion
    }
}