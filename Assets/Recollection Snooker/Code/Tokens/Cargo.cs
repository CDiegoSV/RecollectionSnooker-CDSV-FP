using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SotomaYorch.RecollectionSnooker
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
        [SerializeField] protected bool isLoaded;

        #endregion

        #region UnityMethods

        void Start()
        {
            base.InitializeToken();
        }

        void Update()
        {

        }

        //private void OnCollisionEnter(Collision other)
        //{
        //    ValidateCollision(other);
        //}

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
            get { return isLoaded; }
        }

        #endregion
    }
}