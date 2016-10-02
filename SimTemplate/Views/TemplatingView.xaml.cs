// Copyright 2016 Sam Briggs
//
// This file is part of SimTemplate.
//
// SimTemplate is free software: you can redistribute it and/or modify it under the
// terms of the GNU General Public License as published by the Free Software 
// Foundation, either version 3 of the License, or (at your option) any later
// version.
//
// SimTemplate is distributed in the hope that it will be useful, but WITHOUT ANY 
// WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR 
// A PARTICULAR PURPOSE. See the GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License along with
// SimTemplate. If not, see http://www.gnu.org/licenses/.
//
using log4net;
using SimTemplate.Utilities;
using SimTemplate.ViewModels;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace SimTemplate.Views
{
    /// <summary>
    /// Interaction logic for TemplatingView.xaml
    /// </summary>
    public partial class TemplatingView
    {
        private static readonly ILog m_Log = LogManager.GetLogger(typeof(TemplatingView));

        private TemplatingViewModel m_ViewModel;

        public TemplatingView()
        {
            InitializeComponent();
            DataContextChanged += TemplatingView_DataContextChanged;
        }

        private Vector Scale { get; set; }

        #region Event Handlers

        private void TemplatingView_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            m_ViewModel = e.NewValue as TemplatingViewModel;
            //if (m_ViewModel != null)
            //{
            //    // ViewModel has just been set as context to a view
            //    m_ViewModel.NotifyAllPropertiesChanged();
            //}
        }

        private void templatingCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            Point scaled_pos = e.GetPosition(sender as IInputElement);
            // Account for image scaling
            Point pos = scaled_pos.InvScale(Scale);
            // Pass the pixels of the image 
            m_ViewModel.PositionUpdate(pos);
        }

        private void templatingCanvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            m_Log.Debug("templatingCanvas_MouseUp(...) called.");

            Point scaled_pos = e.GetPosition(sender as IInputElement);
            // Account for image scaling
            Point pos = scaled_pos.InvScale(Scale);
            // Pass the pixels of the image 
            m_ViewModel.PositionInput(pos);
        }

        private void image_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            m_Log.Debug("image_SizeChanged(...) called.");

            if (image.Source != null)
            {
                // Image has been resized.
                // Get scaling in each dimension.
                double scaleX = e.NewSize.Width / image.Source.Width;
                double scaleY = e.NewSize.Height / image.Source.Height;
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
                int index = templatingItemsControl.Items.IndexOf(item);
                m_ViewModel.StartMove(index);
            }
        }

        private void Minutia_MouseUp(object sender, MouseButtonEventArgs e)
        {
            m_Log.Debug("Minutia_MouseUp(...) called.");

            if (e.ChangedButton == MouseButton.Right)
            {
                object item = (sender as FrameworkElement).DataContext;
                int index = templatingItemsControl.Items.IndexOf(item);

                m_ViewModel.RemoveMinutia(index);

                // Mark event as handled so that we don't create a new minutia as soon as we have
                // deleted one.
                e.Handled = true;
            }
        }

        private void Minutia_MouseMove(object sender, MouseEventArgs e)
        {
            Point scaled_pos = e.GetPosition(image);
            // Account for image scaling
            Point pos = scaled_pos.InvScale(Scale);
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
