// ---------------------------------------------------------------------
//
// Copyright (c) 2018 Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Creator Agreement, located
// here: https://id.magicleap.com/creator-terms
//
// ---------------------------------------------------------------------
    
using UnityEngine;
using UnityEngine.UI;
using System;

namespace MagicKit
{
    ///<summary>
    /// This class handles the angle tool logic. The angle will face the user and be 
    /// attached to the users headpose at the intersection of the cursor and the mesh. 
    /// A radial scroll gesture will rotate the angle before it is placed. Trigger will place the angle.
    ///</summary>
    public class AngleTool : MonoBehaviour
    {

        //----------- Public Events ------------

        public event Action OnPlaced;
        public event Action OnRotated;
        public event Action OnDeleted;

        //----------- Public Members -----------

        [HideInInspector] public bool triggerEnabled = true;

        //----------- Private Members -----------

        [SerializeField] private TickingAudioBehavior _angleAudioHandler;
        [SerializeField] private ControllerInput _controllerInput;
        private int _angle;
        private bool _touchActive;
        private int _touchCount;
        private float[] _times;
        private int _timesIdx;
        private Vector2 _previousPos;
        [SerializeField] private GameObject _linePrefab;
        private const float TouchPadDirectionThreshold = 0.8f;
        private const int TouchPadMagnitude = 50;
        private const int MaxAngle = 90;
        private const int TouchCounterThreshold = 15;
        private const float DoubleTapThreshold = 0.45f;
        private Camera _camera;
        private MeshAnchor _meshAnchor;
        private Text _text;

        //----------- MonoBehaviour Methods -----------

        private void Start()
        {
            _camera = Camera.main;
            _meshAnchor = GetComponent<MeshAnchor>();
            _text = transform.gameObject.GetComponentInChildren<Text>();
            _times = new float[2];
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
            if (CursorController.Instance.SelectedGameObject != null)
            {
                transform.position = CursorController.Instance.HitPosition;
            }

            Vector3 lookDir = -_camera.transform.forward;
            transform.rotation = Quaternion.LookRotation(lookDir, Vector3.up);

            bool newTouchActive = _controllerInput.TouchActive;
            if (newTouchActive && !_touchActive)
            {
                _previousPos = _controllerInput.TouchPosition;
                _touchCount = 0;
            }
            else if (newTouchActive && _touchActive)
            {
                HandleTouchpadMoved();
                _touchCount++;
            }
            else if (!newTouchActive && _touchActive)
            {
                HandleTouchPadReleased();
            }
            _touchActive = newTouchActive;

            transform.RotateAround(transform.position, lookDir, _angle);
            _text.text = Mathf.Abs(_angle).ToString();
            _meshAnchor.Adjust();

            _angleAudioHandler.MeasuringTick(_angle); // Plays tick sound on angle change
        }

        //----------- Event Handlers -----------  

        ///<summary>
        ///  Callback when the trigger is pressed, this will place the angle in the world at it's current location
        ///</summary>
        private void HandleOnTriggerPressed()
        {
            if (!triggerEnabled) 
            {
                return;
            }
            GameObject angle = Instantiate(_linePrefab);
            AngleButton buttonComponent = angle.GetComponent<AngleButton>();
            BeltPackUi.Instance.HandleOnObjectPlaced();
            buttonComponent.OnButtonClicked.AddListener(() =>
            {
                if (!BeltPackUi.Instance.allowDeletion)
                {
                    return;
                }
                Destroy(angle);
                if (OnDeleted != null)
                {
                    OnDeleted.Invoke();
                }
            });
            buttonComponent.OnButtonClicked.AddListener(() =>
            {
                if (!BeltPackUi.Instance.allowDeletion)
                {
                    return;
                }
                BeltPackUi.Instance.OnAngleButtonClicked();
            });
            buttonComponent.OnButtonSelected.AddListener(() =>
            {
                BeltPackUi.Instance.HideCurrentTool();
                BeltPackUi.Instance.HandleOnObjectSelected();
            });
            buttonComponent.OnButtonDeselected.AddListener(() =>
            {
                BeltPackUi.Instance.ShowCurrentTool();
                BeltPackUi.Instance.HandleOnObjectDeselected();
            });
            angle.transform.position = transform.position;
            angle.transform.rotation = transform.rotation;
            angle.transform.GetComponentInChildren<Text>().text = Mathf.Abs(_angle).ToString();
            if (OnPlaced != null)
            {
                OnPlaced.Invoke();
            }
        }


        //----------- Private Functions -----------

        ///<summary>
        ///  This function handles the logic when the user moves their finger along the touchpad. 
        ///  If it's moved vertically then it will rotate the angle
        ///</summary>
        private void HandleTouchpadMoved()
        {
            Vector2 currPos = _controllerInput.TouchPosition;
            Vector2 dir = currPos - _previousPos;
            Vector2 dirNorm = dir.normalized;
            if (Mathf.Abs(dirNorm.y) > TouchPadDirectionThreshold)
            {
                if (dir.y > 0)
                {
                    _angle -= Mathf.Max(1, (int)(Mathf.Round(dir.magnitude * TouchPadMagnitude)));
                    _angle = Mathf.Max(-MaxAngle, _angle);
                }
                else
                {
                    _angle += Mathf.Max(1, (int)(Mathf.Round(dir.magnitude * TouchPadMagnitude)));
                    _angle = Mathf.Min(MaxAngle, _angle);
                }
                if (OnRotated != null)
                {
                    OnRotated.Invoke();
                }
            }
            _previousPos = currPos;
        }
        
        private void HandleTouchPadReleased()
        {
            if (_touchCount < TouchCounterThreshold)
            {
                _times[_timesIdx] = Time.realtimeSinceStartup;
                float elapsedTime = 1;
                if (_timesIdx == 0)
                {
                    elapsedTime = _times[0] - _times[1];
                    _timesIdx = 1;
                }
                else
                {
                    elapsedTime = _times[1] - _times[0];
                    _timesIdx = 0;
                }
                // Checking for double tap
                if (elapsedTime < DoubleTapThreshold)
                {

                    _angle = 0;
                }
            }
        }
    }
}