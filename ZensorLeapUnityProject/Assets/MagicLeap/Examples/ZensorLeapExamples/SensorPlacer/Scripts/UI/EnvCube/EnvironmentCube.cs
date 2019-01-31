// ---------------------------------------------------------------------
//
// Copyright (c) 2018 Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Creator Agreement, located
// here: https://id.magicleap.com/creator-terms
//
// ---------------------------------------------------------------------

using UnityEngine;
using UnityEngine.UI;

namespace MagicKit
{
    ///<summary>
    /// EnvironmentCube - This class represents an environment cube that has been placed in the world.
    /// It has six lines and potentially six circles.
    ///</summary>
    public class EnvironmentCube : MonoBehaviour
    {

        //----------- Private Members -----------

        [SerializeField] private GameObject _linePrefab;
        [SerializeField] private GameObject _dotPrefab;
        [SerializeField] private GameObject _guiPrefab;
        [SerializeField] private LayerMask _mask;

        private GameObject _left;
        private GameObject _right;
        private GameObject _up;
        private GameObject _down;
        private GameObject _forward;
        private GameObject _backward;

        private LineRenderer _leftLine;
        private LineRenderer _rightLine;
        private LineRenderer _upLine;
        private LineRenderer _downLine;
        private LineRenderer _forwardLine;
        private LineRenderer _backwardLine;

        private Dimensions _dimensions;

        private GameObject _leftCircle;
        private GameObject _rightCircle;
        private GameObject _upCircle;
        private GameObject _downCircle;
        private GameObject _forwardCircle;
        private GameObject _backwardCircle;
        private GameObject _text;
        private Vector3 _textScale;

        private bool _placed;

        private bool _heightValid;
        private bool _widthValid;
        private bool _lengthValid;

        private const int DefaultLineDistance = 10;
        // RGB (67, 144, 219)
        private Color WidthColor = new Color(0.26f, 0.56f, 0.86f, 1f);
        // RGB (34, 204, 127)
        private Color HeightColor = new Color(0.13f, 0.8f, 0.5f, 1f);
        // RGB (221, 9, 52)
        private Color LengthColor = new Color(0.87f, 0.04f, 0.2f, 1f);

        private const float TextOffsetY = 0.125f;
        private const float TextScale = 1.2f;
        private const float UpLineOffset = 0.17f;
        private const string NoDimensionAvailable = "N/A";
        private const string Meters = "m";
        private const string Valid = "_Valid";
        private const string Point = "_Point01";

        //----------- MonoBehaviour Methods -----------

        public void Update()
        {
            if (!_placed)
            {
                return;
            }

            RaycastHit left;
            RaycastHit right;
            RaycastHit up;
            RaycastHit down;
            RaycastHit forward;
            RaycastHit backward;

            // Shoot rays in 6 directions to get measurements
            Physics.Raycast(transform.position, transform.right, out right, Mathf.Infinity, _mask);
            Physics.Raycast(transform.position, -transform.right, out left, Mathf.Infinity, _mask);
            Physics.Raycast(transform.position, transform.up, out up, Mathf.Infinity, _mask);
            Physics.Raycast(transform.position, -transform.up, out down, Mathf.Infinity, _mask);
            Physics.Raycast(transform.position, transform.forward, out forward, Mathf.Infinity, _mask);
            Physics.Raycast(transform.position, -transform.forward, out backward, Mathf.Infinity, _mask);

            // Update the lines and anchor dots 
            UpdateLineAndAnchor(_rightLine, _rightCircle, right, transform.position, transform.right, WidthColor);
            UpdateLineAndAnchor(_leftLine, _leftCircle, left, transform.position, -transform.right, WidthColor);
            UpdateLineAndAnchor(_forwardLine, _forwardCircle, forward, transform.position, transform.forward, LengthColor);
            UpdateLineAndAnchor(_backwardLine, _backwardCircle, backward, transform.position, -transform.forward, LengthColor);
            UpdateLineAndAnchor(_upLine, _upCircle, up, new Vector3(transform.position.x, transform.position.y + UpLineOffset, transform.position.z), transform.up, HeightColor);
            UpdateLineAndAnchor(_downLine, _downCircle, down, transform.position, -transform.up, HeightColor);

            // Set flags for valid width, length and height
            _widthValid = (left.collider != null && right.collider != null);
            _lengthValid = (forward.collider != null && backward.collider != null);
            _heightValid = (up.collider != null && down.collider != null);

            // Update the dimension text
            UpdateText();
        }

        //----------- Public Methods -----------

        ///<summary>
        ///  This function is called to place this object in the world. It instantiates the objects associated with this instance
        ///</summary>
        ///
        public void Place()
        {
            _rightCircle = MakeCircle();
            _leftCircle = MakeCircle();
            _upCircle = MakeCircle();
            _downCircle = MakeCircle();
            _forwardCircle = MakeCircle();
            _backwardCircle = MakeCircle();

            _left = MakeLine();
            _leftLine = _left.GetComponent<LineRenderer>();
            _right = MakeLine();
            _rightLine = _right.GetComponent<LineRenderer>();
            _up = MakeLine();
            _upLine = _up.GetComponent<LineRenderer>();
            _down = MakeLine();
            _downLine = _down.GetComponent<LineRenderer>();
            _forward = MakeLine();
            _forwardLine = _forward.GetComponent<LineRenderer>();
            _backward = MakeLine();
            _backwardLine = _backward.GetComponent<LineRenderer>();

            _text = Instantiate(_guiPrefab);
            _dimensions = _text.GetComponent<Dimensions>();
            _textScale = _text.transform.localScale;

            _placed = true;
        }

        ///<summary>
        ///  This function is called when the user picks up this tool. It effectively removes it from the world
        ///</summary> 
        public void OnPickUp()
        {
            Destroy(_leftCircle);
            Destroy(_left);
            Destroy(_right);
            Destroy(_up);
            Destroy(_down);
            Destroy(_forward);
            Destroy(_backward);
            Destroy(_forwardCircle);
            Destroy(_backwardCircle);
            Destroy(_rightCircle);
            Destroy(_upCircle);
            Destroy(_downCircle);
            Destroy(_text);
            Destroy(gameObject);
        }

        ///<summary>
        ///  This function is called when the user hovers an instance of the tool. It will scale the text to be more readable
        ///</summary> 
        public void OnSelect()
        {
            _text.GetComponent<RectTransform>().localScale = _textScale * TextScale;
        }

        ///<summary>
        ///  This function is called when the user hovers an instance of the tool. It will scale the text to be normal size;
        ///</summary> 
        public void OnDeselect()
        {
            _text.GetComponent<RectTransform>().localScale = _textScale;
        }

        //----------- Private Methods -----------
        
        private GameObject MakeLine()
        {
            GameObject obj = Instantiate(_linePrefab);
            obj.transform.SetParent(transform, false);
            LineRenderer line = obj.GetComponent<LineRenderer>();
            line.positionCount = 2;

            return obj;
        }
        
        private GameObject MakeCircle()
        {
            GameObject obj = Instantiate(_dotPrefab);
            obj.transform.SetParent(transform, false);
            obj.SetActive(false);

            return obj;
        }
 
        private void UpdateText()
        {
            string height = NoDimensionAvailable;
            if (_heightValid)
            {
                height = Vector3.Distance(_upLine.GetPosition(0), _downLine.GetPosition(0)).ToString("0.00") + Meters;
            }

            string width = NoDimensionAvailable;
            if (_widthValid)
            {
                width = Vector3.Distance(_rightLine.GetPosition(0), _leftLine.GetPosition(0)).ToString("0.00") + Meters;
            }

            string length = NoDimensionAvailable;
            if (_lengthValid)
            {
                length = Vector3.Distance(_forwardLine.GetPosition(0), _backwardLine.GetPosition(0)).ToString("0.00") + Meters;
            }

            _dimensions.SetDimensions(length, width, height);
            _text.transform.position = new Vector3(transform.position.x, transform.position.y + TextOffsetY, transform.position.z);
        }

        ///<summary>
        ///  This function updates lines and anchors as different parts of the environemnt are meshed
        ///</summary> 
        private void UpdateLineAndAnchor(LineRenderer line, GameObject anchor, RaycastHit hit, Vector3 startPoint, Vector3 lineDir, Color color)
        {
            if (hit.collider != null)
            {
                line.SetPosition(0, hit.point);
                line.material.SetInt(Point, 1);
                line.SetPosition(1, startPoint);
                line.material.SetInt(Valid, 1);
                anchor.SetActive(true);
                anchor.transform.position = hit.point;
                anchor.transform.rotation = Quaternion.LookRotation(hit.normal);
                Image[] ims = anchor.GetComponentsInChildren<Image>();
                foreach (Image im in ims)
                {
                    im.color = color;
                }
            }
            else
            {
                anchor.SetActive(false);
                // Draw just a line if nothing was hit
                line.SetPosition(0, startPoint);
                line.material.SetInt(Point, 0);
                line.SetPosition(1, transform.position + (lineDir * DefaultLineDistance));
                line.material.SetInt(Valid, 0);
            }

            anchor.GetComponent<MeshAnchor>().Adjust();
            // Now project the new position onto the direction of the line
            // This ensures that the line will always be in the center of the dot
            Vector3 offset = anchor.transform.position - hit.point;
            Vector3 newOffset = Vector3.Project(offset, -lineDir);
            anchor.transform.position = hit.point + newOffset;
        }
    }

}