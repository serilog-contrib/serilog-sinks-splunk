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
using System.Threading;

namespace Splunk.Logging
{
    /// <summary>
    /// TcpConnectionPolicy encapsulates a policy for what logging via TCP should
    /// do when there is a socket error.
    /// </summary>
    /// <remarks>
    /// TCP loggers in this library (TcpTraceListener and TcpEventSink) take a 
    /// TcpConnectionPolicy as an argument to their constructor. When the TCP
    /// session the logger uses has an error, the logger suspends logging and calls
    /// the Reconnect method of an implementation of TcpConnectionPolicy to get a
    /// new socket.
    /// </remarks>
    public interface ITcpReconnectionPolicy
    {
        // A blocking method that should eventually return a Socket when it finally
        // manages to get a connection, or throw a TcpReconnectFailure if the policy
        // says to give up trying to connect.
        /// <summary>
        /// Try to reestablish a TCP connection.
        /// </summary>
        /// <remarks>
        /// The method should block until it either 
        /// 
        /// 1. succeeds and returns a connected TCP socket, or 
        /// 2. fails and throws a TcpReconnectFailure exception, or
        /// 3. the cancellationToken is cancelled, in which case the method should
        ///    return null.
        ///    
        /// The method takes a zero-parameter function that encapsulates trying to
        /// make a single connection and a cancellation token to stop the method
        /// if the logging system that invoked it is disposed.
        /// 
        /// For example, the default ExponentialBackoffTcpConnectionPolicy invokes
        /// connect after increasingly long intervals until it makes a successful
        /// connnection, or is cancelled by the cancellationToken, at which point
        /// it returns null.
        /// </remarks>
        /// <param name="connect">A zero-parameter function that tries once to 
        /// establish a connection.</param>
        /// <param name="cancellationToken">A token used to cancel the reconnect
        /// attempt when the invoking logger is disposed.</param>
        /// <returns>A connected TCP socket.</returns>
        Socket Connect(Func<IPAddress, int, Socket> connect, IPAddress host, int port, CancellationToken cancellationToken);
    }
}
