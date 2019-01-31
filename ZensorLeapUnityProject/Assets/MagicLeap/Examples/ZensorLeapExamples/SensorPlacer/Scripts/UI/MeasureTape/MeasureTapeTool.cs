// ---------------------------------------------------------------------
//
// Copyright (c) 2018 Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Creator Agreement, located
// here: https://id.magicleap.com/creator-terms
//
// ---------------------------------------------------------------------

using System;
using UnityEngine;

namespace MagicKit
{
    ///<summary>
    /// Handles user input for manipulating tape measure instances.
    ///</summary>
    public class MeasureTapeTool : MonoBehaviour
    {
        //----------- Public Events -----------

        public event Action OnMeasureTapeStarted;
        public event Action OnMeasureTapePointAdded;
        public event Action OnMeasureTapeEnded;
        public event Action OnMeasureTapeDeleted;
        public event Action OnMeasureTapeSelected;
        public event Action OnMeasureTapeDeselected;

        //----------- Public Members -----------

        [HideInInspector] public bool allowDeletion = true;
        [HideInInspector] public bool allowPlacement = true;

        //----------- Private Members -----------

        [SerializeField] private ControllerInput _controllerInput;
        [SerializeField] private MeasureTape _measureTapePrefab;
        private MeasureTape _measureTape;
        private const float MeasureTapeSurfaceOffset = .01f;

        // Are we currently updating a measure tape?
        private bool _isMeasuring = false;

        //----------- MonoBehaviour Methods -----------

        private void OnEnable()
        {
            _controllerInput.OnBumperUp += HandleOnBumperUp;
            _controllerInput.OnTriggerUp += HandleOnTriggerUp;
        }

        private void OnDisable()
        {
            _controllerInput.OnBumperUp -= HandleOnBumperUp;
            _controllerInput.OnTriggerUp -= HandleOnTriggerUp;
        }

        private void Update()
        {
            if (CursorController.Instance.SelectedGameObject != null && _isMeasuring)
            {
                Vector3 placePoint = DeterminePlacementPos(CursorController.Instance.HitPosition, CursorController.Instance.HitNormal);
                _measureTape.Draw(placePoint);
            }
        }

        //----------- Public Methods -----------

        public void HandleOnToolDeselected()
        {
            StopMeasuring();
        }

        //----------- Event Handlers -----------

        private void HandleOnBumperUp()
        {
            StopMeasuring();
        }

        private void HandleOnTriggerUp()
        {
            if(!allowPlacement)
            {
                return;
            }

            if (_measureTape == null)
            {
                // Begin a new measuring tape.
                StartMeasuring();
            }
            else if (_isMeasuring)
            {
                // If we are measuring, place a point.
                Vector3 placePoint = DeterminePlacementPos(CursorController.Instance.HitPosition, CursorController.Instance.HitNormal);
                PlacePoint(placePoint);
            }
        }

        private void HandleOnMeasureTapeSelected(MeasureTape measureTape)
        {
            if (_isMeasuring)
            {
                return;
            }

            _measureTape = measureTape;
            _measureTape.SetSelected(true);

            if (OnMeasureTapeSelected != null)
            {
                OnMeasureTapeSelected();
            }

            BeltPackUi.Instance.HideCurrentTool();
        }

        private void HandleOnMeasureTapeDeselected(MeasureTape measureTape)
        {
            if (_isMeasuring)
            {
                return;
            }

            if (_measureTape == measureTape)
            {
                _measureTape.SetSelected(false);
                _measureTape = null;
            }

            if (OnMeasureTapeDeselected != null)
            {
                OnMeasureTapeDeselected();
            }

            BeltPackUi.Instance.ShowCurrentTool();
        }

        private void HandleOnMeasureTapeClicked(MeasureTape measureTape)
        {
            if (_isMeasuring)
            {
                // Disallow picking up of measuring tapes if we are in the middle of editing one.
                return;
            }

            if (_measureTape == null || _measureTape != measureTape)
            {
                // Does nothing if the clicked measure tape does not match the currently selected one.
                return;
            }

            if(allowDeletion)
            {
                // Deletes the current measure tape.
                GameObject.Destroy(_measureTape.gameObject);
                _measureTape = null;

                if (OnMeasureTapeDeleted != null)
                {
                    OnMeasureTapeDeleted();
                }
            }

            // Notifies the beltpack UI of our being clicked.
            BeltPackUi.Instance.OnMeasuringTapeButtonClicked();
        }

        //----------- Private Methods -----------

        /// <summary>
        /// Starts a new measure tape.
        /// </summary>
        private void StartMeasuring()
        {
            // Creates a new measure tape.
            _measureTape = GameObject.Instantiate(_measureTapePrefab);
            _measureTape.StartDrawing();
            _measureTape.OnMeasureTapeSelected += HandleOnMeasureTapeSelected;
            _measureTape.OnMeasureTapeDeselected += HandleOnMeasureTapeDeselected;
            _measureTape.OnMeasureTapeClicked += HandleOnMeasureTapeClicked;
            _isMeasuring = true;

            SetActivatable(false);

            // Notifies listeners of event.
            if (OnMeasureTapeStarted != null)
            {
                OnMeasureTapeStarted();
            }
        }

        /// <summary>
        /// Terminates the current measure tape.
        /// </summary>
        private void StopMeasuring()
        {
            if (_measureTape != null)
            {
                _measureTape.StopDrawing();

                if(_measureTape._totalDistance == 0)
                {
                    Destroy(_measureTape.gameObject);
                }                
            }

            _isMeasuring = false;
            _measureTape = null;

            SetActivatable(true);

            // Notifies listeners of event.
            if (OnMeasureTapeEnded != null)
            {
                OnMeasureTapeEnded();
            }
        }

        /// <summary>
        /// Adds a new point onto the current measure tape.
        /// </summary>
        private void PlacePoint(Vector3 point)
        {
            _measureTape.AddNewPoint(point);

            // Notifies listeners of event.
            if (OnMeasureTapePointAdded != null)
            {
                OnMeasureTapePointAdded();
            }
        }

        /// <summary>
        /// Calculates a placement position above the mesh.
        /// </summary>
        private Vector3 DeterminePlacementPos(Vector3 hitPoint, Vector3 hitNormal)
        {
            return hitPoint + hitNormal * MeasureTapeSurfaceOffset;
        }

        /// <summary>
        /// Coomunicates whether we are able to be deactivated.
        /// </summary>
        private void SetActivatable(bool activatable)
        {
            BeltpackUiElement element = GetComponent<BeltpackUiElement>();
            if (element != null)
            {
                element.canDeactivate = activatable;
            }
        }
    }
}
