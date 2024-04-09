/*
 * Copyright 2014 Splunk, Inc.
 *
 * Licensed under the Apache License, Version 2.0 (the "License"): you may
 * not use this file except in compliance with the License. You may obtain
 * a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
 * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
 * License for the specific language governing permissions and limitations
 * under the License.
 */
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Splunk.Logging
{
    /// <summary>
    /// TcpSocketWriter encapsulates queueing strings to be written to a TCP socket
    /// and handling reconnections (according to a TcpConnectionPolicy object passed
    /// to it) when a TCP session drops.
    /// </summary>
    /// <remarks>
    /// TcpSocketWriter maintains a fixed sized queue of strings to be sent via
    /// the TCP port and, while the socket is open, sends them as quickly as possible.
    /// 
    /// If the TCP session drops, TcpSocketWriter will stop pulling strings off the
    /// queue until it can reestablish a connection. Any SocketErrors emitted during this
    /// process will be passed as arguments to invocations of LoggingFailureHandler.
    /// If the TcpConnectionPolicy.Connect method throws an exception (in particular,
    /// TcpReconnectFailure to indicate that the policy has reached a point where it 
    /// will no longer try to establish a connection) then the LoggingFailureHandler 
    /// event is invoked, and no further attempt to log anything will be made.
    /// </remarks>
    public class TcpSocketWriter : IDisposable
    {
        private FixedSizeQueue<string> eventQueue;
        private Thread queueListener;
        private ITcpReconnectionPolicy reconnectPolicy;
        private CancellationTokenSource tokenSource; // Must be private or Dispose will not function properly.
        private Func<IPAddress, int, Socket> tryOpenSocket;
        private TaskCompletionSource<bool> disposed = new TaskCompletionSource<bool>();

        private Socket socket;
        private IPAddress host;
        private int port;

        /// <summary>
        /// Event that is invoked when reconnecting after a TCP session is dropped fails.
        /// </summary>
        public event Action<Exception> LoggingFailureHandler = (ex) => { };

        /// <summary>
        /// Construct a TCP socket writer that writes to the given host and port.
        /// </summary>
        /// <param name="host">IPAddress of the host to open a TCP socket to.</param>
        /// <param name="port">TCP port to use on the target host.</param>
        /// <param name="policy">A TcpConnectionPolicy object defining reconnect behavior.</param>
        /// <param name="maxQueueSize">The maximum number of log entries to queue before starting to drop entries.</param>
        /// <param name="progress">An IProgress object that reports when the queue of entries to be written reaches empty or there is
        /// a reconnection failure. This is used for testing purposes only.</param>
        public TcpSocketWriter(IPAddress host, int port, ITcpReconnectionPolicy policy,
            int maxQueueSize, Func<IPAddress, int, Socket> connect = null)
        {
            this.host = host;
            this.port = port;
            this.reconnectPolicy = policy;
            this.eventQueue = new FixedSizeQueue<string>(maxQueueSize);
            this.tokenSource = new CancellationTokenSource();

            if (connect == null)
            {
                this.tryOpenSocket = (h, p) =>
                {
                    try
                    {
                        var socket = new Socket(SocketType.Stream, ProtocolType.Tcp);
                        socket.Connect(host, port);
                        return socket;
                    }
                    catch (SocketException e)
                    {
                        LoggingFailureHandler(e);
                        throw;
                    }
                };
            }
            else
            {
                this.tryOpenSocket = (h, p) =>
                {
                    try
                    {
                        return connect(h, p);
                    }
                    catch (SocketException e)
                    {
                        LoggingFailureHandler(e);
                        throw;
                    }
                };
            }

            var threadReady = new TaskCompletionSource<bool>();

            queueListener = new Thread(() =>
            {
                try
                {
                    this.socket = this.reconnectPolicy.Connect(tryOpenSocket, host, port, tokenSource.Token);
                    threadReady.SetResult(true); // Signal the calling thread that we are ready.

                    string entry = null;
                    while (this.socket != null) // null indicates that the thread has been cancelled and cleaned up.
                    {
                        if (tokenSource.Token.IsCancellationRequested)
                        {
                            eventQueue.CompleteAdding();
                            // Post-condition: no further items will be added to the queue, so there will be a finite number of items to handle.
                            while (eventQueue.Count > 0)
                            {
                                entry = eventQueue.Dequeue();
                                try
                                {
                                    this.socket.Send(Encoding.UTF8.GetBytes(entry));
                                }
                                catch (SocketException ex)
                                {
                                    LoggingFailureHandler(ex);
                                }
                            }
                            break;
                        }
                        else if (entry == null)
                        {
                            entry = eventQueue.Dequeue(tokenSource.Token);
                        }
                        else if (entry != null)
                        {
                            try
                            {
                                if (this.socket.Send(Encoding.UTF8.GetBytes(entry)) != -1)
                                {
                                    entry = null;
                                }
                            }
                            catch (SocketException ex)
                            {
                                LoggingFailureHandler(ex);
                                this.socket = this.reconnectPolicy.Connect(tryOpenSocket, host, port, tokenSource.Token);
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    LoggingFailureHandler(e);
                }
                finally
                {
                    if (socket != null)
                    {
                        socket.Close();
                        socket.Dispose();
                    }

                    disposed.SetResult(true);
                }
            });
            queueListener.IsBackground = true; // Prevent the thread from blocking the process from exiting.
            queueListener.Start();
            threadReady.Task.Wait();
        }

        public void Dispose()
        {
            // The following operations are idempotent. Issue a cancellation to tell the
            // writer thread to stop the queue from accepting entries and write what it has
            // before cleaning up, then wait until that cleanup is finished.
            this.tokenSource.Cancel();
            Task.Run(async () => await disposed.Task).Wait();
        }

        /// <summary>
        /// Push a string onto the queue to be written.
        /// </summary>
        /// <param name="entry">The string to be written to the TCP socket.</param>
        public void Enqueue(string entry)
        {
            this.eventQueue.Enqueue(entry);
        }
    }
}

