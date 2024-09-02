class AppLifecycle
: IAppLifecycle
{
    private TaskCompletionSource _tcs = new();

    public void SignalStarted() => _tcs.TrySetResult();

    public Task WaitForStarted(CancellationToken cancellationToken)
    {
        cancellationToken.Register(() => _tcs.TrySetCanceled());
        return _tcs.Task;
    }
}


