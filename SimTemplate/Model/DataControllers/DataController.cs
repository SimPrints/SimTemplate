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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SimTemplate.Utilities;
using SimTemplate.Model.DataControllers.EventArguments;
using SimTemplate.DataTypes.Enums;

namespace SimTemplate.Model.DataControllers
{
    public abstract class DataController : LoggingClass, IDataController
    {
        private DataControllerConfig m_Config;
        private IDictionary<Guid, CancellationTokenSource> m_TokenSourceLookup;

        private event EventHandler<InitialisationCompleteEventArgs> m_InitialisationComplete;
        private event EventHandler<GetCaptureCompleteEventArgs> m_GetCaptureComplete;
        private event EventHandler<SaveTemplateEventArgs> m_SaveTemplateComplete;

        #region IDataController

        public DataController()
        {
            m_TokenSourceLookup = new Dictionary<Guid, CancellationTokenSource>();
        }

        Guid IDataController.BeginInitialise(DataControllerConfig config)
        {
            Log.Debug("BeginInitialise(...) called.");
            IntegrityCheck.IsNotNull(config, "config");
            IntegrityCheck.IsNotNullOrEmpty(config.ApiKey, "config.ApiKey");
            IntegrityCheck.IsNotNullOrEmpty(config.UrlRoot, "config.UrlRoot");

            m_Config = config;
            m_TokenSourceLookup.Clear();

            return StartLogic((Guid guid, CancellationToken token) =>
                StartInitialiseTask(config, guid, token));
        }

        Guid IDataController.BeginGetCapture(ScannerType scannerType)
        {
            Log.DebugFormat("BeginGetCapture(scannterType={0}) called",
                scannerType);
            IntegrityCheck.IsNotNull(scannerType);

            return StartLogic((Guid guid, CancellationToken token) =>
                StartCaptureTask(scannerType, guid, token));
        }

        Guid IDataController.BeginSaveTemplate(long dbId, byte[] template)
        {
            Log.DebugFormat("BeginGetCapture(dbId={0}, template={1}) called",
                dbId, template);

            return StartLogic((Guid guid, CancellationToken token) =>
                StartSaveTask(dbId, template, guid, token));
        }

        void IDataController.AbortRequest(Guid guid)
        {
            Log.DebugFormat("AbortRequest(guid={0}) called.", guid);
            IntegrityCheck.IsNotNull(guid);

            // Attempt to lookup the token.
            CancellationTokenSource tokenSource;
            bool isSuccessful = m_TokenSourceLookup.TryGetValue(guid, out tokenSource);

            if (isSuccessful)
            {
                // Request cancellation.
                IntegrityCheck.IsNotNull(tokenSource);
                tokenSource.Cancel();
                m_TokenSourceLookup.Remove(guid);
            }
            else
            {
                // TODO: What to do if Guid doesn't correspond to a current request?
                Log.WarnFormat("Cancellation of request (guid={0}) failed, token no longer exists.",
                    guid);
            }
        }

        event EventHandler<InitialisationCompleteEventArgs> IDataController.InitialisationComplete
        {
            add { m_InitialisationComplete += value; }
            remove { m_InitialisationComplete -= value; }
        }

        event EventHandler<GetCaptureCompleteEventArgs> IDataController.GetCaptureComplete
        {
            add { m_GetCaptureComplete += value; }
            remove { m_GetCaptureComplete -= value; }
        }

        event EventHandler<SaveTemplateEventArgs> IDataController.SaveTemplateComplete
        {
            add { m_SaveTemplateComplete += value; }
            remove { m_SaveTemplateComplete -= value; }
        }

        #endregion

        #region Event Helpers

        protected void OnInitialisationComplete(InitialisationCompleteEventArgs e)
        {
            EventHandler<InitialisationCompleteEventArgs> temp = m_InitialisationComplete;
            if (temp != null)
            {
                temp.Invoke(this, e);
            }
        }

        protected void OnGetCaptureComplete(GetCaptureCompleteEventArgs e)
        {
            EventHandler<GetCaptureCompleteEventArgs> temp = m_GetCaptureComplete;
            if (temp != null)
            {
                temp.Invoke(this, e);
            }
        }

        protected void OnSaveTemplateComplete(SaveTemplateEventArgs e)
        {
            EventHandler<SaveTemplateEventArgs> temp = m_SaveTemplateComplete;
            if (temp != null)
            {
                temp.Invoke(this, e);
            }
        }

        #endregion

        #region Protected Members

        protected DataControllerConfig Config { get { return m_Config; } }

        protected abstract void StartInitialiseTask(DataControllerConfig config,
            Guid guid, CancellationToken token);

        protected abstract void StartCaptureTask(ScannerType scannerType,
            Guid guid, CancellationToken token);

        protected abstract void StartSaveTask(long dbId, byte[] template,
            Guid guid, CancellationToken token);

        #endregion

        #region Private Methods

        private Guid StartLogic(Action<Guid, CancellationToken> action)
        {
            // Generate a GUID and TokenSource for the request, and store them for lookup later.
            Guid guid = Guid.NewGuid();
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            m_TokenSourceLookup.Add(guid, cancellationTokenSource);

            // Create a cancellation token for notifying of a cancellation request.
            CancellationToken token = cancellationTokenSource.Token;

            // Run the required behaviour
            action.Invoke(guid, token);

            return guid;
        }

        #endregion
    }
}
