using System.Windows;

namespace SimTemplate.ViewModels
{
    public partial class TemplatingViewModel
    {
        public class Idle : Initialised
        {
            #region Constructor

            public Idle(TemplatingViewModel outer) : base(outer)
            { }

            #endregion

            #region Virtual Overrides

            public override void OnEnteringState()
            {
                base.OnEnteringState();

                // Hide old image from UI, and remove other things.
                Outer.Capture = null;
                Outer.Minutae.Clear();
            }

            #region View Methods

            public override void PositionUpdate(Point position)
            {
                // Ignore a position change when Idle.
            }

            #endregion

            #endregion
        }
    }
}
