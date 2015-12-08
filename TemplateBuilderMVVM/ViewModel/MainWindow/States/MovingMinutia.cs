using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using TemplateBuilder.Helpers;

namespace TemplateBuilder.ViewModel.MainWindow.States
{
    public class MovingMinutia : Templating
    {
        public MovingMinutia(TemplateBuilderViewModel outer, StateManager stateMgr)
            : base(outer, stateMgr)
        { }

        public override void EscapeAction()
        {
            // Do nothing.
        }

        public override void MoveMinutia(int index, Point point)
        {
            IntegrityCheck.IsTrue(index > -1 && index < Outer.Minutae.Count());
            IntegrityCheck.IsNotNull(point);

            // Set position TO SCALE
            Outer.Minutae[index].Location = point.InvScale(Outer.Scale);
        }

        public override void PositionInput(Point point)
        {
            // Do nothing.
        }

        public override void PositionMove(Point point)
        {
            // Do nothing.
        }

        public override void RemoveMinutia(int index)
        {
            // Do nothing.
        }

        public override void SaveTemplate()
        {
            // Do nothing.
        }

        public override void SetMinutiaType(MinutiaType type)
        {
            // Do nothing.
        }

        public override void StartMove()
        {
            // TODO: Resolve issue that makes this stick and reinstate integrity check
            // throw IntegrityCheck.Fail("Unexpected StartMove() call in {0} state.", GetType().Name);
            // For some reason a fast drag misses the MouseUp event and we get stuck in
            // MovingMinutia, as a result we should interpret this start move as a StopMove.
            StopMove();
        }

        public override void StopMove()
        {
            StateMgr.TransitionTo(typeof(WaitLocation));
        }
    }
}
