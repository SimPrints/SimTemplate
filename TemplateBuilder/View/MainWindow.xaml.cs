using log4net;
using log4net.Config;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using TemplateBuilder.Helpers;
using TemplateBuilder.Model.Database;
using TemplateBuilder.ViewModel.MainWindow;

namespace TemplateBuilder.View.MainWindow
{
    /// <summary>
    /// Interaction logic for MainWindowView.xaml
    /// </summary>
    public partial class MainWindowView : Window, INotifyPropertyChanged
    {
        private static readonly ILog m_Log = LogManager.GetLogger(typeof(MainWindowView));
        private readonly TemplateBuilderViewModel m_ViewModel;
        private Vector m_Scale;

        #region Constructor

        public MainWindowView()
        {
            XmlConfigurator.Configure();
            m_Log.Debug("Logging initialised.");
            m_ViewModel = new TemplateBuilderViewModel(new DataController());

            InitializeComponent();
            DataContext = m_ViewModel;

            m_ViewModel.Start();
        }

        #endregion

        public TemplateBuilderViewModel ViewModel { get { return m_ViewModel; } }

        public Vector Scale
        {
            get { return m_Scale; }
            set
            {
                if (value != m_Scale)
                {
                    m_Scale = value;
                    NotifyPropertyChanged();
                }
            }
        }

        #region Event Handlers

        private void itemsControl_MouseMove(object sender, MouseEventArgs e)
        {
            Point scaled_pos = e.GetPosition(image);
            // Account for image scaling
            Point pos = scaled_pos.InvScale(m_Scale);
            // Pass the pixels of the image 
            m_ViewModel.MouseMove(pos);
        }

        private void itemsControl_MouseUp(object sender, MouseButtonEventArgs e)
        {
            m_Log.Debug("itemsControl_MouseUp(...) called.");

            Point scaled_pos = e.GetPosition(image);
            // Account for image scaling
            Point pos = scaled_pos.InvScale(m_Scale);
            // Pass the pixels of the image 
            m_ViewModel.itemsControl_MouseUp(pos, e.ChangedButton);
        }

        private void image_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            m_Log.Debug("image_SizeChanged(...) called.");

            if (image.Source != null)
            {
                // Image has been resized.
                // Get scaling in each dimension.
                double scaleX = e.NewSize.Width / image.Source.Width;
                double scaleY = e.NewSize.Height / image.Source.Width;
                // Check that scaling factor is equal for each dimension.
                Scale = new Vector(scaleX, scaleY);
            }
            else
            {
                // Image has been removed.
                IntegrityCheck.AreEqual(0, e.NewSize.Height);
                IntegrityCheck.AreEqual(0, e.NewSize.Width);
            }
        }

        private void Minutia_MouseDown(object sender, MouseButtonEventArgs e)
        {
            m_Log.Debug("Minutia_MouseDown(...) called.");

            if (e.ChangedButton == MouseButton.Left)
            {
                object item = (sender as FrameworkElement).DataContext;
                int index = itemsControl.Items.IndexOf(item);
                m_ViewModel.StartMove(index);
            }
        }

        private void Minutia_MouseUp(object sender, MouseButtonEventArgs e)
        {
            m_Log.Debug("Minutia_MouseUp(...) called.");

            if (e.ChangedButton == MouseButton.Right)
            {
                object item = (sender as FrameworkElement).DataContext;
                int index = itemsControl.Items.IndexOf(item);

                m_ViewModel.Minutia_MouseUp(index);

                // Mark event as handled so that we don't create a new minutia as soon as we have
                // deleted one.
                e.Handled = true;
            }
        }

        private void Minutia_MouseMove(object sender, MouseEventArgs e)
        {
            Point scaled_pos = e.GetPosition(image);
            // Account for image scaling
            Point pos = scaled_pos.InvScale(m_Scale);
            // Pass the pixels of the image 
            m_ViewModel.MoveMinutia(pos);
        }

        #endregion

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        // This method is called by the Set accessor of each property.
        // The CallerMemberName attribute that is applied to the optional propertyName
        // parameter causes the property name of the caller to be substituted as an argument.
        protected void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion
    }
}
