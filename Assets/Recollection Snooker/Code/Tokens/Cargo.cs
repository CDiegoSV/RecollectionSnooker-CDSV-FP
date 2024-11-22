using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dante.RecollectionSnooker
{
    #region Enums

    public enum CargoTypes
    {
        CREW_MEMBER = 4,
        FUEL = 3,
        MEDICINE = 2,
        SUPPLIES = 1,
        SCREW_PART = 0
        //SCREW_PART,
        //SUPPLIES,
        //MEDICINE,
        //FUEL,
        //CREW_MEMBER
    }

    #endregion

    #region Structs


    #endregion

    public class Cargo : Token
    {
        #region Knobs

        [Header("Knobs / Parameters")]
        public CargoTypes cargoType;

        #endregion

        #region References

        #endregion

        #region RuntimeVariables

        [Header("Runtime Variables")]
        [SerializeField] protected bool _isLoaded;

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
                if (other.gameObject.CompareTag("Ship") && !_isLoaded)
                {
                    _gameReferee.CargoToBeLoaded = this;
                }
                
            }
            else if (_gameReferee.GetGameState == RS_GameStates.LOADING_AND_ORGANIZING_CARGO_BY_PLAYER)
            {
                if (other.gameObject.CompareTag("Ship"))
                {
                    _isLoaded = true;
                }
            }

        }

        private void OnCollisionExit(Collision other)
        {
            if (_gameReferee.GetGameState == RS_GameStates.LOADING_AND_ORGANIZING_CARGO_BY_PLAYER ||
                _gameReferee.GetGameState == RS_GameStates.CANNON_BY_NAVIGATION ||
                _gameReferee.GetGameState == RS_GameStates.CANNON_CARGO)
            {
                if (other.gameObject.CompareTag("Ship"))
                {
                    _isLoaded = false;
                }
            }
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

        public bool IsLoaded
        {
            get { return _isLoaded; }
            set { _isLoaded = value; }
        }

        #endregion
    }
}