using FakeItEasy;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SimTemplate.DataTypes;
using SimTemplate.DataTypes.Enums;
using SimTemplate.Utilities;
using SimTemplate.ViewModels.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SimTemplate.ViewModels.Test
{
    [TestClass]
    public class TemplatingViewModelTest
    {
        #region Constants and Statics

        private static readonly byte[] IMAGE_DATA = new byte[] { 0, 1, 2, 3, 4 }; // TODO: Improve this
        private const string TEMPLATE_1_HEX = "464D5200203230000000003C0000012C019000C500C5010000105B054087000B660080B5003B6700407100176C00407600346D0080A0004BE2000000";

        private const int MAX_IMAGE_SIZE = 1000;

        private const string INPUT_MINUTIA_PROMPT = "Please place minutia";
        private const string INPUT_ANGLE_PROMPT = "Please set minutia angle";

        #region Captures

        private static readonly CaptureInfo CAPTURE_NO_TEMPLATE = new CaptureInfo(
            1,
            IMAGE_DATA,
            null);

        private static readonly CaptureInfo CAPTURE_WITH_TEMPLATE = new CaptureInfo(
            1,
            new byte[] { 0 },
            IsoTemplateHelper.ToByteArray(TEMPLATE_1_HEX));

        private static readonly CaptureInfo CAPTURE_NULL_IMAGE = new CaptureInfo(
            1,
            null,
            null);

        private static readonly CaptureInfo CAPTURE_INVALID_IMAGE = new CaptureInfo(
            1,
            new byte[] { 0 },
            null);

        #endregion

        private const MinutiaType DEFAULT_MINUTIA_TYPE = MinutiaType.Termination;

        #endregion

        // Fakes
        private IDispatcherHelper m_DispatcherHelper;

        // Test class
        private TemplatingViewModel m_ViewModel;
        private ITemplatingViewModel m_IViewModel;

        // Supporting members
        private readonly Random m_Random = new Random();
        private EventMonitor m_Monitor;

        [TestInitialize]
        public void Setup()
        {
            m_DispatcherHelper = A.Fake<IDispatcherHelper>();

            m_ViewModel = new TemplatingViewModel(m_DispatcherHelper);
            m_IViewModel = (ITemplatingViewModel)m_ViewModel;
            m_Monitor = new EventMonitor();
            m_Monitor.AddMonitoredObject(m_ViewModel);
        }

        [TestMethod]
        public void TestInitialise()
        {
            DoTestInitialise();
        }

        [TestMethod]
        public void TestAddMinutia()
        {
            // First initialise
            DoTestInitialise();

            // Then start templating
            DoTestBeginTemplating();

            // Add a minutia
            DoTestAddMinutia();

            // Update angle of minutia
            DoTestUpdateMinutia();

            // Save angle of minutia
            DoTestSetMinutia();
        }

        [TestMethod]
        public void TestCancelMinutia()
        {
            // First initialise
            DoTestInitialise();

            // Then start templating
            DoTestBeginTemplating();

            // Add a minutia
            DoTestAddMinutia();

            // Cancel the minutia
            DoTestCancelMinutia();
        }

        [TestMethod]
        public void TestRemoveMinutia()
        {
            // First initialise
            DoTestInitialise();

            // Then start templating
            DoTestBeginTemplating();

            // Add a few minitae
            int numberToAdd = m_Random.Next(1, 10);
            for (int i = 0; i < numberToAdd; i++)
            {
                // Add a minutia
                DoTestAddMinutia();

                // Update angle of minutia
                DoTestUpdateMinutia();

                // Set the minutia
                DoTestSetMinutia();
            }

            // Remove a minutia
            DoTestRemoveMinutia(m_Random.Next(numberToAdd));

            // Finalise the template
            DoTestFinaliseTemplate();
        }

        // TODO: Test move minutia

        #region Do Test Routines

        private void DoTestBeginTemplating()
        {
            using (Fake.CreateScope())
            {
                // EXECUTE
                // TODO: IntegrityCheck on valid image?
                m_IViewModel.BeginTemplating(CAPTURE_NO_TEMPLATE);

                // ASSERT
                Assert.AreEqual(CAPTURE_NO_TEMPLATE, m_ViewModel.Capture);
                Assert.AreEqual(DEFAULT_MINUTIA_TYPE, m_ViewModel.InputMinutiaType);
                Assert.AreEqual(0, m_ViewModel.Minutae.Count);
                Assert.IsFalse(m_IViewModel.IsSaveTemplatePermitted);
                IEnumerable<UserActionRequiredEventArgs> userActionEvents = m_Monitor.GetEventResponses<UserActionRequiredEventArgs>("UserActionRequired");
                Assert.AreEqual(1, userActionEvents.Count());
                Assert.AreEqual(INPUT_MINUTIA_PROMPT, userActionEvents.ElementAt(0).PromptText);
                IEnumerable<PropertyChangedEventArgs> propertyChangedEvents = m_Monitor.GetEventResponses<PropertyChangedEventArgs>("PropertyChanged");
                Assert.AreEqual(1, propertyChangedEvents.Count());
                Assert.AreEqual("Capture", propertyChangedEvents.ElementAt(0).PropertyName);

                // Assert IDispatcherHelper interaction
                // Minutia are cleared upon starting templating with a new capture
                A.CallTo(() => m_DispatcherHelper.Invoke(A<Action>._)).MustHaveHappened(Repeated.Exactly.Once);
            }
        }

        private void DoTestInitialise()
        {
            using (Fake.CreateScope())
            {
                // EXECUTE
                m_IViewModel.BeginInitialise();

                // ASSERT
                Assert.IsNull(m_ViewModel.Capture);
                Assert.AreEqual(DEFAULT_MINUTIA_TYPE, m_ViewModel.InputMinutiaType);
                Assert.AreEqual(0, m_ViewModel.Minutae.Count);
                Assert.IsFalse(m_IViewModel.IsSaveTemplatePermitted);
                Assert.AreEqual(0, m_Monitor.GetEventResponses<UserActionRequiredEventArgs>("UserActionRequired").Count());
                Assert.AreEqual(0, m_Monitor.GetEventResponses<PropertyChangedEventArgs>("PropertyChanged").Count());

                // Assert IDispatcherHelper interaction
                AssertNoCallsToDispatcherHelper();
            }
        }

        private void DoTestCancelMinutia()
        {
            using (Fake.CreateScope())
            {
                // PREPARE:
                m_Monitor.Reset();
                // Copy the minutae before the operation
                IEnumerable<MinutiaRecord> minutae = m_ViewModel.Minutae.ToArray();
                // Record the input minutia type before the operation
                MinutiaType inputType = m_ViewModel.InputMinutiaType;

                // EXECUTE:
                m_IViewModel.EscapeAction();

                // ASSERT:
                Assert.AreEqual(CAPTURE_NO_TEMPLATE, m_ViewModel.Capture);
                Assert.AreEqual(inputType, m_ViewModel.InputMinutiaType);
                Assert.AreEqual(m_ViewModel.Minutae.Count() > 0, m_IViewModel.IsSaveTemplatePermitted);

                // Assert that the user is prompted to add a minutia
                IEnumerable<UserActionRequiredEventArgs> userActionEvents = m_Monitor.GetEventResponses<UserActionRequiredEventArgs>("UserActionRequired");
                Assert.AreEqual(1, userActionEvents.Count());
                Assert.AreEqual(INPUT_MINUTIA_PROMPT, userActionEvents.ElementAt(0).PromptText);

                Assert.AreEqual(0, m_Monitor.GetEventResponses<PropertyChangedEventArgs>("PropertyChanged").Count());
                // Assert previous minuae are unchanged (note Equals has been overriden)
                Assert.AreEqual(minutae.Count() - 1, m_ViewModel.Minutae.Count());
                CollectionAssert.AreEqual(
                    minutae.Take(minutae.Count() - 1).ToArray(),
                    m_ViewModel.Minutae.ToArray());

                // Assert IDispatcherHelper interaction
                AssertNoCallsToDispatcherHelper();
            }
        }

        private void DoTestUpdateMinutia()
        {
            using (Fake.CreateScope())
            {
                // PREPARE:
                m_Monitor.Reset();
                // Create a random point to update the minutia with
                Point updatePosition = new Point(
                    m_Random.Next(MAX_IMAGE_SIZE),
                    m_Random.Next(MAX_IMAGE_SIZE));
                // Copy the minutae before the operation
                IEnumerable<MinutiaRecord> minutae = m_ViewModel.Minutae.ToArray();
                // Record the input minutia type before the operation
                MinutiaType inputType = m_ViewModel.InputMinutiaType;

                // EXECUTE:
                m_ViewModel.PositionUpdate(updatePosition);

                // ASSERT:
                Assert.AreEqual(CAPTURE_NO_TEMPLATE, m_ViewModel.Capture);
                Assert.AreEqual(inputType, m_ViewModel.InputMinutiaType);
                Assert.AreEqual(minutae.Count(), m_ViewModel.Minutae.Count);
                Assert.IsFalse(m_IViewModel.IsSaveTemplatePermitted);
                Assert.AreEqual(0, m_Monitor.GetEventResponses<UserActionRequiredEventArgs>("UserActionRequired").Count());
                Assert.AreEqual(0, m_Monitor.GetEventResponses<PropertyChangedEventArgs>("PropertyChanged").Count());
                // Assert previous minuae are unchanged (note Equals has been overriden)
                Assert.AreEqual(minutae.Count(), m_ViewModel.Minutae.Count());
                CollectionAssert.AreEqual(
                    minutae.Take(minutae.Count() - 1).ToArray(),
                    m_ViewModel.Minutae.Take(minutae.Count() - 1).ToArray());
                // Assert updated minutia properties
                Assert.AreEqual(minutae.Last().Position, m_ViewModel.Minutae.Last().Position);
                Assert.AreEqual(minutae.Last().Type, m_ViewModel.Minutae.Last().Type);
                Assert.AreEqual(inputType, m_ViewModel.Minutae.Last().Type);

                Vector direction = (updatePosition - minutae.Last().Position);
                Assert.AreEqual(
                    MathsHelper.RadianToDegree(Math.Atan2(direction.Y, direction.X)),
                    m_ViewModel.Minutae.Last().Angle);

                // Assert IDispatcherHelper interaction
                AssertNoCallsToDispatcherHelper();
            }
        }

        private void DoTestSetMinutia()
        {
            using (Fake.CreateScope())
            {
                // PREPARE:
                m_Monitor.Reset();
                // Create a random point to update the minutia with
                Point setPosition = new Point(
                    m_Random.Next(MAX_IMAGE_SIZE),
                    m_Random.Next(MAX_IMAGE_SIZE));
                // Copy the minutae before the operation
                IEnumerable<MinutiaRecord> minutae = m_ViewModel.Minutae.ToArray();
                // Record the input minutia type before the operation
                MinutiaType inputType = m_ViewModel.InputMinutiaType;

                // EXECUTE:
                m_ViewModel.PositionInput(setPosition);

                // ASSERT:
                Assert.AreEqual(CAPTURE_NO_TEMPLATE, m_ViewModel.Capture);
                Assert.AreEqual(inputType, m_ViewModel.InputMinutiaType);
                Assert.AreEqual(minutae.Count(), m_ViewModel.Minutae.Count);
                Assert.AreEqual((m_ViewModel.Minutae.Count() > 0), m_IViewModel.IsSaveTemplatePermitted);

                // Assert that the user is prompted to add a new minutia
                IEnumerable<UserActionRequiredEventArgs> userActionEvents = m_Monitor.GetEventResponses<UserActionRequiredEventArgs>("UserActionRequired");
                Assert.AreEqual(1, userActionEvents.Count());
                Assert.AreEqual(INPUT_MINUTIA_PROMPT, userActionEvents.ElementAt(0).PromptText);

                // Assert that no bound properties were changed
                Assert.AreEqual(0, m_Monitor.GetEventResponses<PropertyChangedEventArgs>("PropertyChanged").Count());

                // Assert previous minuae are unchanged (note Equals has been overridden)
                Assert.AreEqual(minutae.Count(), m_ViewModel.Minutae.Count());
                CollectionAssert.AreEqual(
                    minutae.Take(minutae.Count() - 1).ToArray(),
                    m_ViewModel.Minutae.Take(minutae.Count() - 1).ToArray());
                // Assert set minutia properties
                Assert.AreEqual(minutae.Last().Position, m_ViewModel.Minutae.Last().Position);
                Assert.AreEqual(minutae.Last().Type, m_ViewModel.Minutae.Last().Type);
                Assert.AreEqual(inputType, m_ViewModel.Minutae.Last().Type);

                Vector direction = (setPosition - minutae.Last().Position);
                Assert.AreEqual(
                    MathsHelper.RadianToDegree(Math.Atan2(direction.Y, direction.X)),
                    m_ViewModel.Minutae.Last().Angle);

                // Assert IDispatcherHelper interaction
                AssertNoCallsToDispatcherHelper();
            }
        }

        private void DoTestAddMinutia()
        {
            using(Fake.CreateScope())
            {
                // PREPARE
                m_Monitor.Reset();
                // Create a random minutia position
                Point newMinutiaPosition = new Point(
                    m_Random.Next(MAX_IMAGE_SIZE),
                    m_Random.Next(MAX_IMAGE_SIZE));
                // Copy the minutae before the operation
                IEnumerable<MinutiaRecord> minutae = m_ViewModel.Minutae.ToArray();
                // Record the input minutia type before the operation
                MinutiaType inputType = m_ViewModel.InputMinutiaType;

                // EXECUTE
                // TODO: Should there be a check on position vs. image size?
                m_ViewModel.PositionInput(newMinutiaPosition);

                // ASSERT
                Assert.AreEqual(CAPTURE_NO_TEMPLATE, m_ViewModel.Capture);
                Assert.AreEqual(inputType, m_ViewModel.InputMinutiaType);
                Assert.IsFalse(m_IViewModel.IsSaveTemplatePermitted);
                // UserActionRequired prompting to input angle
                IEnumerable<UserActionRequiredEventArgs> userActionEvents = m_Monitor.GetEventResponses<UserActionRequiredEventArgs>("UserActionRequired");
                Assert.AreEqual(1, userActionEvents.Count());
                Assert.AreEqual(INPUT_ANGLE_PROMPT, userActionEvents.ElementAt(0).PromptText);
                Assert.AreEqual(0, m_Monitor.GetEventResponses<PropertyChangedEventArgs>("PropertyChanged").Count());
                // Assert previous minuae are unchanged (note Equals has been overriden)
                CollectionAssert.AreEqual(minutae.ToArray(), m_ViewModel.Minutae.Take(minutae.Count()).ToArray());
                // Assert new minutia properties
                Assert.AreEqual(minutae.Count() + 1, m_ViewModel.Minutae.Count());
                Assert.AreEqual(newMinutiaPosition, m_ViewModel.Minutae.Last().Position);
                Assert.AreEqual(inputType, m_ViewModel.Minutae.Last().Type);
                Assert.AreEqual(0, m_ViewModel.Minutae.Last().Angle);

                // Assert IDispatcherHelper interaction
                AssertNoCallsToDispatcherHelper();
            }
        }

        private void DoTestRemoveMinutia(int index)
        {
            using (Fake.CreateScope())
            {
                // PREPARE:
                m_Monitor.Reset();
                IEnumerable<MinutiaRecord> minutae = m_ViewModel.Minutae.ToArray();
                // Record the input minutia type before the operation
                MinutiaType inputType = m_ViewModel.InputMinutiaType;

                // EXECUTE:
                m_ViewModel.RemoveMinutia(index);

                // ASSERT:
                Assert.AreEqual(CAPTURE_NO_TEMPLATE, m_ViewModel.Capture);
                Assert.AreEqual(inputType, m_ViewModel.InputMinutiaType);
                Assert.AreEqual(m_ViewModel.Minutae.Count() > 0, m_IViewModel.IsSaveTemplatePermitted);
                Assert.AreEqual(0, m_Monitor.GetEventResponses<UserActionRequiredEventArgs>("UserActionRequired").Count());
                Assert.AreEqual(0, m_Monitor.GetEventResponses<PropertyChangedEventArgs>("PropertyChanged").Count());

                // Assert previous minuae are unchanged (note Equals has been overriden)
                Assert.AreEqual(minutae.Count() - 1, m_ViewModel.Minutae.Count());
                CollectionAssert.AreEqual(
                    minutae.Where((minutia, i) => i != index).ToArray(),
                    m_ViewModel.Minutae.ToArray());

                // Assert IDispatcherHelper interaction
                AssertNoCallsToDispatcherHelper();
            }
        }

        private void DoTestFinaliseTemplate()
        {
            using (Fake.CreateScope())
            {
                // PREPARE
                m_Monitor.Reset();
                IEnumerable<MinutiaRecord> minutae = m_ViewModel.Minutae.ToArray();
                // Record the input minutia type before the operation
                MinutiaType inputType = m_ViewModel.InputMinutiaType;

                // EXECUTE:
                byte[] template = m_IViewModel.FinaliseTemplate();

                // ASSERT:
                // First check that the template is correctly converted
                CollectionAssert.AreEqual(
                    IsoTemplateHelper.ToIsoTemplate(minutae),
                    template);
                // Check the workspace is cleared
                Assert.AreEqual(CAPTURE_NO_TEMPLATE, m_ViewModel.Capture);
                Assert.IsTrue(m_IViewModel.IsSaveTemplatePermitted);
                Assert.AreEqual(minutae.Count(), m_ViewModel.Minutae.Count());
                Assert.AreEqual(0, m_Monitor.GetEventResponses<UserActionRequiredEventArgs>("UserActionRequired").Count());
                Assert.AreEqual(0, m_Monitor.GetEventResponses<PropertyChangedEventArgs>("PropertyChanged").Count());
                Assert.AreEqual(inputType, m_ViewModel.InputMinutiaType);
            }
        }

        #endregion

        #region Assertion Methods

        private void AssertNoCallsToDispatcherHelper()
        {
            A.CallTo(() => m_DispatcherHelper.Invoke(A<Action>._)).MustNotHaveHappened();
        }

        #endregion
    }
}
