using System.Collections.Concurrent;

namespace Channel;


public class ChannelProducer
{
    public static async Task Producer(IWrite<string?> writer)
    {
        for (int i = 0; i < 100; i++)
        {
            writer.Push(i.ToString());
            await Task.Delay(100);
        }
        writer.Complete();
    }
}

public class ChannelConsumer
{
    public static async Task Consumer(IRead<string?> reader)
    {
        while (!reader.IsCompleted)
        {
            var value = await reader.ReadAsync();
            Console.WriteLine($"message: {value}");
        }
    }
}

public interface IRead<T>
{
    Task<T?> ReadAsync();
    bool IsCompleted { get; }
}

public interface IWrite<T>
{
    void Push(T? value);
    void Complete();
}

public class Channel<T> : IRead<T?>, IWrite<T>
{
    private bool _finished;
    private readonly ConcurrentQueue<T?> _queue = new();
    private readonly SemaphoreSlim _flag = new(0);

    public void Complete()
    {
        _finished = true;
    }

    public bool IsCompleted => _finished && _queue.IsEmpty;

    public void Push(T? value)
    {
        _queue.Enqueue(value);
        _flag.Release();
    }

    public async Task<T?> ReadAsync()
    {
        await _flag.WaitAsync();
        if (_queue.TryDequeue(out T? value))
        {
            return value;
        }
        return value;
    }
}
