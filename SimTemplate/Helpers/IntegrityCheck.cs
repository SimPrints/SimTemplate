using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimTemplate.Helpers
{
    public static class IntegrityCheck
    {
        public static readonly ILog m_Log = LogManager.GetLogger(typeof(IntegrityCheck));

        public static TemplateBuilderException Fail(string message)
        {
            m_Log.Error(message);
            return new TemplateBuilderException(message);
        }

        public static TemplateBuilderException Fail(string format, params string[] args)
        {
            return new TemplateBuilderException(String.Format(format, args));
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

        public static TemplateBuilderException FailUnexpectedDefault<T>(T value)
        {
            return new TemplateBuilderException(
                String.Format("Unexpected default value: {0}", value));
        }

        #endregion
    }
}
