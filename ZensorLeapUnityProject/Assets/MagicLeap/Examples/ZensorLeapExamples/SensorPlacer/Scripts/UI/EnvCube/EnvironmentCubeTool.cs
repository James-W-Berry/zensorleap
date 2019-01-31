// ---------------------------------------------------------------------
//
// Copyright (c) 2018 Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Creator Agreement, located
// here: https://id.magicleap.com/creator-terms
//
// ---------------------------------------------------------------------

using UnityEngine;
using System;

namespace MagicKit
{
    ///<summary>
    /// EnvironmentCubeTool - This class lets the user manipulate a cube around the world with their headpose.
    /// When trigger is pressed it will place that cube in the world. The cube rotates with the user on the x and z
    /// directions and moves up and down perfectly vertically on the y plane like a it's moving on a pole
    ///</summary>
    public class EnvironmentCubeTool : MonoBehaviour
    {

        //----------- Public Events ------------

        public event Action OnPlaced;
        public event Action OnMove;
        public event Action OnRotate;
        public event Action OnDeleted;

        //----------- Public Members -----------

        [HideInInspector] public bool triggerEnabled = true;

        //----------- Private Members -----------

        [SerializeField] private TickingAudioBehavior _cubeDistanceTickAudioHandler;
        [SerializeField] private TickingAudioBehavior _cubeAngleTickAudioHandler;
        [SerializeField] private ControllerInput _controllerInput;
        [SerializeField] private GameObject _envCubePrefab;
        private float _distance = 1.0f;
        private Vector2 _previousPos;
        private bool _touchActive;
        private float _angle;
        private Camera _camera;
        private const float DirectionThreshold = 0.8f;
        private const float OffsetThreshold = 0.08f;

        //----------- MonoBehaviour Methods -----------

        private void Start()
        {
            _camera = Camera.main;
        }

        private void OnEnable()
        {
            _controllerInput.OnTriggerUp += HandleOnTriggerPressed;
        }

        private void OnDisable()
        {
            _controllerInput.OnTriggerUp -= HandleOnTriggerPressed;
        }
        
        private void Update()
        {
            Vector3 forward = new Vector3(_camera.transform.forward.x, 0.0f, _camera.transform.forward.z);
            forward.Normalize();
            Vector3 cameraForward = _camera.transform.forward;
            cameraForward.Normalize();

            // Calculate the distance from the user. The max distance is either the far clip plane or the mesh
            // The min distance is the near clip plane
            float maxDistance = _camera.farClipPlane;

            RaycastHit hit;
            bool result = Physics.Raycast(_camera.transform.position, _camera.transform.forward, out hit);
            if (result)
            {
                maxDistance = Vector3.Distance(hit.point, _camera.transform.position);
                maxDistance -= OffsetThreshold;
            }

            // Get this distance onto the plane located in front of the user.
            float angle = Vector3.Angle(forward, cameraForward);
            float rad = angle * Mathf.Deg2Rad;
            float dist = _distance / Mathf.Cos(rad);

            dist = Mathf.Min(dist, maxDistance);
            dist = Mathf.Max(dist, _camera.nearClipPlane + OffsetThreshold);

            // Set the position
            transform.position = _camera.transform.position + dist * _camera.transform.forward;

            // Handle touchpad manipulation
            if(_controllerInput.TouchActive)
            {
                if (!_touchActive)
                {
                    _previousPos = _controllerInput.TouchPosition;
                }
                else
                {
                    HandleTouchpadMoved();
                }
            }

            _touchActive = _controllerInput.TouchActive;
        }

        //----------- Event Handlers -----------

        ///<summary>
        ///  Callback when the trigger is pressed. This will instatiate the environment cube with the same
        ///  position and rotation as the current cube the user is manipulating
        ///</summary>
        private void HandleOnTriggerPressed()
        {
            if (!triggerEnabled) 
            {
                return;
            } 
            GameObject cube = Instantiate(_envCubePrefab);
            MeshButton buttonComponent = cube.GetComponent<MeshButton>();
            EnvironmentCube cubeComponent = cube.GetComponent<EnvironmentCube>();
            BeltPackUi.Instance.HandleOnObjectPlaced();
            buttonComponent.OnButtonClicked.AddListener(() => {
                cubeComponent.OnPickUp();
                if (OnDeleted != null)
                {
                    OnDeleted();
                }
                BeltPackUi.Instance.OnEnvironmentalCubeButtonClicked();
            });
            buttonComponent.OnButtonSelected.AddListener(() => {
                cubeComponent.OnSelect();
                BeltPackUi.Instance.HideCurrentTool();
                BeltPackUi.Instance.HandleOnObjectSelected();
            });
            buttonComponent.OnButtonDeselected.AddListener(() => {
                cubeComponent.OnDeselect();
                BeltPackUi.Instance.ShowCurrentTool();
                BeltPackUi.Instance.HandleOnObjectDeselected();
            });
            cube.transform.position = transform.position;
            cube.transform.rotation = transform.rotation;
            // Place the cube, this will draw the rays and circles
            cubeComponent.Place();
            if (OnPlaced != null)
            {
                OnPlaced();
            }
        }

        //----------- Private Functions -----------

        ///<summary>
        ///  This function handles the logic when the user moves their finger along the touchpad. 
        ///  If it's moved vertically then it will move the cube, if it is moved horizontally it will
        ///  rotate the cube.
        ///</summary>
        private void HandleTouchpadMoved()
        {
            Vector2 currPos =_controllerInput.TouchPosition;
            Vector2 dir = currPos - _previousPos;
            Vector2 dirNorm = dir.normalized;
            if (Mathf.Abs(dirNorm.y) > DirectionThreshold)
            {
                if (dir.y > 0)
                {
                    _distance += dir.magnitude;
                    _cubeDistanceTickAudioHandler.MeasuringTick(_distance);
                    if (OnMove != null)
                    {
                        OnMove.Invoke();
                    }
                }
                else
                {
                    _distance -= dir.magnitude;
                    _cubeDistanceTickAudioHandler.MeasuringTick(_distance);
                    if (OnMove != null)
                    {
                        OnMove.Invoke();
                    }
                }
            }
            else if (Mathf.Abs(dirNorm.x) > DirectionThreshold)
            {
                if (dir.x > 0)
                {
                    _angle = 100.0f * dir.magnitude;
                    transform.Rotate(transform.up, _angle, Space.World);
                    _cubeAngleTickAudioHandler.MeasuringTick(_angle);
                    if (OnRotate != null)
                    {
                        OnRotate.Invoke();
                    }
                }
                else
                {
                    _angle = -100.0f * dir.magnitude;
                    transform.Rotate(transform.up, _angle, Space.World);
                    _cubeAngleTickAudioHandler.MeasuringTick(_angle);
                    if (OnRotate != null)
                    {
                        OnRotate.Invoke();
                    }
                }
            }
            _previousPos = currPos;
        }
    }
}