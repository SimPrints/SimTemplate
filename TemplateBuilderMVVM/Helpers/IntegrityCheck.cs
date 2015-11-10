using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TemplateBuilderMVVM.Helpers
{
    public static class IntegrityCheck
    {
        public static TemplateBuilderException Fail(string message)
        {
            return new TemplateBuilderException(message);
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
    }
}
