using Lombda.StateMachine;

// Simple demonstration of state machine visualization
var stateMachine = new StateMachine<string, string>();

// Create test states from the existing test suite
var startState = new StringToIntState();
var processingState = new MultiplyByTwoState();
var finalState = new IntToStringState() { IsDeadEnd = true };

// Set up transitions
startState.AddTransition(processingState);
processingState.AddTransition(result => result > 5, finalState);

// Configure state machine
stateMachine.SetEntryState(startState);
stateMachine.SetOutputState(finalState);
stateMachine.States.Add(startState);
stateMachine.States.Add(processingState);
stateMachine.States.Add(finalState);

Console.WriteLine("=== State Machine Visualization Demo ===\n");

// Generate Graphviz DOT format
Console.WriteLine("--- Graphviz DOT Format ---");
var dotGraph = stateMachine.ToDotGraph("DemoStateMachine");
Console.WriteLine(dotGraph);

Console.WriteLine("\n--- PlantUML Format ---");
var plantUml = stateMachine.ToPlantUML("Demo State Machine");
Console.WriteLine(plantUml);

Console.WriteLine("\n--- Test Execution ---");
var result = await stateMachine.Run("3");
Console.WriteLine($"Result: {string.Join(", ", result)}");

// Test states used in the demo
public class StringToIntState : BaseState<string, int>
{
    public override async Task<int> Invoke(string input)
    {
        await Task.Delay(10);
        return int.Parse(input);
    }
}

public class MultiplyByTwoState : BaseState<int, int>
{
    public override async Task<int> Invoke(int input)
    {
        await Task.Delay(10);
        return input * 2;
    }
}

public class IntToStringState : BaseState<int, string>
{
    public override async Task<string> Invoke(int input)
    {
        await Task.Delay(10);
        return input.ToString();
    }
}