using NUnit.Framework;
using Lombda.StateMachine;

namespace Lombda.StateMachine.Tests
{
    [TestFixture]
    public class StateMachineTests
    {
        private StateMachine stateMachine;

        [SetUp]
        public void Setup()
        {
            stateMachine = new StateMachine();
        }

        [TearDown]
        public void TearDown()
        {
            stateMachine?.Stop();
            stateMachine = null;
        }

        [Test]
        public async Task Run_WithBasicState_ShouldExecuteSuccessfully()
        {
            // Arrange
            var startState = new StringToIntState();
            startState.AddTransition(new IntToStringState() { IsDeadEnd = true });
            var eventTriggered = false;
            stateMachine.OnBegin += () => eventTriggered = true;

            // Act
            await stateMachine.Run(startState, "42");

            // Assert
            Assert.That(eventTriggered, Is.True);
            Assert.That(stateMachine.IsFinished, Is.True);
            Assert.That(startState.Output.Count, Is.EqualTo(1));
            Assert.That(startState.Output[0], Is.EqualTo(42));
        }

        [Test]
        public async Task Run_WithStateTransition_ShouldTransitionCorrectly()
        {
            // Arrange
            var startState = new StringToIntState();
            startState.AddTransition(new IntToStringState() { IsDeadEnd = true });
            var finishedTriggered = false;
            stateMachine.FinishedTriggered += () => finishedTriggered = true;

            // Act
            await stateMachine.Run(startState, "10");

            // Assert
            Assert.That(finishedTriggered, Is.True);
            Assert.That(stateMachine.IsFinished, Is.True);
            Assert.That(startState.Output[0], Is.EqualTo(10));
        }

        [Test]
        public async Task Run_WithMaxThreadsLimit_ShouldRespectThreadLimit()
        {
            // Arrange
            stateMachine.MaxThreads = 2;
            var startState = new ConditionalState();
            startState.AddTransition((result)=> result > 2, new MultiplyByTwoState() { IsDeadEnd = true });
            var processedInputs = new List<int>();

            stateMachine.OnStateInvoked += (process) =>
            {
                if (process.State is ConditionalState)
                {
                    processedInputs.Add((int)process._Input);
                }
            };

            // Act
            await stateMachine.Run(startState, 10);

            // Assert
            Assert.That(stateMachine.MaxThreads, Is.EqualTo(2));
            Assert.That(processedInputs.Count, Is.GreaterThanOrEqualTo(1));
        }

        [Test]
        public async Task Stop_DuringExecution_ShouldTriggerCancellation()
        {
            // Arrange
            var startState = new StringToIntState();
            startState.AddTransition(new IntToStringState() { IsDeadEnd = true });
            var cancellationTriggered = false;
            stateMachine.CancellationTriggered += () => cancellationTriggered = true;

            // Act
            var runTask = Task.Run(async () => await stateMachine.Run(startState, "42"));
            await Task.Delay(1); // Give it time to start
            stateMachine.Stop();
            await runTask;

            // Assert
            Assert.That(cancellationTriggered, Is.True);
            Assert.That(stateMachine.StopTrigger.IsCancellationRequested, Is.True);
        }

        [Test]
        public void ResetRun_ShouldResetStateMachineState()
        {
            // Arrange
            stateMachine.IsFinished = true;
            stateMachine.Stop();

            // Act
            stateMachine.ResetRun();

            // Assert
            Assert.That(stateMachine.IsFinished, Is.False);
            Assert.That(stateMachine.StopTrigger.IsCancellationRequested, Is.False);
            Assert.That(stateMachine.ActiveProcesses.Count, Is.EqualTo(0));
        }

        [Test]
        public async Task RuntimeProperties_ShouldPersistAcrossStates()
        {
            // Arrange
            var startState = new StringToIntState();
            startState.AddTransition(new IntToStringState() { IsDeadEnd = true });
            stateMachine.RuntimeProperties["TestKey"] = "TestValue";

            // Act
            await stateMachine.Run(startState, "42");

            // Assert
            Assert.That(stateMachine.RuntimeProperties.ContainsKey("TestKey"), Is.True);
            Assert.That(stateMachine.RuntimeProperties["TestKey"], Is.EqualTo("TestValue"));
        }

        [Test]
        public async Task RecordSteps_WhenEnabled_ShouldRecordExecutionSteps()
        {
            // Arrange
            var startState = new StringToIntState();
            startState.AddTransition(new IntToStringState() { IsDeadEnd = true });
            stateMachine.RecordSteps = true;

            // Act
            await stateMachine.Run(startState, "42");

            // Assert
            Assert.That(stateMachine.Steps.Count, Is.GreaterThan(0));
            Assert.That(stateMachine.Steps[0].Count, Is.GreaterThan(0));
        }

        [Test]
        public async Task OnStateEntered_ShouldBeTriggeredWhenStateEntered()
        {
            // Arrange
            var startState = new StringToIntState();
            startState.AddTransition(new IntToStringState() { IsDeadEnd = true });
            var enteredStates = new List<string>();
            
            stateMachine.OnStateEntered += (process) =>
            {
                enteredStates.Add(process.State.GetType().Name);
            };

            // Act
            await stateMachine.Run(startState, "42");

            // Assert
            Assert.That(enteredStates.Count, Is.GreaterThan(0));
            Assert.That(enteredStates, Contains.Item("StringToIntState"));
        }

        [Test]
        public async Task OnStateExited_ShouldBeTriggeredWhenStateExited()
        {
            // Arrange
            var startState = new StringToIntState();
            startState.AddTransition(new IntToStringState() { IsDeadEnd = true });
            var exitedStates = new List<string>();
            
            stateMachine.OnStateExited += (state) =>
            {
                exitedStates.Add(state.GetType().Name);
            };

            // Act
            await stateMachine.Run(startState, "42");

            // Assert
            Assert.That(exitedStates.Count, Is.GreaterThan(0));
            Assert.That(exitedStates, Contains.Item("StringToIntState"));
        }

        [Test]
        public async Task VerboseLog_WhenSet_ShouldReceiveLogMessages()
        {
            // Arrange
            var startState = new StringToIntState();
            startState.AddTransition(new IntToStringState() { IsDeadEnd = true });
            var logMessages = new List<string>();
            
            stateMachine.VerboseLog += (message) =>
            {
                logMessages.Add(message);
            };

            // Act
            await stateMachine.Run(startState, "42");

            // Assert
            Assert.That(logMessages.Count, Is.GreaterThan(0));
            Assert.That(logMessages.Any(m => m.Contains("Entering state")), Is.True);
        }

        [Test]
        public async Task Run_WithIndexAndResultingState_ShouldReturnIndexAndResults()
        {
            // Arrange
            var startState = new StringToIntState();
            var resultState = new StringToIntState();
            var intToStringState = new IntToStringState();
            startState.AddTransition(intToStringState);
            intToStringState.AddTransition(resultState);
            resultState.AddTransition(new IntToStringState() { IsDeadEnd = true });
            int testIndex = 5;

            // Act
            var result = await stateMachine.Run(startState, "42", testIndex, resultState);

            // Assert
            Assert.That(result.Item1, Is.EqualTo(testIndex));
            Assert.That(result.Item2, Is.Not.Null);
        }
    }
}
