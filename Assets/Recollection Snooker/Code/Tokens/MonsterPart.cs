using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dante.RecollectionSnooker
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
        protected RaycastHit _raycastHit;

        #endregion

        #region RuntimeVariables

        protected Vector3 _pushDirection;

        protected bool _canBePlacedAtPosition;
        protected Vector3 _randomPosToPlace;

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

        public void ValidateSpaceToSpawnMonsterParts()
        {
            if(this.monsterPartType == MonsterPartType.LIMB)
            {
                _canBePlacedAtPosition = false;
                while(_canBePlacedAtPosition ==  false)
                {
                    _tokenPhysicalFSM.StateMechanic(TokenStateMechanic.SET_SPOOKY);
                    _randomPosToPlace = new Vector3(Random.Range(-20, 20), 0, Random.Range(-20, 20));
                    transform.position = _randomPosToPlace;
                    if (Physics.SphereCast(new Vector3(_randomPosToPlace.x, _randomPosToPlace.y + 5.0f, _randomPosToPlace.z), 1.0f, -transform.up * 1.0f, out _raycastHit, 5.0f))
                    {
                        if (_raycastHit.collider.gameObject.GetComponent<Token>())
                        { 
                            _tokenPhysicalFSM.StateMechanic(TokenStateMechanic.SET_SPOOKY); _canBePlacedAtPosition = false; transform.position = _randomPosToPlace; 
                        }
                        else 
                        { 
                            _canBePlacedAtPosition = true; _tokenPhysicalFSM.StateMechanic(TokenStateMechanic.SET_PHYSICS); transform.position = _randomPosToPlace; 
                        }
                    }
                }
            }
        }

        #endregion

        #region GettersSetters

        #endregion
    }
}