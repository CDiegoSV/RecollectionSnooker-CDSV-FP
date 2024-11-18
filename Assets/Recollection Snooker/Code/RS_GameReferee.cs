using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SotomaYorch.Game;
using Cinemachine;
using TMPro;
using Unity.VisualScripting;

namespace SotomaYorch.RecollectionSnooker
{
    #region Enum

    public enum RS_GameStates
    {
        //START OF THE GAME
        DROP_CARGO,
        CANNON_BY_DROPPED_CARGO,
        //PLAYER ORDINARY TURN STATES
        CHOOSE_TOKEN_BY_PLAYER,
        CONTACT_POINT_TOKEN_BY_PLAYER,
        FLICK_TOKEN_BY_PLAYER,
        CANNON_BY_NAVIGATION,
        NAVIGATING_SHIP_OF_THE_PLAYER,
        ANCHOR_SHIP_BY_PLAYER,
        CANNON_CARGO,
        LOADING_CARGO_BY_PLAYER,
        ORGANIZE_CARGO_BY_PLAYER,
        MOVE_COUNTER,
        DROP_CARGO_BY_PREVIOUS_PLAYER,
        //MONSTER ATTACK
        PREPARING_MONSTER_ATTACK,
        //PLAYER MONSTER ATTACK STATES
        TURN_DROP_DICE_BY_PLAYER,
        CANNON_DICE,
        MOVE_LIMB_BY_PLAYER,
        MOVE_BODY_BY_PLAYER,
        //META MECHANICS
        VICTORY_OF_THE_PLAYER
    }

    public enum PlayerIndex
    {
        ONE,
        TWO,
        THREE,
        FOUR
    }

    #endregion

    public class RS_GameReferee : GameReferee
    {
        #region References

        //Polymorphism by prefab variants and code
        //for every type of token in the game
        [Header("Token References")]
        [SerializeField] protected Cargo[] allCargoOfTheGame;
        [SerializeField] protected MonsterPart[] allMonsterPartOfTheGame;
        [SerializeField] protected Ship[] allShipsOfTheGame;
        [SerializeField] protected ShipPivot[] allShipPivotsOfTheGame;
        [SerializeField] protected MonsterPart monsterHead;

        [Header("Camera References")]
        [SerializeField] protected CinemachineFreeLook tableFreeLookCamera;

        [Header("Flags")]
        [SerializeField] protected GameObject[] flags;

        [SerializeField] protected TextMeshProUGUI debugText;

        #endregion

        #region RuntimeVariables

        [SerializeField] protected new RS_GameStates _gameState;
        protected PlayerIndex _playerIndex;
        protected CinemachineFreeLook _currentFreeLookCamera;
        [SerializeField] protected GameObject _currentFlag;
        protected Token _interactedToken;
        [SerializeField] protected bool _isAllCargoStill;

        #endregion

        #region UnityMethods

        void Start()
        {
            InitializeGameReferee();
        }

        private void FixedUpdate()
        {
            switch (_gameState)
            {
                case RS_GameStates.DROP_CARGO:
                    ExecutingDropCargoState();
                    break;
                case RS_GameStates.CHOOSE_TOKEN_BY_PLAYER:
                    ExecutingChooseTokenByPlayerState();
                    break;
                case RS_GameStates.CONTACT_POINT_TOKEN_BY_PLAYER:
                    ExecutingContactPointTokenByPlayerState();
                    break;
                case RS_GameStates.FLICK_TOKEN_BY_PLAYER:
                    ExecutingFlickTokenByPlayerState();
                    break;
                case RS_GameStates.CANNON_CARGO:
                    ExecutingCannonCargoState();
                    break;
                    //TODO: Pending remaining states
            }
        }

        #endregion

        #region PublicMethods

        public void DebugInMobile(string value)
        {
            debugText.text = value;
        }

        public void GameStateMechanic(RS_GameStates toNextState)
        {
            //let's validate the possibility to go
            //to the suggested next state
            switch (toNextState)
            {
                case RS_GameStates.CHOOSE_TOKEN_BY_PLAYER:
                    if (_gameState == RS_GameStates.CANNON_CARGO)
                    {
                        FinalizeCurrentState(toNextState);
                    }
                    break;
                case RS_GameStates.CONTACT_POINT_TOKEN_BY_PLAYER:
                    if (_gameState == RS_GameStates.CHOOSE_TOKEN_BY_PLAYER ||
                        _gameState == RS_GameStates.FLICK_TOKEN_BY_PLAYER)
                    {
                        FinalizeCurrentState(toNextState);
                    }
                    break;
                case RS_GameStates.FLICK_TOKEN_BY_PLAYER:
                    if (_gameState == RS_GameStates.CONTACT_POINT_TOKEN_BY_PLAYER)
                    {
                        FinalizeCurrentState(toNextState);
                    }
                    break;
                case RS_GameStates.CANNON_CARGO:
                    if (_gameState == RS_GameStates.FLICK_TOKEN_BY_PLAYER)
                    {
                        FinalizeCurrentState(toNextState);
                    }
                    break;
            }
        }

        #endregion

        #region RuntimeMethods

        protected void FinalizeCurrentState(RS_GameStates toNextState)
        {
            FinalizeState();
            _gameState = toNextState;
            InitializeState();
        }

        protected bool IsAllCargoStill()
        {
            _isAllCargoStill = true;
            foreach (Token token in allCargoOfTheGame)
            {
                if (!token.IsStill)
                {
                    _isAllCargoStill = false;
                    break;
                }
            }
            return _isAllCargoStill;
        }

        protected override void InitializeGameReferee()
        {
            _gameState = RS_GameStates.CHOOSE_TOKEN_BY_PLAYER;  //(RS_GameStates)0;
            //InitializeDropCargoState();
            InitializeState();
        }

        protected void InitializeState()
        {
            switch(_gameState)
            {
                case RS_GameStates.DROP_CARGO:
                    InitializeDropCargoState();
                    break;
                case RS_GameStates.CHOOSE_TOKEN_BY_PLAYER:
                    InitializeChooseTokenByPlayerState();
                    break;
                case RS_GameStates.CONTACT_POINT_TOKEN_BY_PLAYER:
                    InitializeContactPointTokenByPlayerState();
                    break;
                case RS_GameStates.FLICK_TOKEN_BY_PLAYER:
                    InitializeFlickTokenByPlayerState();
                    break;
                    //TODO: Pending remaining states
            }
        }

        protected void FinalizeState()
        {
            switch (_gameState)
            {
                case RS_GameStates.DROP_CARGO:
                    ExitDropCargoState();
                    break;
                case RS_GameStates.CHOOSE_TOKEN_BY_PLAYER:
                    ExitChooseTokenByPlayerState();
                    break;
                case RS_GameStates.CONTACT_POINT_TOKEN_BY_PLAYER:
                    FinalizeContactPointTokenByPlayerState();
                    break;
                case RS_GameStates.FLICK_TOKEN_BY_PLAYER:
                    FinalizeFlickTokenByPlayerState();
                    break;
                    //TODO: Pending remaining states
            }
        }

        protected void DebugInConsole(string value)
        {
            Debug.Log(
                    gameObject.name + ": " +
                    this.name + " - " +
                    value
                );
        }

        #endregion

        //for every state, we will handle
        //Intialize___State
        //Manage___State
        //Exit___State
        #region FiniteStateMachineMethods

        #region DropCargo

        protected void InitializeDropCargoState()
        {
            //TODO: Make the proper initialization of the state

            //Setting all cargo in the GHOST state
            foreach (Cargo cargo in allCargoOfTheGame)
            {
                cargo.StateMechanic(TokenStateMechanic.SET_SPOOKY);
            }
        }

        protected void ExecutingDropCargoState()
        {

        }

        protected void ExitDropCargoState()
        {

        }

        #endregion DropCargo

        #region ChooseTokenByPlayer

        protected void InitializeChooseTokenByPlayerState()
        {
            //All cargo is set to Spooky
            foreach (Token cargo in allCargoOfTheGame)
            {
                cargo.StateMechanic(TokenStateMechanic.SET_PHYSICS);
            }
            //TODO: Set Spooky for ships, monster parts and ship pivots

            //Activate the table camera (with the highest priority)
            if (_currentFreeLookCamera != null)
            {
                _currentFreeLookCamera.Priority = 1;
            }
            _currentFreeLookCamera = tableFreeLookCamera;
            _currentFreeLookCamera.Priority = 1000;

            //Check available cargo for flicking
            foreach (Cargo cargo in allCargoOfTheGame)
            {
                if (!cargo.IsLoaded)
                {
                    cargo.SetHighlight(true);
                    cargo.IsAvalaibleForFlicking = true;
                }
            }
        }

        protected void ExecutingChooseTokenByPlayerState()
        {

        }

        protected void ExitChooseTokenByPlayerState()
        {
            //table free look camera
            _currentFreeLookCamera.Priority = 1;
        }

        #endregion ChooseTokenByPlayer

        #region ContactPointTokenByPlayer

        protected void InitializeContactPointTokenByPlayerState()
        {
            _interactedToken.StateMechanic(TokenStateMechanic.SET_PHYSICS);
            //Focus to the camera rig of the selected token
            _currentFreeLookCamera = _interactedToken.GetFreeLookCamera;
            _currentFreeLookCamera.Priority = 1000;
        }

        protected void ExecutingContactPointTokenByPlayerState()
        {
            
        }

        protected void FinalizeContactPointTokenByPlayerState()
        {

        }

        #endregion ContactPointTokenByPlayer

        #region FlickTokenByPlayer

        protected void InitializeFlickTokenByPlayerState()
        {
            //this virtual camera hasn't changed from the previous
            //state, so this is the camera from the selected token
            _currentFreeLookCamera.gameObject.GetComponent<CinemachineMobileInputProvider>().enableCameraRig = false;
            _currentFreeLookCamera.m_YAxis.Value = 0.0f;
            _currentFlag = flags[(int)_playerIndex];
            _currentFlag.gameObject.SetActive(true);
        }

        protected void ExecutingFlickTokenByPlayerState()
        {

        }

        protected void FinalizeFlickTokenByPlayerState()
        {
            _currentFlag.transform.localRotation = Quaternion.identity;
            _currentFlag.gameObject.SetActive(false);
        }

        #endregion

        #region CannonCargo

        protected void InitializeCannonCargoState()
        {
            
        }

        protected void ExecutingCannonCargoState()
        {
            if (IsAllCargoStill())
            {
                GameStateMechanic(RS_GameStates.CHOOSE_TOKEN_BY_PLAYER);
                //TODO: Pending validation events while the cannon was executing
                //A) LOAD_CARGO_BY_PLAYER
                //B) MOVE_COUNTER
                //C) FINALIZE_TURN (Respawn of the monster parts)
            }
        }

        protected void FinalizeCannonCargoState()
        {

        }

        #endregion

        #region CannonByDroppedCargo


        #endregion CannonByDroppedCargo 

        #endregion FiniteStateMachineMethods

        #region GettersAndSetters

        public RS_GameStates GetGameState
        {
            get { return _gameState; }
        }

        public Token SetInteractedToken
        {
            set {
                _interactedToken = value;
                //DebugInConsole("SetInteractedToken - " + _interactedToken.gameObject.name);
            }
        }

        public GameObject GetCurrentFlag
        {
            get { return _currentFlag; }
        }

        #endregion
    }
}