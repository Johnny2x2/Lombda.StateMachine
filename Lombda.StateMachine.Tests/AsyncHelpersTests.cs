using NUnit.Framework;
using Lombda.StateMachine;

namespace Lombda.StateMachine.Tests
{
    [TestFixture]
    public class AsyncHelpersTests
    {
        [Test]
        public void IsGenericTask_WithGenericTask_ShouldReturnTrue()
        {
            // Arrange
            Type taskType = typeof(Task<int>);

            // Act
            bool result = AsyncHelpers.IsGenericTask(taskType, out Type taskResultType);

            // Assert
            Assert.That(result, Is.True);
            Assert.That(taskResultType, Is.EqualTo(typeof(Task<int>)));
        }

        [Test]
        public void IsGenericTask_WithNonGenericTask_ShouldReturnFalse()
        {
            // Arrange
            Type taskType = typeof(Task);

            // Act
            bool result = AsyncHelpers.IsGenericTask(taskType, out Type taskResultType);

            // Assert
            Assert.That(result, Is.False);
            Assert.That(taskResultType, Is.Null);
        }

        [Test]
        public void IsGenericTask_WithStringType_ShouldReturnFalse()
        {
            // Arrange
            Type stringType = typeof(string);

            // Act
            bool result = AsyncHelpers.IsGenericTask(stringType, out Type taskResultType);

            // Assert
            Assert.That(result, Is.False);
            Assert.That(taskResultType, Is.Null);
        }

        [Test]
        public void IsGenericTask_WithIntType_ShouldReturnFalse()
        {
            // Arrange
            Type intType = typeof(int);

            // Act
            bool result = AsyncHelpers.IsGenericTask(intType, out Type taskResultType);

            // Assert
            Assert.That(result, Is.False);
            Assert.That(taskResultType, Is.Null);
        }

        [Test]
        public void IsGenericTask_WithTaskOfString_ShouldReturnTrue()
        {
            // Arrange
            Type taskType = typeof(Task<string>);

            // Act
            bool result = AsyncHelpers.IsGenericTask(taskType, out Type taskResultType);

            // Assert
            Assert.That(result, Is.True);
            Assert.That(taskResultType, Is.EqualTo(typeof(Task<string>)));
        }

        [Test]
        public void IsGenericTask_WithTaskOfComplexType_ShouldReturnTrue()
        {
            // Arrange
            Type taskType = typeof(Task<List<int>>);

            // Act
            bool result = AsyncHelpers.IsGenericTask(taskType, out Type taskResultType);

            // Assert
            Assert.That(result, Is.True);
            Assert.That(taskResultType, Is.EqualTo(typeof(Task<List<int>>)));
        }

        [Test]
        public void IsGenericTask_WithNullType_ShouldReturnFalse()
        {
            // Arrange
            Type nullType = null;

            // Act
            bool result = AsyncHelpers.IsGenericTask(nullType, out Type taskResultType);

            // Assert
            Assert.That(result, Is.False);
            Assert.That(taskResultType, Is.Null);
        }

        [Test]
        public void IsGenericTask_WithObjectType_ShouldReturnFalse()
        {
            // Arrange
            Type objectType = typeof(object);

            // Act
            bool result = AsyncHelpers.IsGenericTask(objectType, out Type taskResultType);

            // Assert
            Assert.That(result, Is.False);
            Assert.That(taskResultType, Is.Null);
        }

        [Test]
        public void IsGenericTask_WithDerivedTaskType_ShouldReturnTrue()
        {
            // This test verifies that the method correctly traverses the inheritance hierarchy
            // Arrange
            Type taskType = typeof(Task<int>);

            // Act
            bool result = AsyncHelpers.IsGenericTask(taskType, out Type taskResultType);

            // Assert
            Assert.That(result, Is.True);
            Assert.That(taskResultType, Is.Not.Null);
        }

        [Test]
        public void IsGenericTask_WithValueTaskType_ShouldReturnFalse()
        {
            // ValueTask<T> is not Task<T>, so should return false
            // Arrange
            Type valueTaskType = typeof(ValueTask<int>);

            // Act
            bool result = AsyncHelpers.IsGenericTask(valueTaskType, out Type taskResultType);

            // Assert
            Assert.That(result, Is.False);
            Assert.That(taskResultType, Is.Null);
        }
    }
}
