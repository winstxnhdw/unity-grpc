using System;
using Grpc.Core;

public class GrpcChannel : IDisposable {
    Channel Channel { get; }

    public GrpcChannel(string host, int port, ChannelCredentials credentials = null) {
        this.Channel = new Channel(
            $"{host}:{port}",
            credentials ?? ChannelCredentials.Insecure
        );
    }

    public T CreateClient<T>() where T : ClientBase<T> => (T)Activator.CreateInstance(typeof(T), this.Channel);

    public void Dispose() => this.Channel.ShutdownAsync().Wait();

    public static implicit operator bool(GrpcChannel instance) => instance != null;
}
