using System;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Rendering;
using SharpDX;
using Color = System.Drawing.Color;

namespace Kalista_Beta_Fixed
{
    public static class DamageIndicator
    {
        private const int BarWidth = 104;
        private const int LineThickness = 9;

        public delegate float DamageToUnitDelegate(AIHeroClient hero);

        private static DamageToUnitDelegate DamageToUnit { get; set; }

        private static readonly Vector2 BarOffset = new Vector2(1.25f, 14.25f);
        private static readonly Vector2 PercentOffset = new Vector2(0, -15);

        private static Color _drawingColor;
        public static Color DrawingColor
        {
            get { return _drawingColor; }
            set { _drawingColor = Color.FromArgb(170, value); }
        }

        private static Line OverlayLine { get; set; }

        public static bool HealthbarEnabled { get; set; }
        public static bool PercentEnabled { get; set; }

        public static void Initialize(DamageToUnitDelegate damageToUnit)
        {
            DamageToUnit = damageToUnit;
            DrawingColor = Color.White;
            HealthbarEnabled = true;

            OverlayLine = new Line
            {
                Antialias = true,
                Width = LineThickness
            };

            Drawing.OnEndScene += OnEndScene;
        }

        private static void OnEndScene(EventArgs args)
        {
            if (HealthbarEnabled || PercentEnabled)
            {
                foreach (var unit in EntityManager.Heroes.Enemies.Where(u => u.IsValidTarget() && u.IsHPBarRendered))
                {
                    var damage = DamageToUnit(unit);

                    if (damage <= 0)
                    {
                        continue;
                    }

                    if (HealthbarEnabled)
                    {
                        var damagePercentage = (unit.TotalShieldHealth() - damage > 0 ? unit.TotalShieldHealth() - damage : 0) /
                                               (unit.MaxHealth + unit.AllShield + unit.AttackShield + unit.MagicShield);
                        var currentHealthPercentage = unit.TotalShieldHealth() / (unit.MaxHealth + unit.AllShield + unit.AttackShield + unit.MagicShield);

                        var startPoint = new Vector2(unit.HPBarPosition.X + BarOffset.X + damagePercentage * BarWidth, unit.HPBarPosition.Y + BarOffset.Y - 5);
                        var endPoint = new Vector2(unit.HPBarPosition.X + BarOffset.X + currentHealthPercentage * BarWidth + 1, unit.HPBarPosition.Y + BarOffset.Y - 5);

                        OverlayLine.Draw(DrawingColor, startPoint, endPoint);
                        Drawing.DrawLine(startPoint, endPoint, LineThickness, DrawingColor);
                    }

                    if (PercentEnabled)
                    {
                        Drawing.DrawText(unit.HPBarPosition + PercentOffset, DrawingColor, string.Concat(Math.Ceiling(damage / unit.TotalShieldHealth() * 100), "%"), 10);
                    }
                }
            }
        }
    }
}
