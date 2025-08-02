using NUnit.Framework;
using Lombda.StateMachine;

namespace Lombda.StateMachine.Tests
{
    [TestFixture]
    public class StateResultTests
    {
        [Test]
        public void Constructor_WithValidParameters_ShouldCreateResult()
        {
            // Arrange
            var processId = "test-process-123";
            var result = "test result";

            // Act
            var stateResult = new StateResult(processId, result);

            // Assert
            Assert.That(stateResult.ProcessID, Is.EqualTo(processId));
            Assert.That(stateResult._Result, Is.EqualTo(result));
        }

        [Test]
        public void GetResult_ShouldReturnTypedResult()
        {
            // Arrange
            var processId = "test-process-123";
            var result = "test result";
            var stateResult = new StateResult(processId, result);

            // Act
            var typedResult = stateResult.GetResult<string>();

            // Assert
            Assert.That(typedResult, Is.Not.Null);
            Assert.That(typedResult.ProcessID, Is.EqualTo(processId));
            Assert.That(typedResult.Result, Is.EqualTo(result));
            Assert.That(typedResult._Result, Is.EqualTo(result));
        }

        [Test]
        public void GetResult_WithDifferentType_ShouldCastCorrectly()
        {
            // Arrange
            var processId = "test-process-123";
            var result = 42;
            var stateResult = new StateResult(processId, result);

            // Act
            var typedResult = stateResult.GetResult<int>();

            // Assert
            Assert.That(typedResult.Result, Is.EqualTo(42));
            Assert.That(typedResult.ProcessID, Is.EqualTo(processId));
        }

        [Test]
        public void ProcessID_ShouldBeAccessible()
        {
            // Arrange
            var processId = "test-process-123";
            var result = "test result";
            var stateResult = new StateResult(processId, result);

            // Act & Assert
            Assert.That(stateResult.ProcessID, Is.EqualTo(processId));
        }

        [Test]
        public void _Result_ShouldBeAccessible()
        {
            // Arrange
            var processId = "test-process-123";
            var result = "test result";
            var stateResult = new StateResult(processId, result);

            // Act & Assert
            Assert.That(stateResult._Result, Is.EqualTo(result));
        }
    }

    [TestFixture]
    public class GenericStateResultTests
    {
        [Test]
        public void Constructor_WithValidParameters_ShouldCreateTypedResult()
        {
            // Arrange
            var processId = "test-process-123";
            var result = "test result";

            // Act
            var stateResult = new StateResult<string>(processId, result);

            // Assert
            Assert.That(stateResult.ProcessID, Is.EqualTo(processId));
            Assert.That(stateResult.Result, Is.EqualTo(result));
            Assert.That(stateResult._Result, Is.EqualTo(result));
        }

        [Test]
        public void Result_Property_ShouldGetAndSetCorrectly()
        {
            // Arrange
            var processId = "test-process-123";
            var initialResult = "initial result";
            var newResult = "new result";
            var stateResult = new StateResult<string>(processId, initialResult);

            // Act
            stateResult.Result = newResult;

            // Assert
            Assert.That(stateResult.Result, Is.EqualTo(newResult));
            Assert.That(stateResult._Result, Is.EqualTo(newResult));
        }

        [Test]
        public void Result_WhenSet_ShouldUpdateBaseResult()
        {
            // Arrange
            var processId = "test-process-123";
            var stateResult = new StateResult<string>(processId, "initial");

            // Act
            stateResult.Result = "updated";

            // Assert
            Assert.That(stateResult._Result, Is.EqualTo("updated"));
            Assert.That(stateResult.Result, Is.EqualTo("updated"));
        }

        [Test]
        public void TypedStateResult_ShouldInheritFromBaseStateResult()
        {
            // Arrange
            var processId = "test-process-123";
            var result = "test result";

            // Act
            var stateResult = new StateResult<string>(processId, result);

            // Assert
            Assert.That(stateResult, Is.InstanceOf<StateResult>());
        }

        [Test]
        public void Result_WithComplexType_ShouldHandleCorrectly()
        {
            // Arrange
            var processId = "test-process-123";
            var complexResult = new List<int> { 1, 2, 3, 4, 5 };

            // Act
            var stateResult = new StateResult<List<int>>(processId, complexResult);

            // Assert
            Assert.That(stateResult.Result, Is.EqualTo(complexResult));
            Assert.That(stateResult.Result.Count, Is.EqualTo(5));
            Assert.That(stateResult.Result[0], Is.EqualTo(1));
        }

        [Test]
        public void Result_WithNullValue_ShouldHandleCorrectly()
        {
            // Arrange
            var processId = "test-process-123";
            string? nullResult = null;

            // Act
            var stateResult = new StateResult<string?>(processId, nullResult);

            // Assert
            Assert.That(stateResult.Result, Is.Null);
            Assert.That(stateResult._Result, Is.Null);
        }
    }
}
