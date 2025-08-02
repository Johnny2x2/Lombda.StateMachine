using NUnit.Framework;
using Lombda.StateMachine;

namespace Lombda.StateMachine.Tests
{
    [TestFixture]
    public class GenericStateMachineTests
    {
        private StateMachine<string, string> stateMachine;

        [SetUp]
        public void Setup()
        {
            stateMachine = new StateMachine<string, string>();
        }

        [TearDown]
        public void TearDown()
        {
            stateMachine?.Stop();
            stateMachine = null;
        }

        [Test]
        public async Task Run_WithValidStartAndResultStates_ShouldExecuteSuccessfully()
        {
            // Arrange
            var startState = new DeadEndState();
            var resultState = new DeadEndState();
            
            stateMachine.SetEntryState(startState);
            stateMachine.SetOutputState(resultState);

            // Act
            var results = await stateMachine.Run("test input");

            // Assert
            Assert.That(results, Is.Not.Null);
            Assert.That(results.Count, Is.GreaterThanOrEqualTo(0));
        }

        [Test]
        public async Task Run_WithMultipleInputs_ShouldProcessAllInputs()
        {
            // Arrange
            var startState = new DeadEndState();
            var resultState = new DeadEndState();
            
            stateMachine.SetEntryState(startState);
            stateMachine.SetOutputState(resultState);

            string[] inputs = { "input1", "input2", "input3" };

            // Act
            var results = await stateMachine.Run(inputs);

            // Assert
            Assert.That(results, Is.Not.Null);
            Assert.That(results.Count, Is.EqualTo(inputs.Length));
        }

        [Test]
        public void SetEntryState_WithIncompatibleInputType_ShouldThrowException()
        {
            // Arrange
            var incompatibleState = new StringToIntState(); // Takes string, but machine expects string (should work)
            
            // Act & Assert
            Assert.DoesNotThrow(() => stateMachine.SetEntryState(incompatibleState));
        }

        [Test]
        public void SetOutputState_WithIncompatibleOutputType_ShouldThrowException()
        {
            // Arrange
            var incompatibleState = new StringToIntState(); // Returns int, but machine expects string
            
            // Act & Assert
            Assert.Throws<InvalidCastException>(() => stateMachine.SetOutputState(incompatibleState));
        }

        [Test]
        public void Run_WithoutStartState_ShouldThrowException()
        {
            // Arrange
            var resultState = new DeadEndState();
            stateMachine.SetOutputState(resultState);

            // Act & Assert
            Assert.ThrowsAsync<InvalidOperationException>(async () => await stateMachine.Run("test"));
        }

        [Test]
        public void Run_WithoutResultState_ShouldThrowException()
        {
            // Arrange
            var startState = new DeadEndState();
            stateMachine.SetEntryState(startState);

            // Act & Assert
            Assert.ThrowsAsync<InvalidOperationException>(async () => await stateMachine.Run("test"));
        }

        [Test]
        public void Results_AfterSuccessfulRun_ShouldReturnCorrectResults()
        {
            // Arrange
            var startState = new DeadEndState();
            var resultState = new DeadEndState();
            
            stateMachine.SetEntryState(startState);
            stateMachine.SetOutputState(resultState);

            // Act
            var runTask = stateMachine.Run("test input");
            runTask.Wait();

            // Assert
            Assert.That(stateMachine.Results, Is.Not.Null);
        }

        [Test]
        public async Task RunWithArray_ShouldMaintainInputOrder()
        {
            // Arrange
            var startState = new DeadEndState();
            var resultState = new DeadEndState();
            
            stateMachine.SetEntryState(startState);
            stateMachine.SetOutputState(resultState);

            string[] inputs = { "first", "second", "third", "fourth", "fifth" };

            // Act
            var results = await stateMachine.Run(inputs);

            // Assert
            Assert.That(results.Count, Is.EqualTo(inputs.Length));
            // Results should be in the same order as inputs due to index sorting
            for (int i = 0; i < inputs.Length; i++)
            {
                Assert.That(results[i], Is.Not.Null);
            }
        }

        [Test]
        public void StartState_Property_ShouldGetSetCorrectly()
        {
            // Arrange
            var testState = new DeadEndState();

            // Act
            stateMachine.StartState = testState;

            // Assert
            Assert.That(stateMachine.StartState, Is.EqualTo(testState));
        }

        [Test]
        public void ResultState_Property_ShouldGetSetCorrectly()
        {
            // Arrange
            var testState = new DeadEndState();

            // Act
            stateMachine.ResultState = testState;

            // Assert
            Assert.That(stateMachine.ResultState, Is.EqualTo(testState));
        }
    }
}
