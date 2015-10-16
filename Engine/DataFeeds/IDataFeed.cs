﻿/*
 * QUANTCONNECT.COM - Democratizing Finance, Empowering Individuals.
 * Lean Algorithmic Trading Engine v2.0. Copyright 2014 QuantConnect Corporation.
 * 
 * Licensed under the Apache License, Version 2.0 (the "License"); 
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 *
*/

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Threading;
using QuantConnect.Interfaces;
using QuantConnect.Lean.Engine.Results;
using QuantConnect.Packets;
using QuantConnect.Securities;
using QuantConnect.Util;

namespace QuantConnect.Lean.Engine.DataFeeds
{
    /// <summary>
    /// Datafeed interface for creating custom datafeed sources.
    /// </summary>
    [InheritedExport(typeof(IDataFeed))]
    public interface IDataFeed
    {
        /// <summary>
        /// Event fired when the data feed encounters new fundamental data.
        /// This event must be fired when there is nothing in the <see cref="Bridge"/>,
        /// this can be accomplished using <see cref="BusyBlockingCollection{T}.Wait(int,CancellationToken)"/>
        /// </summary>
        event EventHandler<UniverseSelectionEventArgs> UniverseSelection;
        
        /// <summary>
        /// Gets all of the current subscriptions this data feed is processing
        /// </summary>
        IEnumerable<Subscription> Subscriptions
        {
            get;
        }

        /// <summary>
        /// Cross-threading queue so the datafeed pushes data into the queue and the primary algorithm thread reads it out.
        /// </summary>
        BusyBlockingCollection<TimeSlice> Bridge
        {
            get;
        }

        /// <summary>
        /// Public flag indicator that the thread is still busy.
        /// </summary>
        bool IsActive
        {
            get;
        }

        /// <summary>
        /// Initializes the data feed for the specified job and algorithm
        /// </summary>
        void Initialize(IAlgorithm algorithm, AlgorithmNodePacket job, IResultHandler resultHandler);

        /// <summary>
        /// Adds a new subscription to provide data for the specified security.
        /// </summary>
        /// <param name="security">The security to add a subscription for</param>
        /// <param name="utcStartTime">The start time of the subscription</param>
        /// <param name="utcEndTime">The end time of the subscription</param>
        /// <param name="isUserDefinedSubscription">Set to true to prevent coarse universe selection from removing this subscription</param>
        bool AddSubscription(Security security, DateTime utcStartTime, DateTime utcEndTime, bool isUserDefinedSubscription);

        /// <summary>
        /// Removes the subscription from the data feed, if it exists
        /// </summary>
        /// <param name="security">The security to remove subscriptions for</param>
        bool RemoveSubscription(Security security);

        /// <summary>
        /// Primary entry point.
        /// </summary>
        void Run();

        /// <summary>
        /// External controller calls to signal a terminate of the thread.
        /// </summary>
        void Exit();
    }
}
