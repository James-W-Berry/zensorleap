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

namespace MagicKit
{
    ///<summary>
    /// Represents a placed instance of a measure tape.
    ///</summary>
    public class MeasureTape : MonoBehaviour
    {
        //----------- Public Events -----------

        public event Action<MeasureTape> OnMeasureTapeSelected;
        public event Action<MeasureTape> OnMeasureTapeDeselected;
        public event Action<MeasureTape> OnMeasureTapeClicked;

        //----------- Private Members -----------

        [SerializeField] private MeshButton _meshButtonBoxPrefab;
        [SerializeField] private MLLine _line;
        [SerializeField] private string _testKey = "Text";

        private const float LineWidth = .005f;
        private const float LineWidthSelected = .02f;
        private const float LineSelectionColliderWidth = .05f;

        private TickingAudioBehavior _tapeAudioHandler;
        private GameObject _measureTapeTool;
        private bool _isSelected;
        private bool _isDrawing;
        private LineRenderer _currentLine;
        private Vector3 _mostRecentPoint;
        private List<TextMesh> _textLabels;
        private TextMesh _currentText;

        //----------- Public Members -----------

        public float _totalDistance;
        public float _tapeDistanceTraveled;

        //----------- MonoBehaviour Methods -----------

        private void Start()
        {
            _totalDistance = 0;
            _textLabels = new List<TextMesh>();
            _mostRecentPoint = Vector3.zero;

            _currentText = ObjectPooler.Instance.GetObject(_testKey).GetComponent<TextMesh>();
            _currentText.gameObject.SetActive(true);
            _textLabels.Add(_currentText);

            _measureTapeTool = GameObject.Find("MeasureTapeTool");
            _tapeAudioHandler = _measureTapeTool.GetComponent<TickingAudioBehavior>();
        }

        private void OnDestroy()
        {
            Clear();
        }

        //----------- Public Methods -----------

        /// <summary>
        /// Controls renering of the selected state for the measuring line.
        /// </summary>
        public void SetSelected(bool selected)
        {
            if (selected)
            {
                _currentLine.startWidth = _currentLine.endWidth = LineWidthSelected;
                _isSelected = true;
            }
            else
            {
                _currentLine.startWidth = _currentLine.endWidth = LineWidth;
                _isSelected = false;
            }
        }

        /// <summary>
        /// Start drawing a new line.
        /// </summary>
        public void StartDrawing()
        {
            //create a new linerenderer
            _currentLine = _line.Clone().GetLineRenderer();
            _currentLine.positionCount = 0;

            //if we've already drawn some lines, then we'll have a currentText object already.
            if (_currentText != null)
            {
                _currentText.gameObject.SetActive(true);
            }
            _mostRecentPoint = Vector3.zero;
            _isDrawing = true;
        }

        /// <summary>
        /// Stop drawing the current line.
        /// </summary>b
        public void StopDrawing()
        {
            _isDrawing = false;
            //move the last point (which we were using to draw) to the same position as the point before it. (last one placed by the user)
            _currentLine.SetPosition(_currentLine.positionCount - 1, _currentLine.GetPosition(_currentLine.positionCount - 2));
            _currentText.gameObject.SetActive(false);
        }

        /// <summary>
        /// Clears all lines and text.
        /// </summary>
        public void Clear()
        {
            // Clears out all the old text labels.
            foreach (TextMesh label in _textLabels)
            {
                if (label != null)
                {
                    label.text = "";
                    label.gameObject.SetActive(false);
                }

            }
            _textLabels.Clear();

            if (_currentLine != null)
            {
                _currentLine.SetPositions(new Vector3[] { });
                _currentLine.gameObject.SetActive(false);
                _currentText.text = "";
            }
        }

        /// <summary>
        /// Add a new point onto the existing line.
        /// </summary>
        public void AddNewPoint(Vector3 point)
        {
            _currentLine.positionCount++;
            _currentLine.SetPosition(_currentLine.positionCount - 1, point);

            //grab a text object from the object pooler
            _currentText = ObjectPooler.Instance.GetObject(_testKey).GetComponent<TextMesh>();
            _currentText.gameObject.SetActive(true);
            _textLabels.Add(_currentText);

            // Instantiates a button for the new segment to catch raycast for pickup interaction.
            MeshButton button = GameObject.Instantiate(_meshButtonBoxPrefab);
            button.transform.SetParent(transform);
            button.scaleOnSelection = false;

            // Registers for notifications from the UI
            button.OnButtonClicked.AddListener(HandleOnButtonClicked);
            button.OnButtonSelected.AddListener(HandleOnButtonSelected);
            button.OnButtonDeselected.AddListener(HandleOnButtonDeselected);

            GameObject buttonObject = button.gameObject;
            Vector3 lineStart = _currentLine.GetPosition(_currentLine.positionCount - 2);
            Vector3 lineEnd = _currentLine.GetPosition(_currentLine.positionCount - 3);
            Vector3 lineCenter = (lineStart + lineEnd) / 2.0f;
            float lineLength = (lineEnd - lineStart).magnitude;
            buttonObject.transform.position = lineCenter;
            buttonObject.transform.LookAt(lineEnd);
            buttonObject.transform.localScale = new Vector3(LineSelectionColliderWidth, LineSelectionColliderWidth, lineLength);

            //track total distance
            _totalDistance += (point - _mostRecentPoint).magnitude;
            _mostRecentPoint = point;
        }

        /// <summary>
        /// Draws a line from the most recently confirmed point to the supplied point. 
        /// Used to live update the state of the tape measure before next confirmation.
        /// </summary>
        public void Draw(Vector3 point)
        {
            if (_mostRecentPoint == Vector3.zero)
            {
                // This means its the first point
                _mostRecentPoint = point;
                _currentLine.positionCount = 2;
                _currentLine.SetPositions(new Vector3[2] { _mostRecentPoint, _mostRecentPoint });
                return;
            }

            // draw a line to the cursor
            Vector3 distance = point - _mostRecentPoint;
            _currentLine.SetPosition(_currentLine.positionCount - 1, point);

            // place the current text, and update it's label.
            _currentText.transform.position = _mostRecentPoint + distance * 0.5f;
            //orient the text
            Vector3 textForward = Vector3.Cross(distance.normalized, Vector3.up);
            Vector3 textUp = Vector3.Cross(textForward, distance.normalized);
            _currentText.transform.rotation = Quaternion.LookRotation(textForward, textUp);
            _currentText.text = distance.magnitude.ToString("0.00") + "(m)"; // update the text label.

            _tapeDistanceTraveled = (Mathf.Round(distance.magnitude * 100f) / 100f); //up to two decimal places only

            _tapeAudioHandler.MeasuringTick(_tapeDistanceTraveled); //Play tick sound on tape pull
        }

        //----------- Private Methods -----------

        private void HandleOnButtonSelected()
        {
            if (_isDrawing || _isSelected)
            {
                return;
            }

            if (OnMeasureTapeSelected != null)
            {
                OnMeasureTapeSelected(this);
            }
        }

        private void HandleOnButtonDeselected()
        {
            if (!_isSelected)
            {
                return;
            }

            if (OnMeasureTapeDeselected != null)
            {
                OnMeasureTapeDeselected(this);
            }
        }

        private void HandleOnButtonClicked()
        {
            if (OnMeasureTapeClicked != null)
            {
                OnMeasureTapeClicked(this);
            }
        }
    }
}
