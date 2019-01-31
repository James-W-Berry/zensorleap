// ---------------------------------------------------------------------
//
// Copyright (c) 2018 Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Creator Agreement, located
// here: https://id.magicleap.com/creator-terms
//
// ---------------------------------------------------------------------

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Events;

namespace MagicKit
{
    ///<summary>
    /// Class that handles angle tool instances interactions. If continuously raycasts
    /// to check collision with angle tools placed in the world and handles all interaction logic and events
    ///</summary>
    public class AngleButton : MonoBehaviour
    {

        //----------- Public Events -----------

        public UnityEvent OnButtonClicked = new UnityEvent();
        public UnityEvent OnButtonSelected = new UnityEvent();
        public UnityEvent OnButtonDeselected = new UnityEvent();

        //--------- Private Variables ----------

        private bool _isSelected;
        private Vector3 _initialScale;
        private GraphicRaycaster _caster;
        private Camera _camera;
        private const float ScaleFactor = 1.2f;

        //----------- MonoBehaviour Methods -----------

        private void Start()
        {
            _initialScale = transform.localScale;
            _caster = GetComponent<GraphicRaycaster>();
            _camera = Camera.main;
        }

        private void OnDisable()
        {
            InputManager.Instance.OnTriggerUp -= HandleOnTriggerPressed;
        }

        private void OnEnable()
        {
            InputManager.Instance.OnTriggerUp += HandleOnTriggerPressed;
        }

        private void Update()
        {
            if (BeltPackUi.Instance.isDisplaying == true)
            {
                return;
            }

            List<RaycastResult> results = new List<RaycastResult>();
            PointerEventData cursor = new PointerEventData(EventSystem.current);
            // 0.5 is used for the center of the viewport.
            cursor.position = _camera.ViewportToScreenPoint(new Vector2(0.5f, 0.5f));

            //Raycast using the Graphics Raycaster and mouse click position
            _caster.Raycast(cursor, results);

            // If we hit this then it's consider selected
            if (results.Count > 0)
            {
                // if it can be deleted, scale it...
                if (BeltPackUi.Instance.allowDeletion)
                {
                    transform.localScale = _initialScale * ScaleFactor;
                }
                // Let the outside world respond to the selected event 
                if (OnButtonSelected != null && _isSelected == false)
                {
                    OnButtonSelected.Invoke();
                    _isSelected = true;
                }
            }
            else
            {
                // if it can be deleted, scale it to normal
                if (BeltPackUi.Instance.allowDeletion)
                {
                    transform.localScale = _initialScale;
                }
                // Let the outside world respond to the deselected event.
                if (OnButtonDeselected != null && _isSelected == true)
                {
                    OnButtonDeselected.Invoke();
                    _isSelected = false;
                }
            }
        }

        //----------- Event Handlers -----------

        ///<summary>
        ///  Called when trigger is pressed. If this object is selected then it invokes an event for the app to respond to
        ///</summary>
        private void HandleOnTriggerPressed()
        {
            if (BeltPackUi.Instance.isDisplaying == true || BeltPackUi.Instance.allowDeletion == false)
            {
                return;
            }
            if (_isSelected && OnButtonClicked != null)
            {
                OnButtonClicked.Invoke();
            }
        }
    }
}