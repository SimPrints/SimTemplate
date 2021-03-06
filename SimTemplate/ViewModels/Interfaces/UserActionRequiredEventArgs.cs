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
using System;

namespace SimTemplate.ViewModels.Interfaces
{
	public class UserActionRequiredEventArgs : EventArgs
	{
		private readonly string m_PromptText;

        /// <summary>
        /// Gets the text to promt the user to perform the action.
        /// </summary>
        /// <value>
        /// The prompt text.
        /// </value>
        public string PromptText { get { return m_PromptText; } }

		public UserActionRequiredEventArgs(string promptText)
		{
			m_PromptText = promptText;
		}
	}
}