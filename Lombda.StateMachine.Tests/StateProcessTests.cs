using NUnit.Framework;
using Lombda.StateMachine;

namespace Lombda.StateMachine.Tests
{
    [TestFixture]
    public class StateProcessTests
    {
        [Test]
        public void Constructor_WithValidParameters_ShouldCreateProcess()
        {
            // Arrange
            var state = new StringToIntState();
            var input = "42";

            // Act
            var process = new StateProcess(state, input);

            // Assert
            Assert.That(process.State, Is.EqualTo(state));
            Assert.That(process._Input, Is.EqualTo(input));
            Assert.That(process.MaxReruns, Is.EqualTo(3));
            Assert.That(process.ID, Is.Not.Null.And.Not.Empty);
        }

        [Test]
        public void Constructor_WithCustomMaxReruns_ShouldSetCorrectly()
        {
            // Arrange
            var state = new StringToIntState();
            var input = "42";
            var maxReruns = 5;

            // Act
            var process = new StateProcess(state, input, maxReruns);

            // Assert
            Assert.That(process.MaxReruns, Is.EqualTo(maxReruns));
        }

        [Test]
        public void CanReAttempt_WithinMaxReruns_ShouldReturnTrue()
        {
            // Arrange
            var state = new StringToIntState();
            var input = "42";
            var process = new StateProcess(state, input, 3);

            // Act
            var result1 = process.CanReAttempt();
            var result2 = process.CanReAttempt();

            // Assert
            Assert.That(result1, Is.True);
            Assert.That(result2, Is.True);
        }

        [Test]
        public void CanReAttempt_ExceedingMaxReruns_ShouldReturnFalse()
        {
            // Arrange
            var state = new StringToIntState();
            var input = "42";
            var process = new StateProcess(state, input, 2);

            // Act
            var result1 = process.CanReAttempt(); // attempt 1
            var result2 = process.CanReAttempt(); // attempt 2
            var result3 = process.CanReAttempt(); // attempt 3 (exceeds max of 2)

            // Assert
            Assert.That(result1, Is.True);
            Assert.That(result2, Is.False); // Should be false as it reaches max
            Assert.That(result3, Is.False);
        }

        [Test]
        public void CreateStateResult_ShouldReturnCorrectResult()
        {
            // Arrange
            var state = new StringToIntState();
            var input = "42";
            var process = new StateProcess(state, input);
            var resultValue = 100;

            // Act
            var stateResult = process.CreateStateResult(resultValue);

            // Assert
            Assert.That(stateResult.ProcessID, Is.EqualTo(process.ID));
            Assert.That(stateResult._Result, Is.EqualTo(resultValue));
        }

        [Test]
        public void GetProcess_ShouldReturnTypedProcess()
        {
            // Arrange
            var state = new StringToIntState();
            var input = "42";
            var process = new StateProcess(state, input);

            // Act
            var typedProcess = process.GetProcess<string>();

            // Assert
            Assert.That(typedProcess, Is.Not.Null);
            Assert.That(typedProcess.State, Is.EqualTo(state));
            Assert.That(typedProcess.Input, Is.EqualTo(input));
            Assert.That(typedProcess.ID, Is.EqualTo(process.ID));
        }

        [Test]
        public void ID_ShouldBeUniqueForEachProcess()
        {
            // Arrange
            var state = new StringToIntState();
            var input = "42";

            // Act
            var process1 = new StateProcess(state, input);
            var process2 = new StateProcess(state, input);

            // Assert
            Assert.That(process1.ID, Is.Not.EqualTo(process2.ID));
        }
    }

    [TestFixture]
    public class GenericStateProcessTests
    {
        [Test]
        public void Constructor_WithValidParameters_ShouldCreateTypedProcess()
        {
            // Arrange
            var state = new StringToIntState();
            var input = "42";

            // Act
            var process = new StateProcess<string>(state, input);

            // Assert
            Assert.That(process.State, Is.EqualTo(state));
            Assert.That(process.Input, Is.EqualTo(input));
            Assert.That(process._Input, Is.EqualTo(input));
            Assert.That(process.MaxReruns, Is.EqualTo(3));
        }

        [Test]
        public void Constructor_WithIdParameter_ShouldSetId()
        {
            // Arrange
            var state = new StringToIntState();
            var input = "42";
            var customId = "custom-id-123";

            // Act
            var process = new StateProcess<string>(state, input, customId);

            // Assert
            Assert.That(process.ID, Is.EqualTo(customId));
        }

        [Test]
        public void Input_Property_ShouldGetAndSetCorrectly()
        {
            // Arrange
            var state = new StringToIntState();
            var initialInput = "42";
            var newInput = "100";
            var process = new StateProcess<string>(state, initialInput);

            // Act
            process.Input = newInput;

            // Assert
            Assert.That(process.Input, Is.EqualTo(newInput));
            Assert.That(process._Input, Is.EqualTo(newInput));
        }

        [Test]
        public void Input_WhenSet_ShouldUpdateBaseInput()
        {
            // Arrange
            var state = new StringToIntState();
            var process = new StateProcess<string>(state, "initial");

            // Act
            process.Input = "updated";

            // Assert
            Assert.That(process._Input, Is.EqualTo("updated"));
            Assert.That(process.Input, Is.EqualTo("updated"));
        }

        [Test]
        public void TypedProcess_ShouldInheritFromBaseProcess()
        {
            // Arrange
            var state = new StringToIntState();
            var input = "42";

            // Act
            var process = new StateProcess<string>(state, input);

            // Assert
            Assert.That(process, Is.InstanceOf<StateProcess>());
        }
    }
}
