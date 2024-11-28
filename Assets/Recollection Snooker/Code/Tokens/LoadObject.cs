using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dante.RecollectionSnooker
{
    #region Enums


    #endregion

    #region Structs


    #endregion

    public class LoadObject : Token
    {
        #region Knobs



        #endregion

        #region References


        #endregion

        #region RuntimeVariables

        [SerializeField] protected List<Cargo> _loadedCargo;

        #endregion

        #region UnityMethods

        void Start()
        {
            base.InitializeToken();
        }

        void Update()
        {

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
            foreach (Cargo cargo in _gameReferee.AllCargoOfTheGame)
            {
                if (TypeOfCargoIsLoaded(cargo) == false && _lastShorterDistance > Vector3.Distance(transform.position, cargo.transform.position) && !cargo.IsLoaded)
                {
                    _lastShorterDistance = Vector3.Distance(transform.position, cargo.transform.position);
                    _lastNearestCargo = cargo;
                }
            }

            return _lastNearestCargo;
        }

        #endregion

        #region GettersSetters

        public Cargo AddLoadedCargo
        {
            set { _loadedCargo.Add(value); }
        }

        public int GetLoadedCargoCount
        {
            get { return _loadedCargo.Count; }
        }
        #endregion
    }
}