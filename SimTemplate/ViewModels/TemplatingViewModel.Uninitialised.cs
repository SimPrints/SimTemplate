using SimTemplate.DataTypes.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimTemplate.ViewModels
{
    public partial class TemplatingViewModel
    {
        public class Uninitialised : TemplatingState
        {
            #region Constructor

            public Uninitialised(TemplatingViewModel outer) : base(outer)
            { }

            #endregion

            #region Overriden Methods

            public override void OnEnteringState()
            {
                // Set initial values
                Outer.m_InputMinutiaType = MinutiaType.Termination;
            }

            #region ViewModel Methods

            public override void BeginInitialise()
            {
                TransitionTo(typeof(Idle));
            }

            #endregion

            #endregion

        }
    }
}
