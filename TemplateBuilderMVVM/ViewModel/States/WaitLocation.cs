using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using TemplateBuilderMVVM.Model;

namespace TemplateBuilderMVVM.ViewModel.States
{
    class WaitLocation : Templating
    {
        public WaitLocation(TemplateBuilderViewModel outer, StateManager stateMgr) : base(outer, stateMgr)
        { }

        public override void PositionInput(Point pos)
        {
            // The user is starting to record a new minutia

            // Start a new minutia data record.
            MinutiaRecord record = new MinutiaRecord();
            record.Location = new Point(pos.X, pos.Y);

            // Record minutia information.
            m_Outer.Minutae.Add(record);

            // Indicate next input defines the direction.
            m_StateMgr.TransitionTo(typeof(WaitDirection));
        }

        public override void PositionMove(Point e)
        {
            // Do nothing.
        }

        public override void RemoveItem(int index)
        {
            // Remove the item at the specified index.
            m_Outer.Minutae.RemoveAt(index);
        }

        public override void SaveTemplate()
        {
            // TODO: Integrity check that m_Outer.Filename is set

            // We are not partway through inputting a point
            // Construct a file name from the original image file name
            string filename = String.Format(
                "{0}_template.txt",
                System.IO.Path.GetFileNameWithoutExtension(m_Outer.ImageFile));
            string filepath = System.IO.Path.Combine(
                System.IO.Path.GetDirectoryName(m_Outer.ImageFile),
                filename);

            // Write the Minutia details out to a new file
            using (System.IO.StreamWriter file =
            new System.IO.StreamWriter(filepath))
            {
                foreach (MinutiaRecord minutia in m_Outer.Minutae)
                {
                    file.WriteLine(minutia.ToText());
                }
            }
        }
    }
}
