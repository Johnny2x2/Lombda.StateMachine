using NUnit.Framework;
using Lombda.StateMachine;

namespace Lombda.StateMachine.Tests
{
    [TestFixture]
    public class StateMachineVisualizationTests
    {
        [Test]
        public void ToDotGraph_SimpleStateMachine_ShouldGenerateValidDotFormat()
        {
            // Arrange
            var stateMachine = new StateMachine();
            var startState = new StringToIntState();
            var nextState = new IntToStringState() { IsDeadEnd = true };
            
            startState.AddTransition(nextState);
            stateMachine.States.Add(startState);
            stateMachine.States.Add(nextState);

            // Act
            var dotGraph = stateMachine.ToDotGraph("TestStateMachine");

            // Assert
            Assert.That(dotGraph, Is.Not.Null);
            Assert.That(dotGraph, Does.Contain("digraph TestStateMachine"));
            Assert.That(dotGraph, Does.Contain("StringToIntState"));
            Assert.That(dotGraph, Does.Contain("IntToStringState"));
            Assert.That(dotGraph, Does.Contain("->"));
            Assert.That(dotGraph, Does.Contain("rankdir=LR"));
            
            Console.WriteLine("Generated DOT Graph:");
            Console.WriteLine(dotGraph);
        }

        [Test]
        public void ToDotGraph_GenericStateMachine_ShouldGenerateValidDotFormat()
        {
            // Arrange
            var stateMachine = new StateMachine<string, string>();
            var startState = new StringToIntState();
            var resultState = new IntToStringState() { IsDeadEnd = true };
            
            startState.AddTransition(resultState);
            
            stateMachine.SetEntryState(startState);
            stateMachine.SetOutputState(resultState);
            stateMachine.States.Add(startState);
            stateMachine.States.Add(resultState);

            // Act
            var dotGraph = stateMachine.ToDotGraph("GenericTestStateMachine");

            // Assert
            Assert.That(dotGraph, Is.Not.Null);
            Assert.That(dotGraph, Does.Contain("digraph GenericTestStateMachine"));
            Assert.That(dotGraph, Does.Contain("StringToIntState"));
            Assert.That(dotGraph, Does.Contain("IntToStringState"));
            Assert.That(dotGraph, Does.Contain("Start"));
            Assert.That(dotGraph, Does.Contain("Result"));
            Assert.That(dotGraph, Does.Contain("lightgreen")); // Start state color
            Assert.That(dotGraph, Does.Contain("lightcoral")); // Result state color
            
            Console.WriteLine("Generated Generic DOT Graph:");
            Console.WriteLine(dotGraph);
        }

        [Test]
        public void ToPlantUML_SimpleStateMachine_ShouldGenerateValidPlantUMLFormat()
        {
            // Arrange
            var stateMachine = new StateMachine();
            var startState = new StringToIntState();
            var nextState = new IntToStringState() { IsDeadEnd = true };
            
            startState.AddTransition(nextState);
            stateMachine.States.Add(startState);
            stateMachine.States.Add(nextState);

            // Act
            var plantUml = stateMachine.ToPlantUML("Test State Machine");

            // Assert
            Assert.That(plantUml, Is.Not.Null);
            Assert.That(plantUml, Does.Contain("@startuml"));
            Assert.That(plantUml, Does.Contain("@enduml"));
            Assert.That(plantUml, Does.Contain("title Test State Machine"));
            Assert.That(plantUml, Does.Contain("StringToIntState"));
            Assert.That(plantUml, Does.Contain("IntToStringState"));
            Assert.That(plantUml, Does.Contain("-->"));
            
            Console.WriteLine("Generated PlantUML:");
            Console.WriteLine(plantUml);
        }

        [Test]
        public void ToPlantUML_GenericStateMachine_ShouldGenerateValidPlantUMLFormat()
        {
            // Arrange
            var stateMachine = new StateMachine<string, string>();
            var startState = new StringToIntState();
            var resultState = new IntToStringState() { IsDeadEnd = true };
            
            startState.AddTransition(resultState);
            
            stateMachine.SetEntryState(startState);
            stateMachine.SetOutputState(resultState);
            stateMachine.States.Add(startState);
            stateMachine.States.Add(resultState);

            // Act
            var plantUml = stateMachine.ToPlantUML("Generic Test State Machine");

            // Assert
            Assert.That(plantUml, Is.Not.Null);
            Assert.That(plantUml, Does.Contain("@startuml"));
            Assert.That(plantUml, Does.Contain("@enduml"));
            Assert.That(plantUml, Does.Contain("title Generic Test State Machine"));
            Assert.That(plantUml, Does.Contain("StringToIntState"));
            Assert.That(plantUml, Does.Contain("IntToStringState"));
            Assert.That(plantUml, Does.Contain("[*] -->")); // Start marker
            Assert.That(plantUml, Does.Contain("--> [*]")); // End marker
            
            Console.WriteLine("Generated Generic PlantUML:");
            Console.WriteLine(plantUml);
        }

        [Test]
        public void ToDotGraph_ComplexStateMachine_ShouldHandleMultipleTransitions()
        {
            // Arrange
            var stateMachine = new StateMachine<int, string>();
            var startState = new MultiplyByTwoState();
            var conditionalState = new ConditionalState();
            var resultState = new IntToStringState() { IsDeadEnd = true };
            
            startState.AddTransition(conditionalState);
            conditionalState.AddTransition(result => result > 5, resultState);
            conditionalState.AddTransition(result => result <= 5, startState); // Loop back
            
            stateMachine.SetEntryState(startState);
            stateMachine.SetOutputState(resultState);
            stateMachine.States.Add(startState);
            stateMachine.States.Add(conditionalState);
            stateMachine.States.Add(resultState);

            // Act
            var dotGraph = stateMachine.ToDotGraph("ComplexStateMachine");

            // Assert
            Assert.That(dotGraph, Is.Not.Null);
            Assert.That(dotGraph, Does.Contain("MultiplyByTwoState"));
            Assert.That(dotGraph, Does.Contain("ConditionalState"));
            Assert.That(dotGraph, Does.Contain("IntToStringState"));
            // Should have multiple transitions from ConditionalState
            var conditionalLines = dotGraph.Split('\n').Where(line => line.Contains("ConditionalState ->")).ToList();
            Assert.That(conditionalLines.Count, Is.GreaterThan(1));
            
            Console.WriteLine("Generated Complex DOT Graph:");
            Console.WriteLine(dotGraph);
        }

        [Test]
        public void ToPlantUML_DeadEndState_ShouldIncludeDeadEndNote()
        {
            // Arrange
            var stateMachine = new StateMachine();
            var startState = new StringToIntState();
            var deadEndState = new IntToStringState() { IsDeadEnd = true };
            
            startState.AddTransition(deadEndState);
            stateMachine.States.Add(startState);
            stateMachine.States.Add(deadEndState);

            // Act
            var plantUml = stateMachine.ToPlantUML("DeadEnd Test");

            // Assert
            Assert.That(plantUml, Is.Not.Null);
            Assert.That(plantUml, Does.Contain("Dead End State"));
            
            Console.WriteLine("Generated PlantUML with Dead End:");
            Console.WriteLine(plantUml);
        }

        [Test]
        public void ToDotGraph_EmptyStateMachine_ShouldHandleGracefully()
        {
            // Arrange
            var stateMachine = new StateMachine();

            // Act
            var dotGraph = stateMachine.ToDotGraph("EmptyStateMachine");

            // Assert
            Assert.That(dotGraph, Is.Not.Null);
            Assert.That(dotGraph, Does.Contain("digraph EmptyStateMachine"));
            Assert.That(dotGraph, Does.Contain("rankdir=LR"));
            
            Console.WriteLine("Generated Empty DOT Graph:");
            Console.WriteLine(dotGraph);
        }

        [Test]
        public void ToPlantUML_EmptyStateMachine_ShouldHandleGracefully()
        {
            // Arrange
            var stateMachine = new StateMachine();

            // Act
            var plantUml = stateMachine.ToPlantUML("Empty Test");

            // Assert
            Assert.That(plantUml, Is.Not.Null);
            Assert.That(plantUml, Does.Contain("@startuml"));
            Assert.That(plantUml, Does.Contain("@enduml"));
            Assert.That(plantUml, Does.Contain("title Empty Test"));
            
            Console.WriteLine("Generated Empty PlantUML:");
            Console.WriteLine(plantUml);
        }

        [Test]
        public void ToDotGraph_SpecialCharactersInStateName_ShouldSanitize()
        {
            // Arrange
            var stateMachine = new StateMachine();
            // Use a state with special characters that need sanitization
            var startState = new AdditionState() { IsDeadEnd = true }; // List<int> has < > characters
            
            stateMachine.States.Add(startState);

            // Act
            var dotGraph = stateMachine.ToDotGraph("SanitizationTest");

            // Assert
            Assert.That(dotGraph, Is.Not.Null);
            Assert.That(dotGraph, Does.Not.Contain("<"));
            Assert.That(dotGraph, Does.Not.Contain(">"));
            
            Console.WriteLine("Generated Sanitized DOT Graph:");
            Console.WriteLine(dotGraph);
        }
    }
}