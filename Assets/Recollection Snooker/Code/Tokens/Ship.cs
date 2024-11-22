using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dante.RecollectionSnooker
{
    #region Enums


    #endregion

    #region Structs


    #endregion

    public class Ship : Token
    {
        #region Knobs



        #endregion

        #region References

        [SerializeField] private Cargo[] allCargoOfTheGame;
        [SerializeField] private GameObject cargoZone;

        #endregion

        #region RuntimeVariables

        [SerializeField] private List<Cargo> _loadedCargo;

        #endregion

        #region UnityMethods

        void Start()
        {
            base.InitializeToken();
        }

        void Update()
        {

        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.collider.CompareTag("Cargo"))
            {
                switch (_gameReferee.GetGameState)
                {
                    case RS_GameStates.CHOOSE_TOKEN_BY_PLAYER:
                        collision.gameObject.GetComponent<Cargo>().StateMechanic(TokenStateMechanic.SET_RIGID);
                        collision.gameObject.transform.SetParent(transform);
                        break;
                    case RS_GameStates.CANNON_CARGO:
                        print(gameObject.name + ": OnCollisionEnter with " + collision.gameObject.name);
                        _gameReferee.SetACargoHasTouchedTheShip = true;
                        break;

                }
            }
        }

        private void OnCollisionExit(Collision collision)
        {
            if (collision.collider.CompareTag("Cargo"))
            {
                switch (_gameReferee.GetGameState)
                {
                    case RS_GameStates.CANNON_BY_NAVIGATION:
                    case RS_GameStates.CANNON_CARGO:
                        if (collision.gameObject.GetComponent<Cargo>().IsLoaded)
                        {
                            _loadedCargo.Remove(collision.gameObject.GetComponent<Cargo>());
                            foreach (Cargo cargo in allCargoOfTheGame)
                            {
                                if(cargo.cargoType == collision.gameObject.GetComponent<Cargo>().cargoType)
                                {
                                    cargo.IsAvalaibleForFlicking = true;
                                }
                            }
                        }
                        break;
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
        public bool TypeOfCargoIsLoaded(Cargo cargo)
        {
            if (_loadedCargo.Count > 0)
            {
                for (int i = 0; i < _loadedCargo.Count; i++)
                {
                    if (cargo.cargoType == _loadedCargo[i].cargoType)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public Cargo NearestAvailableCargo()
        {
            float _lastShorterDistance = 1000f;
            Cargo _lastNearestCargo = null;
            foreach (Cargo cargo in allCargoOfTheGame)
            {
                if (!TypeOfCargoIsLoaded(cargo) && _lastShorterDistance > Vector3.Distance(transform.position, cargo.transform.position) || !cargo.IsLoaded)
                {
                    _lastShorterDistance = Vector3.Distance(transform.position, cargo.transform.position);
                    _lastNearestCargo = cargo;
                }
            }

            return _lastNearestCargo;
        }

        #endregion

        #region GettersSetters

        public bool SetCargoZone
        {
            set { cargoZone.SetActive(value); }
        }

        public Cargo AddLoadedCargo
        {
            set { _loadedCargo.Add(value); }
        }
        #endregion
    }
}