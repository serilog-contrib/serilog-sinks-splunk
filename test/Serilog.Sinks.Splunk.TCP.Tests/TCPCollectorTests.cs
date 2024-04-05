using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Serilog.Events;
using Xunit;

namespace Serilog.Sinks.Splunk.TCP.Tests;

public class TCPCollectorTests
{
    [Fact]
    public void LoggerExtensionTest()
    {
        using (var TestEnvironment = new TestEnvironment())
        {
            var log = new LoggerConfiguration()
                .WriteTo.SplunkViaTcp(
                    new Serilog.Sinks.Splunk.SplunkTcpSinkConnectionInfo("127.0.0.1", 10001),
                    restrictedToMinimumLevel: LevelAlias.Minimum,
                    formatProvider: null,
                    renderTemplate: true)
                .CreateLogger();

            log.Information("Hello World");
        }
    }

}

class TestEnvironment : IDisposable
{
    TcpListener TcpServer;

    readonly public int TcpServerAddress;

    public TestEnvironment(int TcpServerAddress = 10001)
    {
        this.TcpServerAddress = TcpServerAddress;

        TcpServer = new TcpListener(IPAddress.Loopback, TcpServerAddress);

        TcpServer.Start();
    }

    public void Dispose()
    {
        TcpServer.Stop();
    }
}
