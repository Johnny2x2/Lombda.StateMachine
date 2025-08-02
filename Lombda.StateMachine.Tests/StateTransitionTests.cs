using NUnit.Framework;
using Lombda.StateMachine;

namespace Lombda.StateMachine.Tests
{
    [TestFixture]
    public class StateTransitionTests
    {
        [Test]
        public void Constructor_WithValidTypes_ShouldCreateTransition()
        {
            // Arrange
            var nextState = new IntToStringState();
            TransitionEvent<int> condition = (input) => input > 5;

            // Act
            var transition = new StateTransition<int>(condition, nextState);

            // Assert
            Assert.That(transition.NextState, Is.EqualTo(nextState));
            Assert.That(transition.InvokeMethod, Is.EqualTo(condition));
        }

        [Test]
        public void Constructor_WithIncompatibleTypes_ShouldThrowException()
        {
            // Arrange
            var nextState = new StringToIntState(); // Expects string input
            TransitionEvent<int> condition = (input) => input > 5; // Returns int

            // Act & Assert
            // This should work because int can be converted to string in many contexts
            Assert.Throws(typeof(InvalidOperationException),() => new StateTransition<int>(condition, nextState));
        }

        [Test]
        public void Evaluate_WithTrueCondition_ShouldReturnTrue()
        {
            // Arrange
            var nextState = new MultiplyByTwoState();
            TransitionEvent<int> condition = (input) => input > 5;
            var transition = new StateTransition<int>(condition, nextState);

            // Act
            var result = transition.Evaluate(10);

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void Evaluate_WithFalseCondition_ShouldReturnFalse()
        {
            // Arrange
            var nextState = new MultiplyByTwoState();
            TransitionEvent<int> condition = (input) => input > 5;
            var transition = new StateTransition<int>(condition, nextState);

            // Act
            var result = transition.Evaluate(3);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void ConversionTransition_Constructor_WithValidTypes_ShouldCreateTransition()
        {
            // Arrange
            var nextState = new IntToStringState();
            TransitionEvent<int> condition = (input) => input > 0;
            ConversionMethod<int, int> converter = (input) => input * 2;

            // Act
            var transition = new StateTransition<int, int>(condition, converter, nextState);

            // Assert
            Assert.That(transition.NextState, Is.EqualTo(nextState));
            Assert.That(transition.InvokeMethod, Is.EqualTo(condition));
            Assert.That(transition.ConverterMethod, Is.EqualTo(converter));
        }


        [Test]
        public void ConversionTransition_Evaluate_WithFalseCondition_ShouldReturnFalse()
        {
            // Arrange
            var nextState = new IntToStringState();
            TransitionEvent<int> condition = (input) => input > 10;
            ConversionMethod<int, int> converter = (input) => input * 2;
            var transition = new StateTransition<int, int>(condition, converter, nextState);

            // Act
            var result = transition.Evaluate(5);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void ConversionTransition_Evaluate_WithNullInput_ShouldThrowException()
        {
            // Arrange
            var nextState = new IntToStringState();
            TransitionEvent<int?> condition = (input) => input > 0;
            ConversionMethod<int?, int> converter = (input) => input.Value * 2;
            var transition = new StateTransition<int?, int>(condition, converter, nextState);

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => transition.Evaluate(null));
        }

        [Test]
        public void ConverterMethodResult_AccessedBeforeSet_ShouldThrowException()
        {
            // Arrange
            var nextState = new IntToStringState();
            TransitionEvent<int> condition = (input) => input > 0;
            ConversionMethod<int, int> converter = (input) => input * 2;
            var transition = new StateTransition<int, int>(condition, converter, nextState);

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => { var result = transition.ConverterMethodResult; });
        }

        [Test]
        public void BaseTransition_Type_ShouldBeSetCorrectly()
        {
            // Arrange
            var nextState = new MultiplyByTwoState();
            TransitionEvent<int> condition = (input) => input > 5;

            // Act
            var simpleTransition = new StateTransition<int>(condition, nextState);

            // Assert
            Assert.That(simpleTransition.type, Is.EqualTo("out"));
        }

        [Test]
        public void ConversionTransition_Type_ShouldBeSetCorrectly()
        {
            // Arrange
            var nextState = new IntToStringState();
            TransitionEvent<int> condition = (input) => input > 0;
            ConversionMethod<int, int> converter = (input) => input * 2;

            // Act
            var conversionTransition = new StateTransition<int, int>(condition, converter, nextState);

            // Assert
            Assert.That(conversionTransition.type, Is.EqualTo("in_out"));
        }

        [Test]
        public void StateTransition_NextState_ShouldBeAccessible()
        {
            // Arrange
            var nextState = new MultiplyByTwoState();
            TransitionEvent<int> condition = (input) => input > 5;
            var transition = new StateTransition<int>(condition, nextState);

            // Act & Assert
            Assert.That(transition.NextState, Is.Not.Null);
            Assert.That(transition.NextState, Is.EqualTo(nextState));
        }
    }
}
