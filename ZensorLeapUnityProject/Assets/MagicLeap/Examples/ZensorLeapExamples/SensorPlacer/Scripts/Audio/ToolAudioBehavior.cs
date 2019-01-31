// ---------------------------------------------------------------------
//
// Copyright (c) 2018 Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Creator Agreement, located
// here: https://id.magicleap.com/creator-terms
//
// ---------------------------------------------------------------------

using MagicLeap.Utilities;

namespace MagicKit
{
    /// <summary>
    /// Handles all audio for measurement tools.
    /// </summary>
    public class ToolAudioBehavior : AudioBehavior
    {
        //----------- Private Members -----------

        private const string PlacedKey = "placed";
        private const string DeletedKey = "deleted";
        private const string SelectedKey = "selected";
        private const string DeselectedKey = "deselected";

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
        /// Plays the sfx for an object being deleted.
        /// </summary>
        public void Deleted()
        {
            PlaySound(DeletedKey);
        }

        /// <summary>
        /// Plays the sfx for an object being placed.
        /// </summary>
        public void Placed()
        {
            PlaySound(PlacedKey);
        }
    }
}
