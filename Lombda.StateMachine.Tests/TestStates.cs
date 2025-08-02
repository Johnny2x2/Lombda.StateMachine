using NUnit.Framework;
using Lombda.StateMachine;
using System.Collections.Concurrent;

namespace Lombda.StateMachine.Tests
{
    /// <summary>
    /// Test states used across multiple test classes
    /// </summary>
    public class StringToIntState : BaseState<string, int>
    {
        public override async Task<int> Invoke(string input)
        {
            await Task.Delay(10); // Simulate some work
            return int.Parse(input);
        }
    }

    public class IntToStringState : BaseState<int, string>
    {
        public override async Task<string> Invoke(int input)
        {
            await Task.Delay(10); // Simulate some work
            return input.ToString();
        }
    }

    public class MultiplyByTwoState : BaseState<int, int>
    {
        public override async Task<int> Invoke(int input)
        {
            await Task.Delay(10); // Simulate some work
            return input * 2;
        }
    }

    public class AdditionState : BaseState<List<int>, int>
    {
        public override async Task<int> Invoke(List<int> input)
        {
            await Task.Delay(10); // Simulate some work
            return input.Sum();
        }
    }

    public class ConditionalState : BaseState<int, int>
    {
        public override async Task<int> Invoke(int input)
        {
            await Task.Delay(10); // Simulate some work
            return input;
        }
    }

    public class DeadEndState : BaseState<string, string>
    {
        public DeadEndState()
        {
            IsDeadEnd = true;
        }

        public override async Task<string> Invoke(string input)
        {
            await Task.Delay(10); // Simulate some work
            return $"Processed: {input}";
        }
    }

    public class ErrorState : BaseState<string, string>
    {
        public override async Task<string> Invoke(string input)
        {
            throw new InvalidOperationException("Simulated error");
        }
    }
}
