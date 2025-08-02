# Lombda.StateMachine

A powerful and flexible state machine library for .NET 8.0 that supports concurrent execution, complex state transitions, and type-safe operations. This library provides both generic and non-generic state machine implementations with extensive event handling and logging capabilities.

## Features

- **Generic and Non-Generic State Machines**: Support for both strongly-typed and object-based state machines
- **Concurrent Execution**: Built-in thread management with configurable thread limits
- **Flexible State Transitions**: Conditional state transitions with support for data transformation
- **Event-Driven Architecture**: Comprehensive event system for monitoring state machine execution
- **Lambda State Support**: Easy integration of lambda functions as states
- **Cancellation Support**: Graceful cancellation and cleanup mechanisms
- **Runtime Properties**: Persistent storage for state machine runtime data
- **Step Recording**: Optional execution step recording for debugging and analysis
- **Async/Await Support**: Full asynchronous operation support throughout the library

## Installation

Add the project reference to your solution or include the source files directly in your project.

## Quick Start

### Basic State Machine Usage

```csharp
using Lombda.StateMachine;

// Create a simple state that converts string to int
public class StringToIntState : BaseState<string, int>
{
    public override async Task<int> Invoke(string input)
    {
        return int.Parse(input);
    }

    public override List<StateProcess>? CheckConditions()
    {
        // Return null for terminal state, or return new states to transition to
        return null;
    }
}

// Use the state machine
var stateMachine = new StateMachine();
var startState = new StringToIntState();

await stateMachine.Run(startState, "42");
```

### Generic State Machine Usage

```csharp
var stateMachine = new StateMachine<string, int>();
var startState = new StringToIntState();
var resultState = new StringToIntState();

stateMachine.SetEntryState(startState);
stateMachine.SetOutputState(resultState);

var results = await stateMachine.Run("42");
```

### Lambda State Usage

```csharp
// Create a lambda state for simple transformations
LambdaMethod<string, int> stringLength = (input) => input.Length;
var lambdaState = new LambdaState<string, int>(stringLength);

var stateMachine = new StateMachine<string, int>();
stateMachine.SetEntryState(lambdaState);
stateMachine.SetOutputState(lambdaState);

var results = await stateMachine.Run("Hello World"); // Returns [11]
```

## Architecture Overview

### Core Components

#### StateMachine
The main orchestrator that manages state execution, transitions, and lifecycle.

**Key Properties:**
- `MaxThreads`: Maximum number of concurrent threads (default: 20)
- `RuntimeProperties`: Thread-safe dictionary for storing runtime data
- `RecordSteps`: Enable/disable execution step recording
- `IsFinished`: Indicates if execution is complete

**Key Events:**
- `OnBegin`: Triggered when state machine starts
- `OnTick`: Triggered at each execution loop
- `OnStateEntered`: Triggered when entering a state
- `OnStateExited`: Triggered when exiting a state
- `OnStateInvoked`: Triggered when invoking a state
- `FinishedTriggered`: Triggered when execution completes
- `CancellationTriggered`: Triggered when execution is cancelled

#### BaseState<TInput, TOutput>
Abstract base class for implementing custom states.

**Key Methods to Implement:**
- `Invoke(TInput input)`: Main processing logic
- `CheckConditions()`: Determines next state transitions

**Key Properties:**
- `Input`: List of input values being processed
- `Output`: List of output results
- `CombineInput`: Process all inputs in a single invocation
- `IsDeadEnd`: Marks state as terminal (no transitions required)
- `AllowsParallelTransitions`: Enable parallel state transitions

#### StateTransition<T>
Defines conditional transitions between states.

```csharp
// Simple transition
var transition = new StateTransition<int>(
    input => input > 5,  // Condition
    nextState           // Target state
);

// Transition with data transformation
var conversionTransition = new StateTransition<int, string>(
    input => input > 0,           // Condition
    input => input.ToString(),    // Conversion
    nextState                     // Target state
);
```

#### StateProcess
Represents a unit of work within the state machine.

**Key Properties:**
- `State`: The state to execute
- `Input`: The input data
- `MaxReruns`: Maximum retry attempts (default: 3)
- `ID`: Unique identifier

#### StateResult
Contains the output from state execution.

**Key Properties:**
- `ProcessID`: Identifier linking to the source process
- `Result`: The actual result data

## Advanced Usage

### Complex State Transitions

```csharp
public class ConditionalState : BaseState<int, int>
{
    public override async Task<int> Invoke(int input)
    {
        // Process the input
        return input;
    }

    public override List<StateProcess>? CheckConditions()
    {
        var processes = new List<StateProcess>();
        
        foreach (var output in Output)
        {
            if (output > 10)
            {
                // Transition to one state for large values
                processes.Add(new StateProcess(new LargeNumberState(), output));
            }
            else if (output > 0)
            {
                // Transition to another state for small positive values
                processes.Add(new StateProcess(new SmallNumberState(), output));
            }
            // Negative numbers don't transition (terminal)
        }
        
        return processes;
    }
}
```

### Event Handling and Monitoring

```csharp
var stateMachine = new StateMachine();

// Set up comprehensive logging
stateMachine.VerboseLog += message => Console.WriteLine($"[LOG] {message}");

// Monitor state lifecycle
stateMachine.OnStateEntered += process => 
    Console.WriteLine($"Entered: {process.State.GetType().Name}");

stateMachine.OnStateExited += state => 
    Console.WriteLine($"Exited: {state.GetType().Name}");

// Track execution progress
var executionLog = new List<string>();
stateMachine.OnTick += () => executionLog.Add($"Tick at {DateTime.Now}");

await stateMachine.Run(startState, input);
```

### Cancellation and Cleanup

```csharp
var stateMachine = new StateMachine();
var cancellationTokenSource = new CancellationTokenSource();

// Set up cancellation handling
stateMachine.CancellationTriggered += () => 
    Console.WriteLine("State machine execution was cancelled");

// Start execution
var runTask = stateMachine.Run(startState, input);

// Cancel after 5 seconds
await Task.Delay(5000);
stateMachine.Stop();

await runTask; // Will complete with cancellation
```

### Runtime Properties

```csharp
var stateMachine = new StateMachine();

// Store configuration
stateMachine.RuntimeProperties["MaxRetries"] = 3;
stateMachine.RuntimeProperties["StartTime"] = DateTime.Now;

// Access in states
public class ConfigurableState : BaseState<string, string>
{
    public override async Task<string> Invoke(string input)
    {
        var maxRetries = (int)CurrentStateMachine.RuntimeProperties["MaxRetries"];
        var startTime = (DateTime)CurrentStateMachine.RuntimeProperties["StartTime"];
        
        // Use configuration in processing
        return ProcessWithRetries(input, maxRetries);
    }
}
```

### Batch Processing

```csharp
var stateMachine = new StateMachine<string, string>();
stateMachine.SetEntryState(processingState);
stateMachine.SetOutputState(resultState);

// Process multiple inputs
string[] inputs = { "input1", "input2", "input3", "input4" };
var results = await stateMachine.Run(inputs);

// Results maintain input order
for (int i = 0; i < inputs.Length; i++)
{
    Console.WriteLine($"Input: {inputs[i]} -> Output: {results[i]}");
}
```

## Testing

The library includes comprehensive NUnit tests covering:

- Basic state machine operations
- Generic state machine functionality
- State lifecycle management
- Transition logic
- Event handling
- Cancellation scenarios
- Integration testing
- Lambda state functionality
- Error handling

### Running Tests

```bash
# Build the solution
dotnet build

# Run all tests
dotnet test

# Run tests with detailed output
dotnet test --logger "console;verbosity=detailed"

# Run specific test class
dotnet test --filter "FullyQualifiedName~StateMachineTests"
```

### Test Coverage

The test suite includes:

- **Unit Tests**: Individual component testing
- **Integration Tests**: End-to-end workflow testing
- **Performance Tests**: Concurrent execution validation
- **Error Handling Tests**: Exception and cancellation scenarios

## Performance Considerations

### Thread Management
- Default maximum threads: 20 (configurable via `MaxThreads`)
- Thread-safe operations throughout the library
- Semaphore-based access control for critical sections

### Memory Management
- Automatic cleanup of completed processes
- Optional step recording (disable for production if not needed)
- Efficient collection handling for large datasets

### Best Practices
1. Use `CombineInput = true` for states that can process multiple inputs efficiently
2. Implement proper `CheckConditions()` logic to avoid infinite loops
3. Set `IsDeadEnd = true` for terminal states
4. Use cancellation tokens for long-running operations
5. Monitor memory usage when processing large datasets

## Common Patterns

### Pipeline Processing
```csharp
// State 1: Parse input
public class ParseState : BaseState<string, Data> { ... }

// State 2: Validate data
public class ValidateState : BaseState<Data, Data> { ... }

// State 3: Process data
public class ProcessState : BaseState<Data, Result> { ... }

// Chain them together through CheckConditions()
```

### Conditional Branching
```csharp
public override List<StateProcess>? CheckConditions()
{
    var processes = new List<StateProcess>();
    
    foreach (var output in Output)
    {
        if (output.IsValid)
            processes.Add(new StateProcess(new SuccessState(), output));
        else
            processes.Add(new StateProcess(new ErrorHandlingState(), output));
    }
    
    return processes;
}
```

### Fan-Out/Fan-In
```csharp
// Fan-out: One state produces multiple processes
public override List<StateProcess>? CheckConditions()
{
    return Output.SelectMany(item => 
        item.SubItems.Select(sub => 
            new StateProcess(new ProcessSubItemState(), sub)
        )
    ).ToList();
}

// Fan-in: Collect results in a aggregation state
public class AggregationState : BaseState<List<Result>, Summary>
{
    public AggregationState()
    {
        CombineInput = true; // Process all inputs together
    }
}
```

## API Reference

### StateMachine Class
```csharp
public class StateMachine
{
    // Properties
    public int MaxThreads { get; set; }
    public bool RecordSteps { get; set; }
    public bool IsFinished { get; set; }
    public ConcurrentDictionary<string, object> RuntimeProperties { get; set; }
    public List<StateProcess> ActiveProcesses { get; }
    public CancellationToken CancelToken { get; }

    // Events
    public event Action OnBegin;
    public event Action OnTick;
    public event Action<StateProcess>? OnStateEntered;
    public event Action<BaseState>? OnStateExited;
    public event Action<StateProcess>? OnStateInvoked;
    public event Action? FinishedTriggered;
    public event Action? CancellationTriggered;
    public event Action<string>? VerboseLog;

    // Methods
    public async Task Run(BaseState runStartState, object? input = null);
    public void Stop();
    public void Finish();
    public void ResetRun();
}
```

### StateMachine<TInput, TOutput> Class
```csharp
public class StateMachine<TInput, TOutput> : StateMachine
{
    // Properties
    public List<TOutput>? Results { get; }
    public BaseState StartState { get; set; }
    public BaseState ResultState { get; set; }

    // Methods
    public async Task<List<TOutput?>> Run(TInput input);
    public async Task<List<List<TOutput?>>> Run(TInput[] inputs);
    public void SetEntryState(BaseState startState);
    public void SetOutputState(BaseState resultState);
}
```

## Contributing

When contributing to this project:

1. Follow the existing code style and patterns
2. Add comprehensive unit tests for new functionality
3. Update documentation for public API changes
4. Ensure all tests pass before submitting

## License

This project is available under the terms specified in the project license.

## Changelog

### Version 1.0.0
- Initial release with core state machine functionality
- Generic and non-generic implementations
- Lambda state support
- Comprehensive event system
- Thread management and cancellation support
- Full test coverage