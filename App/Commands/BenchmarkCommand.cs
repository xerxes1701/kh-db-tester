using CliFx;
using CliFx.Infrastructure;
using CliFx.Attributes;
using BenchmarkDotNet.Running;

[Command("benchmark")]
public class BenchmarkCommand()
  : ICommand
{
    public ValueTask ExecuteAsync(IConsole console)
    {
        BenchmarkRunner.Run<MyBenchmark>();
        return ValueTask.CompletedTask;
    }
}


