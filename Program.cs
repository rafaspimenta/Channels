using System.Threading.Channels;

namespace Channel
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var channel = new Channel<string?>();

            Task.WaitAll(ChannelConsumer.Consumer(channel), ChannelProducer.Producer(channel), ChannelProducer.Producer(channel), ChannelProducer.Producer(channel));
            
        }
    }
}