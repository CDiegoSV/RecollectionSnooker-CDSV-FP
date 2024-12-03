using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RS_Cinemachine_TargetGroup : CinemachineTargetGroup
{
    #region Public Methods

    public void ResetTargetGroup()
    {
        m_PositionMode = PositionMode.GroupCenter;
        m_RotationMode = RotationMode.Manual;
        m_UpdateMethod = UpdateMethod.LateUpdate;
        m_Targets = Array.Empty<Target>();
    }

    #endregion
}
