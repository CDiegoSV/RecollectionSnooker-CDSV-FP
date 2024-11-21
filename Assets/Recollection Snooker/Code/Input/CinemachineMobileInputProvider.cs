using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.InputSystem;

namespace Dante.RecollectionSnooker
{
    public class CinemachineMobileInputProvider : CinemachineInputProvider
    {
        #region Parameters

        public bool enableCameraRig;

        #endregion

        #region PublicMethods

        public override float GetAxisValue(int axis)
        {
            if (enabled && enableCameraRig)
            {
                var action = ResolveForPlayer(axis, axis == 2 ? ZAxis : XYAxis);
                if (action != null)
                {
                    switch (axis)
                    {
                        case 0: return action.ReadValue<Vector3>().y;
                        case 1: return action.ReadValue<Vector3>().x;
                        case 2: return action.ReadValue<float>();
                    }
                }
            }
            return 0;
        }

        #endregion
    }
}

