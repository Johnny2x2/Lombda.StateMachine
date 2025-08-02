using NUnit.Framework;
using Lombda.StateMachine;

namespace Lombda.StateMachine.Tests
{
    [TestFixture]
    public class BaseStateTests
    {
        [Test]
        public async Task EnterState_ShouldAddToInputProcesses()
        {
            // Arrange
            var state = new StringToIntState();
            var process = new StateProcess<string>(state, "42");

            // Act
            await state._EnterState(process);

            // Assert
            Assert.That(state.InputProcesses.Count, Is.EqualTo(1));
            Assert.That(state.InputProcesses[0].Input, Is.EqualTo("42"));
            Assert.That(state.WasInvoked, Is.False);
        }

        [Test]
        public async Task ExitState_ShouldClearInputProcesses()
        {
            // Arrange
            var state = new StringToIntState();
            var process = new StateProcess<string>(state, "42");
            await state._EnterState(process);

            // Act
            await state._ExitState();

            // Assert
            Assert.That(state.InputProcesses.Count, Is.EqualTo(0));
        }

        [Test]
        public async Task Invoke_ShouldProcessInputAndSetWasInvoked()
        {
            // Arrange
            var state = new StringToIntState();
            var process = new StateProcess<string>(state, "42");
            await state._EnterState(process);

            // Act
            await state._Invoke();

            // Assert
            Assert.That(state.WasInvoked, Is.True);
            Assert.That(state.OutputResults.Count, Is.EqualTo(1));
            Assert.That(state.OutputResults[0].Result, Is.EqualTo(42));
        }

        [Test]
        public void Invoke_WithNoInputProcesses_ShouldThrowException()
        {
            // Arrange
            var state = new StringToIntState();

            // Act & Assert
            Assert.ThrowsAsync<InvalidOperationException>(async () => await state._Invoke());
        }

        [Test]
        public async Task CombineInput_WhenTrue_ShouldProcessOnlyFirstInput()
        {
            // Arrange
            var state = new StringToIntState();
            state.CombineInput = true;
            
            var process1 = new StateProcess<string>(state, "10");
            var process2 = new StateProcess<string>(state, "20");
            
            await state._EnterState(process1);
            await state._EnterState(process2);

            // Act
            await state._Invoke();

            // Assert
            Assert.That(state.OutputResults.Count, Is.EqualTo(1));
            // Only the first input should be processed when CombineInput is true
        }

        [Test]
        public void GetInputType_ShouldReturnCorrectType()
        {
            // Arrange
            var state = new StringToIntState();

            // Act
            var inputType = state.GetInputType();

            // Assert
            Assert.That(inputType, Is.EqualTo(typeof(string)));
        }

        [Test]
        public void GetOutputType_ShouldReturnCorrectType()
        {
            // Arrange
            var state = new StringToIntState();

            // Act
            var outputType = state.GetOutputType();

            // Assert
            Assert.That(outputType, Is.EqualTo(typeof(int)));
        }

        [Test]
        public async Task StateEvents_ShouldBeTriggeredCorrectly()
        {
            // Arrange
            var state = new StringToIntState();
            var process = new StateProcess<string>(state, "42");
            
            bool enteredTriggered = false;
            bool exitedTriggered = false;
            bool invokedTriggered = false;

            state.OnStateEntered += (p) => enteredTriggered = true;
            state.OnStateExited += (s) => exitedTriggered = true;
            state.OnStateInvoked += (p) => invokedTriggered = true;

            // Act
            await state._EnterState(process);
            await state._Invoke();
            await state._ExitState();

            // Assert
            Assert.That(enteredTriggered, Is.True);
            Assert.That(invokedTriggered, Is.True);
            Assert.That(exitedTriggered, Is.True);
        }

        [Test]
        public void ID_ShouldBeUniqueForEachState()
        {
            // Arrange & Act
            var state1 = new StringToIntState();
            var state2 = new StringToIntState();

            // Assert
            Assert.That(state1.ID, Is.Not.EqualTo(state2.ID));
            Assert.That(state1.ID, Is.Not.Null.And.Not.Empty);
            Assert.That(state2.ID, Is.Not.Null.And.Not.Empty);
        }

        [Test]
        public void IsDeadEnd_WhenSet_ShouldAllowNoTransitions()
        {
            // Arrange
            var state = new DeadEndState();

            // Act & Assert
            Assert.That(state.IsDeadEnd, Is.True);
            Assert.That(state.CheckConditions(), Is.Null);
        }

        [Test]
        public void AllowsParallelTransitions_ShouldBeConfigurable()
        {
            // Arrange
            var state = new StringToIntState();

            // Act
            state.AllowsParallelTransitions = true;

            // Assert
            Assert.That(state.AllowsParallelTransitions, Is.True);
        }

        [Test]
        public async Task Output_ShouldReturnProcessedResults()
        {
            // Arrange
            var state = new StringToIntState();
            var process = new StateProcess<string>(state, "42");
            await state._EnterState(process);

            // Act
            await state._Invoke();

            // Assert
            Assert.That(state.Output.Count, Is.EqualTo(1));
            Assert.That(state.Output[0], Is.EqualTo(42));
        }

        [Test]
        public async Task Input_ShouldReturnInputValues()
        {
            // Arrange
            var state = new StringToIntState();
            var process = new StateProcess<string>(state, "42");

            // Act
            await state._EnterState(process);

            // Assert
            Assert.That(state.Input.Count, Is.EqualTo(1));
            Assert.That(state.Input[0], Is.EqualTo("42"));
        }
    }
}
