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
    /// Convenience class for setting the length, width, and height values in
    /// the EnvCube GUI
    ///</summary>
    public class Dimensions : MonoBehaviour
    {

        //----------- Private Members -----------

        [SerializeField] private Text _heightText;
        [SerializeField] private Text _widthText;
        [SerializeField] private Text _lengthText;

        //----------- Public Methods -----------

        ///<summary>
        ///  This function will set the values of the width, height, and length
        ///</summary>
        public void SetDimensions(string length, string width, string height)
        {
            _heightText.text = height;
            _widthText.text = width;
            _lengthText.text = length;
        }
    }
}