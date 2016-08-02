using System;
using System.Windows;
using SimTemplate.StateMachine;
using SimTemplate.DataTypes.Enums;
using SimTemplate.DataTypes;

namespace SimTemplate.ViewModels
{
    public partial class TemplatingViewModel
    {
        public abstract class TemplatingState : State
        {
            #region Constructor

            public TemplatingState(TemplatingViewModel outer)
                : base(outer)
            { }

            #endregion

            #region Virtual Methods

            #region View Methods

            public virtual void PositionInput(Point position) { MethodNotImplemented(); }

            public virtual void PositionUpdate(Point position) { MethodNotImplemented(); }

            public virtual void RemoveMinutia(int index) { MethodNotImplemented(); }

            public virtual void MoveMinutia(Point position) { MethodNotImplemented(); }

            public virtual void StartMove(int index) { MethodNotImplemented(); }

            #endregion

            #region ITemplatingViewModel Methods

            public virtual void BeginInitialise() { MethodNotImplemented(); }

            public virtual void BeginTemplating(CaptureInfo capture) { MethodNotImplemented(); }

            public virtual void EscapeAction() { MethodNotImplemented(); }

            public virtual void SetMinutiaType(MinutiaType type) { MethodNotImplemented(); }

            public virtual byte[] FinaliseTemplate() { MethodNotImplemented(); return null; }

            #endregion

            #endregion

            #region Protected Members

            /// <summary>
            /// Gets the outer class that this state is behaviour for.
            /// </summary>
            protected TemplatingViewModel Outer { get { return (TemplatingViewModel)BaseOuter; } }

            protected void TransitionTo(Type newState)
            {
                Outer.m_StateMgr.TransitionTo(newState);
            }

            #endregion
        }
    }
}
