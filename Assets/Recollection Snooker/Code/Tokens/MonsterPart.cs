using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SotomaYorch.RecollectionSnooker
{
    #region Enums

    public enum MonsterPartType
    {
        LIMB,
        HEAD
    }

    #endregion

    #region Structs


    #endregion

    public class MonsterPart : Token
    {
        #region Knobs

        [Header("Knobs / Parameters")]
        public MonsterPartType monsterPartType;

        #endregion

        #region References

        #endregion

        #region RuntimeVariables

        protected Vector3 _pushDirection;

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

        private void OnCollisionStay(Collision other)
        {
            if (this.monsterPartType == MonsterPartType.HEAD
                && (other.gameObject.tag == "Dice" || other.gameObject.tag == "Cargo")
                )
            {
                //Debug.Log("Push from " + gameObject.name + " to " + other.gameObject.name);
                _pushDirection = (other.transform.position -
                    this.transform.position).normalized;
                other.gameObject.GetComponent<RS_TokenFiniteStateMachine>()?.ThrowToken(_pushDirection * 2.0f);
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

        #endregion
    }
}