using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media.Imaging;
using TemplateBuilder.Helpers;
using TemplateBuilder.Model.Database;

namespace TemplateBuilder.ViewModel.MainWindow.States
{
    abstract public class Initialised : State
    {
        #region Constants

        private const int MAX_INVALID_FILES = 10;

        #endregion

        public Initialised(TemplateBuilderViewModel outer, StateManager stateMgr) : base(outer, stateMgr)
        { }

        //IEnumerable<string> m_ValidFileExtensions = new List<string>()
        //{
        //    "jpg",
        //    "jpeg",
        //    "bmp",
        //    "png",
        //};

        #region Overriden Methods

        public override void OpenFile()
        {

        }

        public override void image_SizeChanged(Size newSize)
        {
            if (Outer.Image != null)
            {
                // Image has been resized.
                // Get scaling in each dimension.
                double scaleX = newSize.Width / Outer.Image.Width;
                double scaleY = newSize.Height / Outer.Image.Height;
                // Check that scaling factor is equal for each dimension.
                Vector scale = new Vector(scaleX, scaleY);
                // Update ViewModel scale
                Outer.Scale = scale;
            }
            else
            {
                // Image has been removed.
                IntegrityCheck.AreEqual(0, newSize.Height);
                IntegrityCheck.AreEqual(0, newSize.Width);
            }
        }

        public override void DataController_InitialisationComplete(InitialisationCompleteEventArgs e)
        {
            throw IntegrityCheck.Fail("Not expected to have InitialisationComplete event when initialised.");
        }

        #endregion

        #region Protected Methods

        protected void LoadImage()
        {
            OpenImageResult openImageStatus = OpenImageResult.Running;
            int attempts = 0;
            while (openImageStatus == OpenImageResult.Running)
            {
                attempts++;
                if (attempts > MAX_INVALID_FILES)
                {
                    openImageStatus = OpenImageResult.InvalidFileCountExceeded;
                }

                // Try to get a file from the database
                string imageFilename = Outer.DataController.GetImageFile();

                if (imageFilename != null)
                {
                    // A file was found.
                    Logger.DebugFormat("An image file was found for image: {0}.", imageFilename);
                    BitmapImage image = null;
                    bool isOpenedSuccessfully = false;
                    try
                    {
                        image = new BitmapImage(new Uri(imageFilename, UriKind.Absolute));
                        isOpenedSuccessfully = true;
                    }
                    catch (NotSupportedException ex)
                    {
                        Logger.WarnFormat("Failed to open image file: {0}\n" + ex, imageFilename);
                    }

                    if (isOpenedSuccessfully)
                    {
                        Outer.Image = image;
                        openImageStatus = OpenImageResult.Successful;
                        StateMgr.TransitionTo(typeof(WaitLocation));
                    }
                }
                else
                {
                    // No file was found.
                    Logger.DebugFormat("No image file was found for {0}", imageFilename);
                    openImageStatus = OpenImageResult.Failed;
                    // TODO: Reinitialise?
                    //m_StateMgr.TransitionTo(typeof(Idle))
                }
            }

            switch (openImageStatus)
            {
                case OpenImageResult.Successful:
                    StateMgr.TransitionTo(typeof(WaitLocation));
                    break;

                case OpenImageResult.Failed:
                case OpenImageResult.InvalidFileCountExceeded:
                    StateMgr.TransitionTo(typeof(Error));
                    break;

                default:
                    throw IntegrityCheck.FailUnexpectedDefault(openImageStatus);
            }
        }

        #endregion
    }
}
