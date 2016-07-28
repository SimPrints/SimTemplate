using System;

namespace SimTemplate.ViewModels
{
	public class UserActionRequiredEventArgs : EventArgs
	{
		private readonly string m_PromptText;

		public string PromptText { get { return m_PromptText; } }

		public UserActionRequiredEventArgs(string promptText)
		{
			m_PromptText = promptText;
		}
	}
}