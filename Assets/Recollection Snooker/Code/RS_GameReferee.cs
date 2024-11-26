using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dante.Game;
using Cinemachine;
using TMPro;
using Unity.VisualScripting;

namespace Dante.RecollectionSnooker
{
    #region Enum

    public enum RS_GameStates
    {
        //START OF THE GAME
        SHOW_THE_LAYOUT_TO_THE_PLAYER,
        //PLAYER ORDINARY TURN STATES
        CHOOSE_TOKEN_BY_PLAYER,
        CONTACT_POINT_TOKEN_BY_PLAYER,
        FLICK_TOKEN_BY_PLAYER,
        CANNON_BY_NAVIGATION,
        NAVIGATING_SHIP_OF_THE_PLAYER, //Including loading treasures to the island
        ANCHOR_SHIP,
        CANNON_CARGO,
        LOADING_AND_ORGANIZING_CARGO_BY_PLAYER,
        MOVE_COUNTER_BY_SANCTION,
        //END OF THE TURN
        SHIFT_MONSTER_PARTS,
        //META MECHANICS
        VICTORY_OF_THE_PLAYER,
        FAILURE_OF_THE_PLAYER
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
        [SerializeField] protected Ship shipOfTheGame;
        [SerializeField] protected ShipPivot shipPivotsOfTheGame;
        [SerializeField] protected MonsterPart monsterHead;

        [Header("Camera References")]
        [SerializeField] protected CinemachineFreeLook tableFreeLookCamera;
        [SerializeField] protected CinemachineVirtualCameraBase shipVirtualCamera;

        [Header("Flags")]
        [SerializeField] protected GameObject flag;

        [SerializeField] protected TextMeshProUGUI debugText;

        [Header("Random Token Positions")]
        [SerializeField] protected List<Transform> tokenPositionsList;

        #endregion

        #region RuntimeVariables

        protected new RS_GameStates _gameState;
        protected CinemachineVirtualCameraBase _currentVirtualCameraBase;
        protected Token _interactedToken;
        protected bool _isAllCargoStill;
        protected int _randomTokenPos;
        [SerializeField] protected Cargo _nearestAvailableCargoToTheShip;
        protected bool _aCargoCollidedWithMonsterPart;
        [SerializeField] protected Cargo _cargoToBeLoaded;
        protected bool _aCargoHasTouchedTheShip;
        protected Vector3 _originalPositionOfTheFlag;

        #endregion

        #region UnityMethods

        void Start()
        {
            InitializeGameReferee();
        }

        private void FixedUpdate()
        {
            print(_gameState);
            switch (_gameState)
            {
                case RS_GameStates.SHOW_THE_LAYOUT_TO_THE_PLAYER:
                    ExecutingShowTheLayoutToThePlayerState();
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
                case RS_GameStates.CANNON_BY_NAVIGATION:
                    ExecutingCannonByNavigationState();
                    break;
                case RS_GameStates.NAVIGATING_SHIP_OF_THE_PLAYER:
                    ExecutingNavigatingShipOfThePlayerState();
                    break;
                case RS_GameStates.ANCHOR_SHIP:
                    ExecutingAnchorShipState();
                    break;
                case RS_GameStates.CANNON_CARGO:
                    ExecutingCannonCargoState();
                    break;
                case RS_GameStates.LOADING_AND_ORGANIZING_CARGO_BY_PLAYER:
                    ExecutingLoadingAndOrganizingCargoByPlayerState();
                    break;
                case RS_GameStates.MOVE_COUNTER_BY_SANCTION:
                    ExecutingMoveCounterBySanctionState();
                    break;
                case RS_GameStates.SHIFT_MONSTER_PARTS:
                    ExecutingShiftMonsterPartsState();
                    break;
                case RS_GameStates.VICTORY_OF_THE_PLAYER:
                    ExecutingVictoryOfThePlayerState();
                    break;
                case RS_GameStates.FAILURE_OF_THE_PLAYER:
                    ExecutingFailureOfThePlayerState();
                    break;
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
                    if (_gameState == RS_GameStates.SHIFT_MONSTER_PARTS ||
                        _gameState == RS_GameStates.SHOW_THE_LAYOUT_TO_THE_PLAYER)
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
                case RS_GameStates.CANNON_BY_NAVIGATION:
                    if (_gameState == RS_GameStates.FLICK_TOKEN_BY_PLAYER)
                    {
                        FinalizeCurrentState(toNextState);
                    }
                    break;
                case RS_GameStates.NAVIGATING_SHIP_OF_THE_PLAYER:
                    if (_gameState == RS_GameStates.CANNON_BY_NAVIGATION)
                    {
                        FinalizeCurrentState(toNextState);
                    }
                    break;
                case RS_GameStates.ANCHOR_SHIP:
                    if (_gameState == RS_GameStates.NAVIGATING_SHIP_OF_THE_PLAYER)
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
                case RS_GameStates.LOADING_AND_ORGANIZING_CARGO_BY_PLAYER:
                    if (_gameState == RS_GameStates.CANNON_CARGO)
                    {
                        FinalizeCurrentState(toNextState);
                    }
                    break;
                case RS_GameStates.MOVE_COUNTER_BY_SANCTION:
                    if (_gameState == RS_GameStates.CANNON_CARGO)
                    {
                        FinalizeCurrentState(toNextState);
                    }
                    break;
                case RS_GameStates.SHIFT_MONSTER_PARTS:
                    if (_gameState == RS_GameStates.LOADING_AND_ORGANIZING_CARGO_BY_PLAYER ||
                        _gameState == RS_GameStates.MOVE_COUNTER_BY_SANCTION ||
                        _gameState == RS_GameStates.CANNON_CARGO ||
                        _gameState == RS_GameStates.CANNON_BY_NAVIGATION)
                    {
                        FinalizeCurrentState(toNextState);
                    }
                    break;
                case RS_GameStates.VICTORY_OF_THE_PLAYER:
                    if (_gameState == RS_GameStates.NAVIGATING_SHIP_OF_THE_PLAYER)
                    {
                        FinalizeCurrentState(toNextState);
                    }
                    break;
                case RS_GameStates.FAILURE_OF_THE_PLAYER:
                    if (_gameState == RS_GameStates.MOVE_COUNTER_BY_SANCTION)
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
            _gameState = RS_GameStates.SHOW_THE_LAYOUT_TO_THE_PLAYER;  //(RS_GameStates)0;
            //InitializeDropCargoState();
            InitializeState();
        }

        protected void InitializeState()
        {
            switch(_gameState)
            {
                case RS_GameStates.SHOW_THE_LAYOUT_TO_THE_PLAYER:
                    InitializeShowTheLayoutToThePlayerState();
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
                case RS_GameStates.CANNON_BY_NAVIGATION:
                    InitializeCannonByNavigationState();
                    break;
                case RS_GameStates.NAVIGATING_SHIP_OF_THE_PLAYER:
                    InitializeNavigatingShipOfThePlayerState();
                    break;
                case RS_GameStates.ANCHOR_SHIP:
                    InitializeAnchorShipState();
                    break;
                case RS_GameStates.CANNON_CARGO:
                    InitializeCannonCargoState();
                    break;
                case RS_GameStates.LOADING_AND_ORGANIZING_CARGO_BY_PLAYER:
                    InitializeLoadingAndOrganizingCargoByPlayerState();
                    break;
                case RS_GameStates.MOVE_COUNTER_BY_SANCTION:
                    InitializeMoveCounterBySanctionState();
                    break;
                case RS_GameStates.SHIFT_MONSTER_PARTS:
                    InitializeShiftMonsterPartsState();
                    break;
                case RS_GameStates.VICTORY_OF_THE_PLAYER:
                    InitializeVictoryOfThePlayerState();
                    break;
                case RS_GameStates.FAILURE_OF_THE_PLAYER:
                    InitializeFailureOfThePlayerState();
                    break;
            }
        }

        protected void FinalizeState()
        {
            switch (_gameState)
            {
                case RS_GameStates.SHOW_THE_LAYOUT_TO_THE_PLAYER:
                    FinalizeShowTheLayoutToThePlayerState();
                    break;
                case RS_GameStates.CHOOSE_TOKEN_BY_PLAYER:
                    FinalizeChooseTokenByPlayerState();
                    break;
                case RS_GameStates.CONTACT_POINT_TOKEN_BY_PLAYER:
                    FinalizeContactPointTokenByPlayerState();
                    break;
                case RS_GameStates.FLICK_TOKEN_BY_PLAYER:
                    FinalizeFlickTokenByPlayerState();
                    break;
                case RS_GameStates.CANNON_BY_NAVIGATION:
                    FinalizeCannonByNavigationState();
                    break;
                case RS_GameStates.NAVIGATING_SHIP_OF_THE_PLAYER:
                    FinalizeNavigatingShipOfThePlayerState();
                    break;
                case RS_GameStates.ANCHOR_SHIP:
                    FinalizeAnchorShipState();
                    break;
                case RS_GameStates.CANNON_CARGO:
                    FinalizeCannonCargoState();
                    break;
                case RS_GameStates.LOADING_AND_ORGANIZING_CARGO_BY_PLAYER:
                    FinalizeLoadingAndOrganizingCargoByPlayerState();
                    break;
                case RS_GameStates.SHIFT_MONSTER_PARTS:
                    FinalizeShiftMonsterPartsState();
                    break;
                case RS_GameStates.VICTORY_OF_THE_PLAYER:
                    FinalizeVictoryOfThePlayerState();
                    break;
                case RS_GameStates.FAILURE_OF_THE_PLAYER:
                    FinalizeFailureOfThePlayerState();
                    break;
            }
        }


        protected void ChangeCameraTo(CinemachineVirtualCameraBase nextCamera)
        {
            if (_currentVirtualCameraBase != null)
            {
                _currentVirtualCameraBase.Priority = 1;
            }
            _currentVirtualCameraBase = nextCamera;
            _currentVirtualCameraBase.Priority = 10;
        }

        #endregion

        #region FiniteStateMachineMethods

        #region ShowTheLayoutToThePlayer

        protected void InitializeShowTheLayoutToThePlayerState()
        {
            _aCargoHasTouchedTheShip = false;
            _aCargoCollidedWithMonsterPart = false;
            _originalPositionOfTheFlag = flag.transform.localPosition;


            foreach (Cargo cargo in allCargoOfTheGame)
            {
                _randomTokenPos = Random.Range(0, tokenPositionsList.Count -1);
                cargo.gameObject.transform.position = tokenPositionsList[_randomTokenPos].position;
                tokenPositionsList.RemoveAt(_randomTokenPos);
            }


            foreach (Cargo cargo in allCargoOfTheGame)
            {
                cargo.StateMechanic(TokenStateMechanic.SET_SPOOKY);
            }

            if (!IsAllCargoStill())
            {
                GameStateMechanic(RS_GameStates.CHOOSE_TOKEN_BY_PLAYER);
            }
        }

        protected void ExecutingShowTheLayoutToThePlayerState()
        {

        }

        protected void FinalizeShowTheLayoutToThePlayerState()
        {

        }

        #endregion ShowTheLayoutToThePlayer

        #region ChooseTokenByPlayer

        protected void InitializeChooseTokenByPlayerState()
        {
            _nearestAvailableCargoToTheShip = shipOfTheGame.NearestAvailableCargo();

            _nearestAvailableCargoToTheShip.IsAvalaibleForFlicking = false;

            //All cargo is set to Spooky
            foreach (Cargo cargo in allCargoOfTheGame)
            {
                if (!cargo.IsLoaded)
                {
                    cargo.StateMechanic(TokenStateMechanic.SET_SPOOKY);
                }
            }
            //TODO: Set Spooky for ships, monster parts and ship pivots

            //Activate the table camera (with the highest priority)
            ChangeCameraTo(tableFreeLookCamera);

            //Check available cargo for flicking
            foreach (Cargo cargo in allCargoOfTheGame)
            {
                if (!cargo.IsLoaded && !shipOfTheGame.TypeOfCargoIsLoaded(cargo) && cargo != _nearestAvailableCargoToTheShip)
                {
                    cargo.SetHighlight(true);
                    cargo.IsAvalaibleForFlicking = true;
                }
            }
        }

        protected void ExecutingChooseTokenByPlayerState()
        {

        }

        protected void FinalizeChooseTokenByPlayerState()
        {
            //table free look camera
            _currentVirtualCameraBase.Priority = 1;
        }

        #endregion ChooseTokenByPlayer

        #region ContactPointTokenByPlayer

        protected void InitializeContactPointTokenByPlayerState()
        {
            _interactedToken.StateMechanic(TokenStateMechanic.SET_PHYSICS);
            //Focus to the camera rig of the selected token
            _currentVirtualCameraBase = _interactedToken.GetFreeLookCamera;
            _currentVirtualCameraBase.Priority = 1000;
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
            _currentVirtualCameraBase.gameObject.GetComponent<CinemachineMobileInputProvider>().enableCameraRig = false;
            CinemachineFreeLook tempCam = (CinemachineFreeLook)_currentVirtualCameraBase;
            tempCam.m_YAxis.Value = 0.0f;
            flag.gameObject.SetActive(true);
        }

        protected void ExecutingFlickTokenByPlayerState()
        {

        }

        protected void FinalizeFlickTokenByPlayerState()
        {
            flag.gameObject.SetActive(false);
            flag.transform.rotation = Quaternion.identity;
            flag.gameObject.transform.localPosition = _originalPositionOfTheFlag;
            _nearestAvailableCargoToTheShip.gameObject.SetActive(true);
            _nearestAvailableCargoToTheShip = null;
        }

        #endregion

        #region CannonByNavigation

        protected void InitializeCannonByNavigationState()
        {

        }

        protected void ExecutingCannonByNavigationState()
        {

        }

        protected void FinalizeCannonByNavigationState()
        {

        }

        #endregion

        #region NavigatingShipOfThePlayer

        protected void InitializeNavigatingShipOfThePlayerState()
        {

        }

        protected void ExecutingNavigatingShipOfThePlayerState()
        {

        }

        protected void FinalizeNavigatingShipOfThePlayerState()
        {

        }

        #endregion

        #region AnchorShip

        protected void InitializeAnchorShipState()
        {

        }

        protected void ExecutingAnchorShipState()
        {

        }

        protected void FinalizeAnchorShipState()
        {

        }

        #endregion

        #region CannonCargo

        protected void InitializeCannonCargoState()
        {
            _nearestAvailableCargoToTheShip?.gameObject.SetActive(true);
            _nearestAvailableCargoToTheShip = null;
            _aCargoCollidedWithMonsterPart = false;

            foreach (Cargo cargo in allCargoOfTheGame)
            {
                cargo.StateMechanic(TokenStateMechanic.SET_PHYSICS);
            }
        }

        protected void ExecutingCannonCargoState()
        {
            if (IsAllCargoStill())
            {
                if (_aCargoCollidedWithMonsterPart)
                {
                    GameStateMechanic(RS_GameStates.MOVE_COUNTER_BY_SANCTION);
                    _aCargoCollidedWithMonsterPart = false;
                }
                else
                {
                    if ((_aCargoHasTouchedTheShip) && (_cargoToBeLoaded != null))
                    {
                        GameStateMechanic(RS_GameStates.LOADING_AND_ORGANIZING_CARGO_BY_PLAYER);
                        _aCargoHasTouchedTheShip = false;
                    }
                    else
                    {
                        GameStateMechanic(RS_GameStates.SHIFT_MONSTER_PARTS);
                    }
                }

                //TODO: Pending validation events while the cannon was executing
                //A) LOAD_CARGO_BY_PLAYER
                //B) MOVE_COUNTER
                //C) FINALIZE_TURN (Respawn of the monster parts)
            }
        }

        protected void FinalizeCannonCargoState()
        {
            foreach (Cargo cargo in allCargoOfTheGame)
            {
                cargo.StateMechanic(TokenStateMechanic.SET_SPOOKY);
            }
        }

        #endregion

        #region LoadingAndOrganizingCargoByPlayer

        protected void InitializeLoadingAndOrganizingCargoByPlayerState()
        {
            ChangeCameraTo(shipVirtualCamera);
            shipOfTheGame.SetCargoZone = true;

            
            _cargoToBeLoaded.StateMechanic(TokenStateMechanic.SET_SPOOKY);
            _cargoToBeLoaded.IsLoaded = true;

        }

        protected void ExecutingLoadingAndOrganizingCargoByPlayerState()
        {

        }

        protected void FinalizeLoadingAndOrganizingCargoByPlayerState()
        {
            shipOfTheGame.SetCargoZone = false;
            shipOfTheGame.AddLoadedCargo = _cargoToBeLoaded;

            foreach (Cargo cargo in allCargoOfTheGame)
            {
                cargo.gameObject.SetActive(true);
                if (cargo.cargoType == _cargoToBeLoaded.cargoType)
                {
                    cargo.StateMechanic(TokenStateMechanic.SET_PHYSICS);
                    cargo.IsAvalaibleForFlicking = false;
                }
            }

            _cargoToBeLoaded = null;
        }

        #endregion

        #region MoveCounterBySanction

        protected void InitializeMoveCounterBySanctionState()
        {

        }

        protected void ExecutingMoveCounterBySanctionState()
        {

        }

        protected void FinalizeMoveCounterBySanctionState()
        {

        }

        #endregion

        #region ShiftMonsterParts

        protected void InitializeShiftMonsterPartsState()
        {
            foreach (MonsterPart monsterPart in allMonsterPartOfTheGame)
            {
                monsterPart.ValidateSpaceToSpawnMonsterParts();
            }

            GameStateMechanic(RS_GameStates.CHOOSE_TOKEN_BY_PLAYER);
        }

        protected void ExecutingShiftMonsterPartsState()
        {

        }

        protected void FinalizeShiftMonsterPartsState()
        {

        }

        #endregion

        #region VictoryOfThePlayer

        protected void InitializeVictoryOfThePlayerState()
        {

        }

        protected void ExecutingVictoryOfThePlayerState()
        {

        }

        protected void FinalizeVictoryOfThePlayerState()
        {

        }

        #endregion

        #region FailureOfThePlayer

        protected void InitializeFailureOfThePlayerState()
        {

        }

        protected void ExecutingFailureOfThePlayerState()
        {

        }

        protected void FinalizeFailureOfThePlayerState()
        {

        }

        #endregion

        #endregion FiniteStateMachineMethods

        #region GettersAndSetters

        public RS_GameStates GetGameState
        {
            get { return _gameState; }
        }

        public Token SetInteractedToken
        {
            set { _interactedToken = value; }
        }

        public GameObject GetCurrentFlag
        {
            get { return flag; }
        }

        public bool SetCargoCollidedWithMonsterPart
        {
            set { _aCargoCollidedWithMonsterPart = value; }
        }

        public Cargo CargoToBeLoaded
        {
            set { _cargoToBeLoaded = value; }
            get { return _cargoToBeLoaded; }
        }

        public bool SetACargoHasTouchedTheShip
        {
            set { _aCargoHasTouchedTheShip = value; }
        }

        public Cargo[] AllCargoOfTheGame
        {
            get { return allCargoOfTheGame; }
        }

        #endregion
    }
}