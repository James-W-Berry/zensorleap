// ---------------------------------------------------------------------
//
// Copyright (c) 2018 Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Creator Agreement, located
// here: https://id.magicleap.com/creator-terms
//
// ---------------------------------------------------------------------

using System;
using UnityEngine;
using MagicLeap.Utilities;

namespace MagicKit
{
    /// <summary>
    /// This is a Singleton Wrapper for the ControllerInput script. 
    /// Made so that run-time instantiated objects can easily listen to controller events without hookup to the ControllerInput object.
    /// </summary>
    public class InputManager : Singleton<InputManager>
    {
        // ------ Public  Events ------
        public event Action OnTriggerDown;
        public event Action OnTriggerUp;
        public event Action OnBumperDown;
        public event Action OnBumperUp;
        public event Action OnTouchDown;
        public event Action OnTouchUp;

        //----------- Private Members -----------

        [SerializeField] private ControllerInput _controller;

        // ------ MonoBehaviour Methods ------

        private void OnEnable()
        {
            _controller.OnBumperDown += HandleOnBumperDown;
            _controller.OnBumperUp += HandleOnBumperUp;
            _controller.OnTouchDown += HandleOnTouchDown;
            _controller.OnTouchUp += HandleOnTouchUp;
            _controller.OnTriggerDown += HandleOnTriggerDown;
            _controller.OnTriggerUp += HandleOnTriggerUp;
        }

        private void OnDisable()
        {
            _controller.OnBumperDown -= HandleOnBumperDown;
            _controller.OnBumperUp -= HandleOnBumperUp;
            _controller.OnTouchDown -= HandleOnTouchDown;
            _controller.OnTouchUp -= HandleOnTouchUp;
            _controller.OnTriggerDown -= HandleOnTriggerDown;
            _controller.OnTriggerUp -= HandleOnTriggerUp;
        }

        //----------- Event Handlers -----------

        private void HandleOnBumperDown()
        {
            if (OnBumperDown != null)
            {
                OnBumperDown();
            }
        }

        private void HandleOnBumperUp()
        {
            if (OnBumperUp != null)
            {
                OnBumperUp();
            }
        }

        private void HandleOnTouchDown()
        {
            if (OnTouchDown != null)
            {
                OnTouchDown();
            }
        }

        private void HandleOnTouchUp()
        {
            if (OnTouchUp != null)
            {
                OnTouchUp();
            }
        }

        private void HandleOnTriggerDown()
        {
            if( OnTriggerDown != null)
            {
                OnTriggerDown();
            }
        }

        private void HandleOnTriggerUp()
        {
            if (OnTriggerUp != null)
            {
                OnTriggerUp();
            }
        }
    }
}