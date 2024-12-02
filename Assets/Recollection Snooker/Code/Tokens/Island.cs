using Dante.RecollectionSnooker;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Island : LoadObject
{
    #region Runtime Variables

    [SerializeField] protected bool aShipPivotTouchedTheIsland;
    [SerializeField] protected Transform[] cargoLoadedPositions;

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
        if (collision.collider.CompareTag("ShipPivot"))
        {
            switch (_gameReferee.GetGameState)
            {
                case RS_GameStates.CANNON_BY_NAVIGATION:
                    _gameReferee.GetTheShipOfTheGame.LoadToTheIslandAllTheCargo(cargoLoadedPositions, transform);
                    CheckWinCondition();
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

    #region Runtime Methods

    private void CheckWinCondition()
    {
        if(_loadedCargo.Count == 5)
        {
            print("You Win");
            _gameReferee.GameStateMechanic(RS_GameStates.VICTORY_OF_THE_PLAYER);
        }
    }

    #endregion
}
