// ---------------------------------------------------------------------
//
// Copyright (c) 2018 Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Creator Agreement, located
// here: https://id.magicleap.com/creator-terms
//
// ---------------------------------------------------------------------

using UnityEngine;
using MagicLeap.Utilities;

namespace MagicKit
{
    /// <summary>
    /// This class manages the core state machine for the Measure application. 
    /// </summary>
    public class MeasureApp : Singleton<MeasureApp>
    {
        private enum State
        {
            None,
            RoomMeshingTutorial,
            SummonUiTutorial,
            SelectEnvironmentCubeToolTutorial,
            EnvironmentCubeTutorial,
            SummonUiTutorial2,
            SelectMeasureTapeTutorial,
            StartMeasureTapeTutorial,
            AddPointToMeasureTapeTutorial,
            EndMeasureTapeTutorial,
            SummonUiTutorial3,
            SelectAngleTutorial,
            AngleTutorial,
            ToolInstanceDeletionTutorial,
            Interaction
        }
        private State _state;

        [SerializeField] private BeltPackUi _ui;

        [SerializeField] private GameObject _angleToolButton;
        [SerializeField] private GameObject _environmentCubeToolButton;
        [SerializeField] private GameObject _measureToolButton;

        [SerializeField] private AngleTool _angleTool;
        [SerializeField] private EnvironmentCubeTool _environmentCubeTool;
        [SerializeField] private MeasureTapeTool _measureTool;

        [SerializeField] private GameObject _tutorial1Meshing;
        [SerializeField] private GameObject _tutorial2Toolbelt;
        [SerializeField] private GameObject _tutorial3SelectTool;
        [SerializeField] private GameObject _tutorial4AMeasuringTape;
        [SerializeField] private GameObject _tutorial4BMeasuringTape;
        [SerializeField] private GameObject _tutorial4CMeasuringTape;
        [SerializeField] private GameObject _tutorial5AEnvironmentCube;
        [SerializeField] private GameObject _tutorial5BEnvironmentCube;
        [SerializeField] private GameObject _tutorial6AAngle;
        [SerializeField] private GameObject _tutorial6BAngle;
        [SerializeField] private GameObject _tutorial7Removal;
        [SerializeField] private GameObject _tutorial8Success;

        [SerializeField] private bool _skipTutorial;

        private bool _meshingDone;

        private bool _envCubeSelected;
        private bool _envCubePlaced;
        private bool _envCubeMoved;
        private bool _envCubeRotated;

        private bool _measureTapeStarted;
        private bool _measureTapePointAdded;
        private bool _measureTapeEnded;

        private bool _anglePlaced;
        private bool _angleRotated;
        private bool _angleSelected;

        private bool _firstTime = true;

        private bool _objectDeleted;

        private bool _started;

        private const int TutorialSuccessTimeout = 3;

        private void Start()
        {
            InputManager.Instance.OnTriggerUp += HandleOnTriggerPressed;
            _state = State.None;
        }

        private void Update()
        {
            if (!_started)
            {
                if (_skipTutorial)
                {
                    _state = State.Interaction;
                    InteractionEnter();
                }
                else
                {
                    GoToState(State.RoomMeshingTutorial);
                }
                _started = true;
            }
            UpdateStateMachine();
        }

        //----------- State Machine Methods -----------

        private void GoToState(State newState)
        {
            switch (_state)
            {
                case State.RoomMeshingTutorial:
                    RoomMeshingTutorialExit();
                    break;
                case State.SummonUiTutorial:
                    SummonUiTutorialExit();
                    break;
                case State.SelectEnvironmentCubeToolTutorial:
                    SelectEnvironmentCubeToolTutorialExit();
                    break;
                case State.EnvironmentCubeTutorial:
                    EnvironmentCubeTutorialExit();
                    break;
                case State.SummonUiTutorial2:
                    SummonUiTutorial2Exit();
                    break;
                case State.SelectMeasureTapeTutorial:
                    SelectMeasureTapeTutorialExit();
                    break;
                case State.StartMeasureTapeTutorial:
                    StartMeasureTapeTutorialExit();
                    break;
                case State.AddPointToMeasureTapeTutorial:
                    AddPointToMeasureTapeTutorialExit();
                    break;
                case State.EndMeasureTapeTutorial:
                    EndMeasureTapeTutorialExit();
                    break;
                case State.SummonUiTutorial3:
                    SummonUiTutorial3Exit();
                    break;
                case State.SelectAngleTutorial:
                    SelectAngleTutorialExit();
                    break;
                case State.AngleTutorial:
                    AngleTutorialExit();
                    break;
                case State.ToolInstanceDeletionTutorial:
                    ToolInstanceDeletionTutorialExit();
                    break;
                case State.Interaction:
                    InteractionExit();
                    break;

            }

            _state = newState;

            switch (_state)
            {
                case State.RoomMeshingTutorial:
                    RoomMeshingTutorialEnter();
                    break;
                case State.SummonUiTutorial:
                    SummonUiTutorialEnter();
                    break;
                case State.SelectEnvironmentCubeToolTutorial:
                    SelectEnvironmentCubeToolTutorialEnter();
                    break;
                case State.EnvironmentCubeTutorial:
                    EnvironmentCubeTutorialEnter();
                    break;
                case State.SummonUiTutorial2:
                    SummonUiTutorial2Enter();
                    break;
                case State.SelectMeasureTapeTutorial:
                    SelectMeasureTapeTutorialEnter();
                    break;
                case State.StartMeasureTapeTutorial:
                    StartMeasureTapeTutorialEnter();
                    break;
                case State.AddPointToMeasureTapeTutorial:
                    AddPointToMeasureTapeTutorialEnter();
                    break;
                case State.EndMeasureTapeTutorial:
                    EndMeasureTapeTutorialEnter();
                    break;
                case State.SummonUiTutorial3:
                    SummonUiTutorial3Enter();
                    break;
                case State.SelectAngleTutorial:
                    SelectAngleTutorialEnter();
                    break;
                case State.AngleTutorial:
                    AngleTutorialEnter();
                    break;
                case State.ToolInstanceDeletionTutorial:
                    ToolInstanceDeletionTutorialEnter();
                    break;
                case State.Interaction:
                    InteractionEnter();
                    break;
            }
        }

        private void UpdateStateMachine()
        {
            switch (_state)
            {
                case State.RoomMeshingTutorial:
                    RoomMeshingTutorialUpdate();
                    break;
                case State.SummonUiTutorial:
                    SummonUiTutorialUpdate();
                    break;
                case State.SelectEnvironmentCubeToolTutorial:
                    SelectEnvironmentCubeToolTutorialUpdate();
                    break;
                case State.EnvironmentCubeTutorial:
                    EnvironmentCubeTutorialUpdate();
                    break;
                case State.SummonUiTutorial2:
                    SummonUiTutorial2Update();
                    break;
                case State.SelectMeasureTapeTutorial:
                    SelectMeasureTapeTutorialUpdate();
                    break;
                case State.StartMeasureTapeTutorial:
                    StartMeasureTapeTutorialUpdate();
                    break;
                case State.AddPointToMeasureTapeTutorial:
                    AddPointToMeasureTapeTutorialUpdate();
                    break;
                case State.EndMeasureTapeTutorial:
                    EndMeasureTapeTutorialUpdate();
                    break;
                case State.SummonUiTutorial3:
                    SummonUiTutorial3Update();
                    break;
                case State.SelectAngleTutorial:
                    SelectAngleTutorialUpdate();
                    break;
                case State.AngleTutorial:
                    AngleTutorialUpdate();
                    break;
                case State.ToolInstanceDeletionTutorial:
                    ToolInstanceDeletionTutorialUpdate();
                    break;
                case State.Interaction:
                    InteractionUpdate();
                    break;
            }
        }

        //----------- MeshingTutorial State Methods -----------

        private void RoomMeshingTutorialEnter()
        {
            _tutorial1Meshing.SetActive(true);
            _ui.showTools = false;
        }

        private void RoomMeshingTutorialUpdate()
        {
            if (_meshingDone)
            {
                GoToState(State.SummonUiTutorial);
            }
        }

        private void RoomMeshingTutorialExit()
        {
            _tutorial1Meshing.SetActive(false);
        }

        //----------- SummonUiTutorial State Methods -----------

        private void SummonUiTutorialEnter()
        {
            _ui.gameObject.SetActive(true);
            BeltPackUi.Instance.allowDeletion = false;
            _ui.canUseBumper = true;
            _tutorial2Toolbelt.SetActive(true);
        }

        private void SummonUiTutorialUpdate()
        {
            if (_ui.isDisplaying)
            {
                _ui.canUseBumper = false;
                _ui.HideUIButtons();
                GoToState(State.SelectEnvironmentCubeToolTutorial);
            }
        }

        private void SummonUiTutorialExit()
        {
            _tutorial2Toolbelt.SetActive(false);
        }


        private void SelectEnvironmentCubeToolTutorialEnter()
        {
            _environmentCubeToolButton.SetActive(true);
            _ui.OnEnvCubeSelected += HandleOnEnvCubeSelected;
            _tutorial3SelectTool.SetActive(true);
        }

        private void SelectEnvironmentCubeToolTutorialUpdate()
        {
            if (_envCubeSelected)
            {
                GoToState(State.EnvironmentCubeTutorial);
            }
        }

        private void SelectEnvironmentCubeToolTutorialExit()
        {
            _ui.gameObject.SetActive(false);
            _tutorial3SelectTool.SetActive(false);
        }

        //----------- EnvironmentCubeTutorial State Methods -----------

        private void EnvironmentCubeTutorialEnter()
        {
            _environmentCubeTool.OnMove += HandleOnEnvCubeMoved;
            _environmentCubeTool.OnRotate += HandleOnEnvCubeRotated;
            _environmentCubeTool.triggerEnabled = false;
            _tutorial5BEnvironmentCube.SetActive(true);
            _firstTime = true;
        }

        private void EnvironmentCubeTutorialUpdate()
        {
            // Make sure all events are satisfied and the UI is shown again
            if (_envCubeRotated && _envCubeMoved && _firstTime)
            {
                _environmentCubeTool.OnPlaced += HandleOnEnvCubePlaced;
                _environmentCubeTool.triggerEnabled = true;
                _tutorial5BEnvironmentCube.SetActive(false);
                _tutorial5AEnvironmentCube.SetActive(true);
                // Register place callback
                _firstTime = false;
            }

            if (_envCubePlaced)
            {
                _environmentCubeTool.triggerEnabled = false;
                _tutorial5AEnvironmentCube.SetActive(false);
            }
            if (_envCubeRotated && _envCubeMoved && _envCubePlaced)
            {
                GoToState(State.SummonUiTutorial2);
            }
        }

        private void EnvironmentCubeTutorialExit()
        {
            _environmentCubeToolButton.SetActive(false);
        }

        private void SummonUiTutorial2Enter()
        {
            _ui.gameObject.SetActive(true);
            _ui.canUseBumper = true;
            _tutorial2Toolbelt.SetActive(true);
        }

        private void SummonUiTutorial2Update()
        {
            if (_ui.isDisplaying)
            {
                BeltPackUi.Instance.canUseBumper = false;
                _ui.HideUIButtons();
                GoToState(State.SelectMeasureTapeTutorial);
            }
        }

        private void SummonUiTutorial2Exit()
        {
            _tutorial2Toolbelt.SetActive(false);
        }

        //----------- TutorialMeasureTapeStart State Methods -----------

        private void SelectMeasureTapeTutorialEnter()
        {
            _tutorial3SelectTool.SetActive(true);
            _ui.gameObject.SetActive(true);
            _measureToolButton.SetActive(true);
            _ui.OnTapeMeasureSelected += HandleOnMeasureTapeSelected;
        }

        private void SelectMeasureTapeTutorialUpdate()
        {
        }

        private void SelectMeasureTapeTutorialExit()
        {
            _tutorial3SelectTool.SetActive(false);
            _measureToolButton.SetActive(false);
            _ui.gameObject.SetActive(false);
            _ui.OnTapeMeasureSelected -= HandleOnMeasureTapeSelected;
        }

        private void HandleOnMeasureTapeSelected()
        {
            GoToState(State.StartMeasureTapeTutorial);
        }

        //----------- TutorialMeasureTapeStart State Methods -----------

        private void StartMeasureTapeTutorialEnter()
        {
            _tutorial4AMeasuringTape.SetActive(true);
            _measureTool.OnMeasureTapeStarted += HandleOnMeasureTapeStarted;
        }

        private void StartMeasureTapeTutorialUpdate()
        {
        }

        private void StartMeasureTapeTutorialExit()
        {
            _tutorial4AMeasuringTape.SetActive(false);
            _measureTool.OnMeasureTapeStarted -= HandleOnMeasureTapeStarted;
        }

        private void HandleOnMeasureTapeStarted()
        {
            GoToState(State.AddPointToMeasureTapeTutorial);
        }

        //----------- TutorialAddPointToMeasureTape State Methods -----------

        private void AddPointToMeasureTapeTutorialEnter()
        {
            _tutorial4BMeasuringTape.SetActive(true);
            _measureTool.OnMeasureTapePointAdded += HandleOnMeasureTapePointAdded;
        }

        private void AddPointToMeasureTapeTutorialUpdate()
        {
        }

        private void AddPointToMeasureTapeTutorialExit()
        {
            _tutorial4BMeasuringTape.SetActive(false);

            _measureTool.OnMeasureTapePointAdded -= HandleOnMeasureTapePointAdded;
        }

        private void HandleOnMeasureTapePointAdded()
        {
            GoToState(State.EndMeasureTapeTutorial);
        }

        //----------- TutorialEndMeasureTape State Methods -----------

        private void EndMeasureTapeTutorialEnter()
        {
            _measureTool.allowPlacement = true;
            _measureTool.OnMeasureTapeEnded += HandleOnMeasureTapeEnded;

            _tutorial4CMeasuringTape.SetActive(true);
        }

        private void EndMeasureTapeTutorialUpdate()
        {
        }

        private void EndMeasureTapeTutorialExit()
        {
            _measureTool.allowPlacement = false;
            _measureTool.OnMeasureTapeEnded -= HandleOnMeasureTapeEnded;

            _tutorial4CMeasuringTape.SetActive(false);
        }

        private void HandleOnMeasureTapeEnded()
        {
            GoToState(State.SummonUiTutorial3);
        }

        //----------- SummonUiTutorial3 State Methods -----------

        private void SummonUiTutorial3Enter()
        {
            _ui.gameObject.SetActive(true);
            _ui.canUseBumper = true;
            _tutorial2Toolbelt.SetActive(true);
        }

        private void SummonUiTutorial3Update()
        {
            if (_ui.isDisplaying)
            {
                _ui.canUseBumper = false;
                _ui.HideUIButtons();
                GoToState(State.SelectAngleTutorial);
            }
        }
        private void SummonUiTutorial3Exit()
        {
            _tutorial2Toolbelt.SetActive(false);
        }

        //----------- AngleTutorial State Methods -----------

        private void SelectAngleTutorialEnter()
        {
            _ui.OnAngleToolSelected += HandleOnAngleToolSelected;
            _tutorial3SelectTool.SetActive(true);
            _angleToolButton.SetActive(true);
        }
        private void SelectAngleTutorialUpdate()
        {
            if (_angleSelected)
            {
                GoToState(State.AngleTutorial);
            }
        }

        private void SelectAngleTutorialExit()
        {
            _ui.gameObject.SetActive(false);
            _tutorial3SelectTool.SetActive(false);
        }

        private void AngleTutorialEnter()
        {
            _firstTime = true;
            _tutorial6BAngle.SetActive(true);
            _angleTool.triggerEnabled = false;
            _angleTool.OnRotated += HandleOnAngleRotated;
        }

        private void AngleTutorialUpdate()
        {
            if (_angleRotated && _firstTime)
            {
                _tutorial6BAngle.SetActive(false);
                _tutorial6AAngle.SetActive(true);
                _angleTool.OnPlaced += HandleOnAnglePlaced;
                _angleTool.triggerEnabled = true;
                _firstTime = false;
            }
            if (_anglePlaced)
            {
                _angleTool.triggerEnabled = false;
                _tutorial6AAngle.SetActive(false);
            }
            if (_anglePlaced && _angleRotated)
            {
                GoToState(State.ToolInstanceDeletionTutorial);
            }
        }

        private void AngleTutorialExit()
        {
            _angleToolButton.SetActive(false);
        }

        //----------- ToolInstanceDeletionTutorial State Methods -----------

        private void ToolInstanceDeletionTutorialEnter()
        {
            _ui.gameObject.SetActive(true);
            _ui.allowDeletion = true;
            _measureTool.allowDeletion = true;
            _angleTool.OnDeleted += HandleOnObjectDeleted;
            _measureTool.OnMeasureTapeDeleted += HandleOnObjectDeleted;
            _environmentCubeTool.OnDeleted += HandleOnObjectDeleted;

            _tutorial7Removal.SetActive(true);
        }

        private void ToolInstanceDeletionTutorialUpdate()
        {
            if (_objectDeleted)
            {
                GoToState(State.Interaction);
            }
        }

        private void ToolInstanceDeletionTutorialExit()
        {
            _angleTool.OnDeleted -= HandleOnObjectDeleted;
            _measureTool.OnMeasureTapeDeleted -= HandleOnObjectDeleted;
            _environmentCubeTool.OnDeleted -= HandleOnObjectDeleted;
            _angleTool.triggerEnabled = true;
            _environmentCubeTool.triggerEnabled = true;

            _tutorial7Removal.SetActive(false);
        }

        //----------- Interaction State Methods -----------

        private void InteractionEnter()
        {
            if (!_skipTutorial)
            {
                _tutorial8Success.SetActive(true);
                _ui.canUseBumper = true;
                _ui.showTools = true;
                Invoke("TutorialDone", TutorialSuccessTimeout);
            }

            _ui.gameObject.SetActive(true);

            _measureTool.allowPlacement = true;
            _measureTool.allowDeletion = true;
        }

        private void InteractionUpdate()
        {
        }

        private void InteractionExit()
        {
        }

        private void TutorialDone()
        {
            _tutorial8Success.SetActive(false);
        }

        private void HandleOnTriggerPressed()
        {
            _meshingDone = true;
        }

        private void HandleOnEnvCubePlaced()
        {
            _envCubePlaced = true;
        }
        private void HandleOnEnvCubeMoved()
        {
            _envCubeMoved = true;
        }
        private void HandleOnEnvCubeRotated()
        {
            _envCubeRotated = true;
        }

        private void HandleOnAnglePlaced()
        {
            _anglePlaced = true;
        }
        private void HandleOnAngleRotated()
        {
            _angleRotated = true;
        }

        private void HandleOnObjectDeleted()
        {
            _objectDeleted = true;
        }

        private void HandleOnEnvCubeSelected()
        {
            _envCubeSelected = true;
        }
        private void HandleOnAngleToolSelected()
        {
            _angleSelected = true;
        }
    }
}
