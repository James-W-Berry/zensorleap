// ---------------------------------------------------------------------
//
// Copyright (c) 2018 Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Creator Agreement, located
// here: https://id.magicleap.com/creator-terms
//
// ---------------------------------------------------------------------

using UnityEngine;

namespace MagicKit
{
    /// <summary>
    /// This class contains common implementation for integrating gameobjects with a beltpack UI.
    /// </summary>
    public class BeltpackUiElement : MonoBehaviour
    {
        // Allows gameobjects to specify whether they are in a proper state for the UI to deactivate them.
        [HideInInspector] public bool canDeactivate = true;
    }
}
