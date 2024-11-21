using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dante.Game
{
    #region Enum

    public enum GameStates
    {
        START_GAME, //0
        GAME,
        PAUSE,
        VICTORY,
        FAILURE, //GAME_OVER
        DRAW
    }

    #endregion

    public class GameReferee : MonoBehaviour
    {
        #region RuntimeVariables

        protected GameStates _gameState;

        #endregion

        #region UnityMethods
        void Start()
        {
            InitializeGameReferee();
        }

        void Update()
        {

        }

        #endregion

        #region RuntimeMethods

        protected virtual void InitializeGameReferee()
        {
            _gameState = GameStates.START_GAME;
        }

        #endregion
    }
}

