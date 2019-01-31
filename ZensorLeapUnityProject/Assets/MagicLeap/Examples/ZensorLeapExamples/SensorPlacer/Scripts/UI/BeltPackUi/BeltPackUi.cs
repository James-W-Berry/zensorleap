// ---------------------------------------------------------------------
//
// Copyright (c) 2018 Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Creator Agreement, located
// here: https://id.magicleap.com/creator-terms
//
// ---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using UnityEngine;
using MagicLeap.Utilities;

namespace MagicKit
{
    ///<summary>
    /// This class handles the UI. It will listen for button presses and 
    /// activate the tools, hide/show the UI, restore last active tools
    ///</summary>
    public class BeltPackUi : Singleton<BeltPackUi>
    {

        //----------- Public Events -----------

        public event Action OnEnvCubeSelected;
        public event Action OnTapeMeasureSelected;
        public event Action OnAngleToolSelected;

        //----------- Public Members -----------

        [HideInInspector] public bool isDisplaying;
        [HideInInspector] public bool canUseBumper = true;
        [HideInInspector] public bool showTools = true;
        [HideInInspector] public bool allowDeletion = true;

        //----------- Private Members -----------

        [SerializeField] private BeltpackUiAudioBehavior _audioHandler;
        [SerializeField] private ToolAudioBehavior _toolAudioHandler;
        [SerializeField] private List<GameObject> _buttons;
        [SerializeField] private GameObject _angleTool;
        [SerializeField] private GameObject _environmentCubeTool;
        [SerializeField] private GameObject _measuringTapeTool;
        [SerializeField] private GameObject _background;

        private GameObject _activeTool;
        private float _distance = 1.0f;
        private Camera _camera;

        //----------- MonoBehaviour Methods -----------

        private void Start()
        {
            // Register event listeners with measure tape tool.
            MeasureTapeTool measureTapeTool = _measuringTapeTool.GetComponent<MeasureTapeTool>();
            measureTapeTool.OnMeasureTapeSelected += HandleOnObjectSelected;
            measureTapeTool.OnMeasureTapeDeselected += HandleOnObjectDeselected;
            measureTapeTool.OnMeasureTapeStarted += HandleOnObjectPlaced;
            measureTapeTool.OnMeasureTapePointAdded += HandleOnObjectPlaced;

            _camera = Camera.main;
        }

        private void OnDestroy()
        {
            // Deregister event listeners with measure tape tool.
            MeasureTapeTool measureTapeTool = _measuringTapeTool.GetComponent<MeasureTapeTool>();
            measureTapeTool.OnMeasureTapeSelected -= HandleOnObjectSelected;
            measureTapeTool.OnMeasureTapeDeselected -= HandleOnObjectDeselected;
            measureTapeTool.OnMeasureTapeStarted -= HandleOnObjectPlaced;
            measureTapeTool.OnMeasureTapePointAdded -= HandleOnObjectPlaced;
        }

        private void OnEnable()
        {
            InputManager.Instance.OnBumperUp += HandleOnBumperPressed;
        }

        private void OnDisable()
        {
            InputManager.Instance.OnBumperUp -= HandleOnBumperPressed;
        }

        //----------- Event Handlers -----------

        ///<summary>
        ///  This is a callback for when the bumper is clicked and will hide or show the UI
        ///</summary>
        private void HandleOnBumperPressed()
        {
            if (!canUseBumper)
            {
                return;
            }

            if (!isDisplaying)
            {
                // Only show the UI if the current tool allows deactivation.
                BeltpackUiElement element = null;
                if (_activeTool != null)
                {
                    element = _activeTool.GetComponent<BeltpackUiElement>();
                }
                bool canShow = (element == null) || element.canDeactivate;
                if (canShow)
                {
                    Show();
                }
            }
            else
            {
                Hide();
            }
        }

        ///<summary>
        ///  Callback for when any object is selected.
        ///</summary>
        public void HandleOnObjectSelected()
        {
            _toolAudioHandler.Selected();
        }

        ///<summary>
        ///  Callback for when any object is deselected.
        ///</summary>
        public void HandleOnObjectDeselected()
        {
            _toolAudioHandler.Deselected();
        }

        ///<summary>
        ///  Callback for when any object is placed.
        ///</summary>
        public void HandleOnObjectPlaced()
        {
            _toolAudioHandler.Placed();
        }

        ///<summary>
        ///  Callback for when any object is deleted.
        ///</summary>
        public void HandleOnObjectDeleted()
        {
            _toolAudioHandler.Deleted();
        }

        ///<summary>
        ///  Callback for when the button for the angle tool is clicked. This will hide the UI 
        ///  and enable the angle tool
        ///</summary>
        public void OnAngleButtonClicked()
        {
            if (OnAngleToolSelected != null)
            {
                OnAngleToolSelected.Invoke();
            }

            ChangeTool(_angleTool);
            Hide();
        }

        ///<summary>
        ///  Callback for when the button for the measuring tape tool is clicked. This will hide the UI 
        ///  and enable the measuring tape tool
        ///</summary>
        public void OnMeasuringTapeButtonClicked()
        {
            ChangeTool(_measuringTapeTool);
            Hide();

            if (OnTapeMeasureSelected != null)
            {
                OnTapeMeasureSelected();
            }
        }

        ///<summary>
        ///  Callback for when the button for the environment cube tool is clicked. This will hide the UI 
        ///  and enable the environment cube tool
        ///</summary>
        public void OnEnvironmentalCubeButtonClicked()
        {
            if (OnEnvCubeSelected != null)
            {
                OnEnvCubeSelected.Invoke();
            }

            ChangeTool(_environmentCubeTool);
            Hide();
        }
        
        public void HideUIButtons()
        {
            for (int i = 0; i < _buttons.Count; ++i)
            {
                _buttons[i].SetActive(false);
            }
        }

        public void HideCurrentTool()
        {
            _activeTool.SetActive(false);
        }

        public void ShowCurrentTool()
        {
            _activeTool.SetActive(true);
        }

        //----------- Private Methods -----------

        ///<summary>
        ///  Helper function to disable the current active tool and make this tool active
        ///</summary>
        private void ChangeTool(GameObject newTool)
        {
            if (_activeTool != null)
            {
                HideCurrentTool();
                if(_activeTool == _measuringTapeTool)
                {
                    MeasureTapeTool measureTapeTool = _measuringTapeTool.GetComponent<MeasureTapeTool>();
                    measureTapeTool.HandleOnToolDeselected();

                }
                _activeTool = null;
            }
            _activeTool = newTool;
        }
        
        private void FindPosition()
        { 
            float distance = _distance;

            RaycastHit hit;
            // Make sure there is nothing obstructing where we want to place the UI. If so, move it on top
            bool result = Physics.Raycast(_camera.transform.position, _camera.transform.forward, out hit);
            if (result)
            {
                distance = Vector3.Distance(_camera.transform.position, hit.point);
            }
            distance = Mathf.Min(distance, _distance);
            Vector3 forward = _camera.transform.forward;
            transform.position = _camera.transform.position + distance * forward;
            transform.rotation = Quaternion.LookRotation(forward, Vector3.up);
        }

        ///<summary>
        ///   This function will hide the UI when either a tool is selected or bumper is pressed while the UI is showing
        ///   It will enable inputs to the previous active tool if there was one.
        ///</summary>
        private void Hide()
        {
            for (int i = 0; i < _buttons.Count; ++i)
            {
                _buttons[i].SetActive(false);
            }
            _background.SetActive(false);
            if (_activeTool != null)
            {
                ShowCurrentTool();
            }
            isDisplaying = false;

            _audioHandler.MenuClose();
        }

        ///<summary>
        ///   This function will show the UI when bumper is pressed while the UI is hidden. It will block inputs
        ///   to the current tool if there is one.
        ///</summary>
        private void Show()
        {
            FindPosition();
            // In tutorial we don't want to immediately show all the tools
            if (showTools)
            {
                for (int i = 0; i < _buttons.Count; ++i)
                {
                    _buttons[i].SetActive(true);
                }
            }
            _background.SetActive(true);
            if (_activeTool != null)
            {
                HideCurrentTool();
            }
            isDisplaying = true;

            _audioHandler.MenuOpen();
        }
    }
}