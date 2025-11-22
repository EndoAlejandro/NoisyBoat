using System.Collections.Generic;
using Enemies;
using Shapes;
using UnityEngine;
using UnityEngine.Serialization;

namespace Drawing
{
    public class DrawingManager : ImmediateModeShapeDrawer
    {
        public static DrawingManager Instance { get; private set; }

        [SerializeField] private float _targetRadius = 15f;
        [SerializeField] private float _enemyRadius = 50f;
        [SerializeField] private float _beaconRadius = 30f;
        [SerializeField] private float _duration = 1f;

        private List<SonarDisc> _sonarDiscs;

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        private void Start()
        {
            _sonarDiscs = new List<SonarDisc>();

            Sonar.OnTargetFound += SonarOnTargetFound;
        }

        public override void DrawShapes(Camera cam)
        {
            using (Draw.Command(cam))
            {
                Draw.LineGeometry = LineGeometry.Volumetric3D;
                Draw.ThicknessSpace = ThicknessSpace.Meters;

                Draw.Matrix = transform.localToWorldMatrix;


                foreach (SonarDisc disc in _sonarDiscs)
                {
                    if (!disc.IsAnimating) continue;

                    Draw.Thickness = Mathf.Min(5f, disc.CurrentSize);

                    var innerColor = disc.Color;
                    innerColor.a = Mathf.Min(disc.CurrentAlpha, 0f);
                    var outerDiscColor = disc.Color;
                    outerDiscColor.a = Mathf.Min(disc.CurrentAlpha, 1f);

                    var colors = new DiscColors() {
                        innerStart = innerColor,
                        innerEnd = innerColor,
                        outerStart = outerDiscColor,
                        outerEnd = outerDiscColor,
                    };

                    Draw.Ring(disc.Position, Quaternion.Euler(Vector3.right * 90f), disc.CurrentSize, colors);
                }
            }
        }

        private void SonarOnTargetFound(SonarImmediateDrawer target)
        {
            foreach (SonarDisc disc in _sonarDiscs)
            {
                if (disc.IsAnimating) continue;
                disc.Draw(target.BaseColor, target.transform.position);
                return;
            }

            var radius = target switch {
                Shark => _enemyRadius,
                Beacon => _beaconRadius,
                Target => _targetRadius,
                _ => _targetRadius,
            };

            var newDisc = new SonarDisc(target.transform.position, radius, _duration, target.BaseColor);
            newDisc.Draw(target.BaseColor, target.transform.position);
            _sonarDiscs.Add(newDisc);
        }
    }
}