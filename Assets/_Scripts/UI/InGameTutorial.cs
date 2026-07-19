using PlayerComponents;
using UnityEngine;

namespace UI
{
    public class InGameTutorial : MonoBehaviour
    {
        private enum TutorialSteps
        {
            None,
            Sonar,
            Movement,
            Done,
        }

        [SerializeField] private GameObject _background;

        [Header("Sonar")]
        [SerializeField] private GameObject _sonarButton;

        [SerializeField] private GameObject _sonarTutorialPanel;

        [Header("Movement")]
        [SerializeField] private GameObject _movementJoystick;

        [SerializeField] private GameObject _movementTutorialPanel;

        private TutorialSteps _step = TutorialSteps.None;

        private void Awake()
        {
            MainMenuUI.GameStarted += MainMenuUIOnGameStarted;
        }

        private void Start()
        {
            PanelVisibility(TutorialSteps.None, false);
        }

        private void Update()
        {
            switch (_step)
            {
                case TutorialSteps.Sonar:
                    CheckSonarStep();
                    break;
                case TutorialSteps.Movement:
                    CheckMovementStep();
                    break;
            }
        }

        private void PanelVisibility(TutorialSteps step, bool isVisible)
        {
            _background.SetActive(isVisible);
            switch (step)
            {
                case TutorialSteps.Sonar:
                    _sonarButton.SetActive(isVisible);
                    _sonarTutorialPanel.SetActive(isVisible);
                    //_movementJoystick.SetActive(false);
                    break;
                case TutorialSteps.Movement:
                    _movementJoystick.SetActive(isVisible);
                    _movementTutorialPanel.SetActive(isVisible);
                    break;
                default:
                    _sonarButton.SetActive(true);
                    _sonarTutorialPanel.SetActive(isVisible);

                    _movementJoystick.SetActive(true);
                    _movementTutorialPanel.SetActive(isVisible);
                    break;
            }
        }

        private void CheckSonarStep()
        {
            if (!InputReader.Sonar) return;
            _step = TutorialSteps.None;
            PanelVisibility(TutorialSteps.Sonar, false);
            Invoke(nameof(LaterSonarCheck), 1f);
        }

        private void LaterSonarCheck()
        {
            _step = TutorialSteps.Movement;
            PanelVisibility(TutorialSteps.Movement, true);
        }

        private void CheckMovementStep()
        {
            if (InputReader.Move.magnitude < 0.01f) return;

            _step = TutorialSteps.Done;
            PanelVisibility(TutorialSteps.Done, false);
            InGameMessages.Instance.ShowMessage(0);
        }

        private void MainMenuUIOnGameStarted()
        {
            _step = TutorialSteps.Sonar;
            PanelVisibility(TutorialSteps.Sonar, true);
        }
    }
}