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
    /// Handles all audio for measuring tape and angle tools.
    /// </summary>
    public class TickingAudioBehavior : AudioBehavior
    {
        //----------- Private Members -----------

        [SerializeField] private float _measurementRequiredForTickSound = 1f;
        [SerializeField] private GameObject _audioObject;

        private const string ForwardMeasuringTickKey = "fwd_measuring_tick";
        private const string ReverseMeasuringTickKey = "rev_measuring_tick";
        private float _previousMeasurement = 0;


        //----------- Public Methods -----------

        /// <summary>
        /// Plays the sfx for measurements being taken by Tape/Angle. (Tick Sound)
        /// </summary>
        public void MeasuringTick(float measurement)
        {
            if (((measurement - _previousMeasurement) > _measurementRequiredForTickSound) && (measurement != _previousMeasurement)) //tick once at every multiple of _tickSoundAngle going forward
            {
                _previousMeasurement = measurement;
                ForwardMeasuringTick();
            }
            else if (((_previousMeasurement - measurement) > _measurementRequiredForTickSound) && (measurement != _previousMeasurement)) //tick once at every multiple of _tickSoundAngle going reverse
            {
                _previousMeasurement = measurement;
                ReverseMeasuringTick();
            }
        }

        //----------- Private Methods -----------

        /// <summary>
        /// Plays the sfx for an increasing measurement being taken.
        /// </summary>
        private void ForwardMeasuringTick()
        {
            PlaySoundAt(ForwardMeasuringTickKey, _audioObject);
        }

        /// <summary>
        /// Plays the sfx for a decreasing measurement being taken.
        /// </summary>
        private void ReverseMeasuringTick()
        {
            PlaySoundAt(ReverseMeasuringTickKey, _audioObject);
        }
    }
}
