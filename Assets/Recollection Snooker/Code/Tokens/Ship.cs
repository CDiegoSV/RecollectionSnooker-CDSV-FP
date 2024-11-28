using Dante.RecollectionSnooker;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ship : LoadObject
{
    #region References

    [SerializeField] protected GameObject cargoZone;

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
                        foreach (Cargo cargo in _gameReferee.AllCargoOfTheGame)
                        {
                            if (cargo.cargoType == collision.gameObject.GetComponent<Cargo>().cargoType)
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

    #region Public Methods

    public void LoadToTheIslandAllTheCargo(Transform[] cargoPositions, Transform islandTransform)
    {
        if(_loadedCargo.Count > 0)
        {
            for (int i = 0; i < cargoPositions.Length; i++)
            {
                if (!islandTransform.gameObject.GetComponent<Island>().TypeOfCargoIsLoaded(_loadedCargo[i]))
                {
                    _loadedCargo[i].gameObject.transform.position = cargoPositions[islandTransform.gameObject.GetComponent<Island>().GetLoadedCargoCount].position;
                    _loadedCargo[i].gameObject.transform.SetParent(islandTransform);
                    _loadedCargo.Remove(_loadedCargo[i]);
                    islandTransform.gameObject.GetComponent<Island>().AddLoadedCargo = _loadedCargo[i];
                    _loadedCargo[i].StateMechanic(TokenStateMechanic.SET_RIGID);
                }
            }
        }
    }

    #endregion

    #region GettersSetters

    public bool SetCargoZone
    {
        set { cargoZone.SetActive(value); }
    }

    #endregion
}
