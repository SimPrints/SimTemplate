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
using System.Threading.Tasks;

namespace SimTemplate.Utilities
{
    public static class IntegrityCheck
    {
        public static readonly ILog m_Log = LogManager.GetLogger(typeof(IntegrityCheck));

        public static SimTemplateException Fail(string message)
        {
            m_Log.Error(message);
            return new SimTemplateException(message);
        }

        public static SimTemplateException Fail(string format, params string[] args)
        {
            return new SimTemplateException(String.Format(format, args));
        }

        #region IsTrue

        public static void IsTrue(bool condition)
        {
            if (!condition)
            {
                throw Fail("Not true.");
            }
        }

        public static void IsTrue(bool condition, string message)
        {
            if (!condition)
            {
                throw Fail(String.Format("Not true: {0}", message));
            }
        }

        public static void IsTrue(bool condition, string format, params object[] args)
        {
            if (!condition)
            {
                throw Fail(String.Format("Not true: {0}", String.Format(format, args)));
            }
        }

        #endregion

        #region IsFalse

        public static void IsFalse(bool condition)
        {
            IsTrue(!condition, "Is not false.");
        }

        public static void IsFalse(bool condition, string message)
        {
            IsTrue(!condition, message);
        }

        public static void IsFalse(bool condition, string format, params string[] args)
        {
            IsTrue(!condition, format, args);
        }

        #endregion

        #region AreEqual

        public static void AreEqual<T>(T expected, T actual)
        {
            if (!expected.Equals(actual))
            {
                throw Fail("Not equal.");
            }
        }

        public static void AreEqual<T>(T expected, T actual, string message)
        {
            if (!expected.Equals(actual))
            {
                throw Fail(String.Format("{0} not equal to {1}: {2}",
                    actual, expected, message));
            }
        }

        public static void AreEqual<T>(T expected, T actual, string format, params object[] args)
        {
            if (!expected.Equals(actual))
            {
                throw Fail(String.Format("{0} not equal to {1}: {2}",
                    actual, expected, String.Format(format, args)));
            }
        }

        #endregion

        #region AreNotEqual

        public static void AreNotEqual<T>(T expected, T actual)
        {
            if (expected.Equals(actual))
            {
                throw Fail("Equal.");
            }
        }

        public static void AreNotEqual<T>(T expected, T actual, string message)
        {
            if (expected.Equals(actual))
            {
                throw Fail(String.Format("{0} equal to {1}: {2}",
                    actual, expected, message));
            }
        }

        public static void AreNotEqual<T>(T expected, T actual, string format, params object[] args)
        {
            if (expected.Equals(actual))
            {
                throw Fail(String.Format("{0} equal to {1}: {2}",
                    actual, expected, String.Format(format, args)));
            }
        }

        #endregion

        #region IsNull

        public static void IsNull(object value)
        {
            if (value != null)
            {
                throw Fail("Is not null.");
            }
        }

        public static void IsNull(object value, string message)
        {
            if (value != null)
            {
                throw Fail(String.Format("Is not null: {0}", message));
            }
        }

        public static void IsNull(object value, string format, params object[] args)
        {
            if (value != null)
            {
                throw Fail(String.Format("Is not null: {0}", String.Format(format, args)));
            }
        }

        #endregion

        #region IsNotNull

        public static void IsNotNull(object value)
        {
            if (value == null)
            {
                throw Fail("Value cannot be null.");
            }
        }

        public static void IsNotNull(object value, string message)
        {
            if (value == null)
            {
                throw Fail(String.Format("Value cannot be null: {0}", message));
            }
        }

        public static void IsNotNull(object value, string format, params object[] args)
        {
            if (value == null)
            {
                throw Fail(String.Format("Is not null: {0}", String.Format(format, args)));
            }
        }

        #endregion

        #region IsNotNullOrEmpty

        public static void IsNotNullOrEmpty(string value)
        {
            if (String.IsNullOrEmpty(value))
            {
                throw Fail("Is null or empty.");
            }
        }

        public static void IsNotNullOrEmpty(string value, string message)
        {
            if (String.IsNullOrEmpty(value))
            {
                throw Fail(String.Format("Is not null: {0}", message));
            }
        }

        public static void IsNotNullOrEmpty(string value, string format, params object[] args)
        {
            if (String.IsNullOrEmpty(value))
            {
                throw Fail(String.Format("Is not null: {0}", String.Format(format, args)));
            }
        }

        #endregion

        #region FailUnexpectedDefault

        public static SimTemplateException FailUnexpectedDefault<T>(T value)
        {
            return new SimTemplateException(
                String.Format("Unexpected default value: {0}", value));
        }

        #endregion
    }
}
