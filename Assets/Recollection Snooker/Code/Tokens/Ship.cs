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
                    case RS_GameStates.CANNON_CARGO:
                        print(gameObject.name + ": OnCollisionEnter with " + collision.gameObject.name);
                        _gameReferee.SetACargoHasTouchedTheShip = true;
                        break;
                    case RS_GameStates.LOADING_AND_ORGANIZING_CARGO_BY_PLAYER:
                        if (!collision.gameObject.GetComponent<Cargo>().IsLoaded)
                        {
                            _loadedCargo.Add(collision.gameObject.GetComponent<Cargo>());
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

        private bool TypeOfCargoIsLoaded(Cargo cargo)
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

        #endregion

        #region PublicMethods

        public Cargo NearestAvailableCargo()
        {
            float _lastShorterDistance = 100f;
            Cargo _lastNearestCargo = null;
            foreach (Cargo cargo in allCargoOfTheGame)
            {
                if (!TypeOfCargoIsLoaded(cargo) && _lastShorterDistance > Vector3.Distance(transform.position, cargo.transform.position))
                {
                    _lastNearestCargo = cargo;
                    _lastShorterDistance = Vector3.Distance(transform.position, cargo.transform.position);
                }
                Debug.Log(_lastShorterDistance.ToString());
            }

            if (_lastNearestCargo != null)
            {
                return _lastNearestCargo;
            }
            else
            {
                return null;
            }
        }

        #endregion

        #region GettersSetters

        #endregion
    }
}