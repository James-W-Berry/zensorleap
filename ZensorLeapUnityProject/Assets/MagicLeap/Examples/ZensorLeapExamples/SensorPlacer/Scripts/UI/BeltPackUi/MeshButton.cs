// ---------------------------------------------------------------------
//
// Copyright (c) 2018 Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Creator Agreement, located
// here: https://id.magicleap.com/creator-terms
//
// ---------------------------------------------------------------------

using UnityEngine;
using UnityEngine.Events;

namespace MagicKit
{
    ///<summary>
    /// MeshButton - This class represents a button. It handles highlighting the button
    /// when selected and invoking an OnClick event when selected
    ///</summary>
    public class MeshButton : MonoBehaviour
    {
        //----------- Public Events -----------

        public UnityEvent OnButtonClicked = new UnityEvent();
        public UnityEvent OnButtonSelected = new UnityEvent();
        public UnityEvent OnButtonDeselected = new UnityEvent();

        //----------- Public Members -----------

        public bool scaleOnSelection = true;

        //----------- Private Members -----------

        [SerializeField] private Vector3 _initialScale = new Vector3(1, 1, 1);
        [SerializeField] private bool _isSelected;
        [SerializeField] private bool _isUI;

        private Animator _animator;

        private const float ScaleFactor = 1.2f;
        private const float AnimationTimeout = 0.2f;
        private const string HoverName = "hover";
        private const string SelectName = "select";

        //----------- MonoBehaviour Methods -----------

        private void Start()
        {
            _animator = GetComponentInParent<Animator>();
        }

        private void OnEnable()
        {
            InputManager.Instance.OnTriggerUp += HandleOnTriggerPressed;
            _isSelected = false;
            if (scaleOnSelection)
            {
                transform.localScale = _initialScale;
            }
            if (_isUI)
            {
                Idle();
            }
        }

        private void OnDisable()
        {
            if (scaleOnSelection)
            {
                transform.localScale = _initialScale;
            }
            _isSelected = false;
            InputManager.Instance.OnTriggerUp -= HandleOnTriggerPressed;
            if (_isUI)
            {
                Idle();
            }
        }

        //----------- Private Methods -----------

        ///<summary>
        ///  Callback when this game object becomes selected by the cursor colliding with it
        ///</summary>
        private void OnTriggerEnter(Collider other)
        {
            if (_isUI == false && BeltPackUi.Instance.isDisplaying == true)
            {
                return;
            }
            _isSelected = true;
            if (_isUI)
            {
                BeltPackUi.Instance.HandleOnObjectSelected();
                Hover();
            }
            if (OnButtonSelected != null)
            {
                OnButtonSelected.Invoke();
            }
            if (!_isUI && scaleOnSelection)
            {
                transform.localScale = _initialScale * ScaleFactor;
            }
        }

        ///<summary>
        ///  Callback when this game object becomes deselected by the cursor no longer colliding with it
        ///</summary>
        private void OnTriggerExit(Collider other)
        {
            if (_isUI == false && BeltPackUi.Instance.isDisplaying == true)
            {
                return;
            }
            _isSelected = false;
            if (_isUI)
            {
                BeltPackUi.Instance.HandleOnObjectDeselected();
                Idle();
            }
            if (OnButtonDeselected != null)
            {
                OnButtonDeselected.Invoke();
            }
            if (!_isUI && scaleOnSelection)
            {
                transform.localScale = _initialScale;
            }
        }

        private void OnUIButtonSelect()
        {
            OnButtonClicked.Invoke();
        }

        private void Idle()
        {
            if (_animator != null)
            {
                _animator.SetBool(SelectName, false);
                _animator.SetBool(HoverName, false);
            }
        }

        private void Hover()
        {
            if (_animator != null)
            {
                _animator.SetBool(HoverName, true);
            }
        }

        private void Select()
        {
            if (_animator != null)
            {
                _animator.SetBool(SelectName, true);
            }
        }

        //----------- Event Handlers -----------

        ///<summary>
        ///  Callback when the trigger is pressed. If this object is selected then it invokes an event for the app to respond to
        ///</summary>
        private void HandleOnTriggerPressed()
        {
            if (_isUI == false && (BeltPackUi.Instance.isDisplaying == true || BeltPackUi.Instance.allowDeletion == false))
            {
                return;
            }
            if (_isSelected && OnButtonClicked != null)
            {
                if (_isUI)
                {
                    Select();
                    Invoke("OnUIButtonSelect", AnimationTimeout);
                }
                else
                {
                    OnButtonClicked.Invoke();
                }
            }
        }
    }
}