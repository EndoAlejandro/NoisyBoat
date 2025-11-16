using System.Collections.Generic;
using Shapes;
using UnityEngine;

namespace Drawing
{
    public class DrawingManager : ImmediateModeShapeDrawer
    {
        public static DrawingManager Instance { get; private set; }

        [Header("Disc")]
        [SerializeField] private float _radius = 10f;

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
                Draw.Thickness = .1f;

                Draw.Matrix = transform.localToWorldMatrix;
                foreach (SonarDisc disc in _sonarDiscs)
                {
                    if (!disc.Animating) continue;
                    Draw.Ring(disc.Position, Quaternion.Euler(Vector3.right * 90f), disc.CurrentSize, Color.black);
                }
            }
        }

        private void SonarOnTargetFound(Transform target)
        {
            foreach (SonarDisc disc in _sonarDiscs)
            {
                if (disc.Animating) continue;
                disc.Draw(null, target.position);
                return;
            }

            var newDisc = new SonarDisc(target.position, _radius, _duration);
            newDisc.Draw(null);
            _sonarDiscs.Add(newDisc);
        }
    }
}