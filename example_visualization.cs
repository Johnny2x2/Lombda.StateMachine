using Lombda.StateMachine;
using System;
using System.IO;

// This example demonstrates how to use the state machine visualization features

// First, create some example states
public class StartState : BaseState<string, int>
{
    public override async Task<int> Invoke(string input)
    {
        await Task.Delay(10); // Simulate some work
        return int.Parse(input);
    }
}

public class ProcessingState : BaseState<int, string>
{
    public override async Task<string> Invoke(int input)
    {
        await Task.Delay(10); // Simulate some work
        return $"Processed: {input * 2}";
    }
}

public class ConditionalBranchState : BaseState<int, int>
{
    public override async Task<int> Invoke(int input)
    {
        await Task.Delay(10); // Simulate some work
        return input;
    }
}

public class FinalState : BaseState<string, string>
{
    public FinalState()
    {
        IsDeadEnd = true; // This is a terminal state
    }

    public override async Task<string> Invoke(string input)
    {
        await Task.Delay(10); // Simulate some work
        return $"Final result: {input}";
    }
}

class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("State Machine Visualization Example");
        Console.WriteLine("===================================");

        // Create a complex state machine with multiple paths
        var stateMachine = new StateMachine<string, string>();
        
        // Create states
        var startState = new StartState();
        var processingState = new ProcessingState();
        var conditionalState = new ConditionalBranchState();
        var finalState = new FinalState();

        // Set up transitions
        startState.AddTransition(conditionalState);
        
        // Conditional transitions based on output value
        conditionalState.AddTransition(value => value > 5, processingState);
        conditionalState.AddTransition(value => value <= 5, startState); // Loop back
        
        processingState.AddTransition(finalState);

        // Configure the state machine
        stateMachine.SetEntryState(startState);
        stateMachine.SetOutputState(finalState);
        
        // Add all states to the machine for visualization
        stateMachine.States.Add(startState);
        stateMachine.States.Add(processingState);
        stateMachine.States.Add(conditionalState);
        stateMachine.States.Add(finalState);

        // Generate Graphviz DOT format
        Console.WriteLine("\n--- Graphviz DOT Format ---");
        var dotGraph = stateMachine.ToDotGraph("ExampleStateMachine");
        Console.WriteLine(dotGraph);

        // Generate PlantUML format
        Console.WriteLine("\n--- PlantUML Format ---");
        var plantUml = stateMachine.ToPlantUML("Example State Machine");
        Console.WriteLine(plantUml);

        // Save to files
        await File.WriteAllTextAsync("state_machine.dot", dotGraph);
        await File.WriteAllTextAsync("state_machine.puml", plantUml);
        
        Console.WriteLine("\n--- Files Generated ---");
        Console.WriteLine("state_machine.dot - Graphviz DOT format");
        Console.WriteLine("state_machine.puml - PlantUML format");
        
        Console.WriteLine("\n--- Usage Instructions ---");
        Console.WriteLine("To render the DOT file with Graphviz:");
        Console.WriteLine("  dot -Tpng state_machine.dot -o state_machine.png");
        Console.WriteLine("  dot -Tsvg state_machine.dot -o state_machine.svg");
        
        Console.WriteLine("\nTo render the PlantUML file:");
        Console.WriteLine("  plantuml state_machine.puml");
        Console.WriteLine("  Or use online PlantUML editor: http://www.plantuml.com/plantuml/");

        // Test the state machine with some input
        Console.WriteLine("\n--- Running State Machine ---");
        var result = await stateMachine.Run("10");
        Console.WriteLine($"State machine result: {string.Join(", ", result)}");
    }
}