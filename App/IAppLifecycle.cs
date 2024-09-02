public interface IAppLifecycle
{
    Task WaitForStarted(CancellationToken cancellationToken);
}


