using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using TemplateBuilderMVVM.Helpers;
using TemplateBuilderMVVM.Model;

namespace TemplateBuilderMVVM.ViewModel.States
{
    public class Uninitialised : BaseState
    {
        public Uninitialised(TemplateBuilderViewModel outer, StateManager stateMgr) : base(outer, stateMgr) { }

        public override void image_SizeChanged(Size newSize)
        {
            IntegrityCheck.IsNotNull(m_Outer.Image);
            // Get scaling in each dimension.
            double scaleX = newSize.Width / m_Outer.Image.Width;
            double scaleY = newSize.Height / m_Outer.Image.Height;
            // Check that scaling factor is equal for each dimension.
            IntegrityCheck.AreEqual(scaleX, scaleY);
            double scale = (scaleX + scaleY) / 2;
            // Update ViewModel scale
            m_Outer.Scale = scale;
        }

        public override void OnEnteringState()
        {
            // TODO: grey out 'save template button'?
        }

        public override void PositionInput(Point e)
        {
            // Do nothing when no image has been loaded.
        }

        public override void PositionMove(Point e)
        {
            // Do nothing when no image has been loaded.
        }

        public override void RemoveItem(int index)
        {
            // Do nothing when no image has been loaded.
        }

        public override void SaveTemplate()
        {
            // Do nothing when no image has been loaded
        }

        public override void SetMinutiaType(MinutiaType type)
        {
            // Do nothing when no image has been loaded
        }
    }
}
