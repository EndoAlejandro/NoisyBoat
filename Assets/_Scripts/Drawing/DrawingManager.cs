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
        [SerializeField] private Color _innerDiscColor;
        [SerializeField] private Color _outerDiscColor;

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

                    _innerDiscColor.a = Mathf.Min(disc.CurrentAlpha, 0f);
                    _outerDiscColor.a = Mathf.Min(disc.CurrentAlpha, 1f);

                    var colors = new DiscColors() {
                        innerStart = _innerDiscColor,
                        innerEnd = _innerDiscColor,
                        outerStart = _outerDiscColor,
                        outerEnd = _outerDiscColor,
                    };

                    Draw.Ring(disc.Position, Quaternion.Euler(Vector3.right * 90f), disc.CurrentSize, colors);
                }
            }
        }

        private void SonarOnTargetFound(Transform target)
        {
            foreach (SonarDisc disc in _sonarDiscs)
            {
                if (disc.IsAnimating) continue;
                disc.Draw(null, target.position);
                return;
            }

            var newDisc = new SonarDisc(target.position, _radius, _duration);
            newDisc.Draw(null);
            _sonarDiscs.Add(newDisc);
        }
    }
}