// ---------------------------------------------------------------------
//
// Copyright (c) 2018 Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Creator Agreement, located
// here: https://id.magicleap.com/creator-terms
//
// ---------------------------------------------------------------------

using MagicLeap.Utilities;
using UnityEngine;

namespace MagicKit
{
    /// <summary>
    /// Handles audio for the Beltpack UI menu.
    /// </summary>
    public class BeltpackUiAudioBehavior : AudioBehavior
    {
        //----------- Private Members -----------

        private const string SelectedKey = "selected";
        private const string DeselectedKey = "deselected";
        private const string MenuOpenKey = "menu_appears";
        private const string MenuCloseKey = "menu_disappears";

        //----------- Public Methods -----------

        /// <summary>
        /// Plays the sfx for a headpose-targetable object being selected (hovered over).
        /// </summary>
        public void Selected()
        {
            PlaySound(SelectedKey);
        }

        /// <summary>
        /// Plays the sfx for a headpose-targetable object being deselected.
        /// </summary>
        public void Deselected()
        {
            PlaySound(DeselectedKey);
        }

        /// <summary>
        /// Plays the sfx for the menu opening on separate channel so it is not interrupted.
        /// </summary>
        public void MenuOpen()
        {
            PlaySound(MenuOpenKey);
        }

        /// <summary>
        /// Plays the sfx for the menu closing on separate channel so it is not interrupted.
        /// </summary>
        public void MenuClose()
        {
            PlaySound(MenuCloseKey);
        }
    }
}
