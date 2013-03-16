/* LaTeX Math IDE
Copyright (C) Daniel Jurcau 2013 

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program. If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Latex.Helpers;
using System.Threading;
using System.IO;
using System.Diagnostics;

namespace Latex.Scheduler
{
    /// <summary>
    /// Schedules the invokation of LaTeX
    /// </summary>
    public class InvokeScheduler : IDisposable
    {
        #region Public
        /// <summary>
        /// Default constructor
        /// </summary>
        public InvokeScheduler() : this(CPUInfoHelper.CoreCount)
        {
        }        
        /// <summary>
        /// Constructor that accepts a custom number of threads
        /// </summary>
        /// <param name="numberOfThreads"></param>
        public InvokeScheduler(int numberOfThreads)
        {
            if (numberOfThreads < 1)
            {
                throw new ArgumentOutOfRangeException("numberOfThreads");
            }
            this.numberOfThreads = numberOfThreads;

            semaphore = new Semaphore(numberOfThreads, numberOfThreads);
            dispather = new Thread((ThreadStart)delegate 
                {
                    InvokeRequest request = null;
                    while (!stopDispatcher)
                    {
                        request = null;
                        lock (lockObject)
                        {
                            if (pendingRequests.Count > 0)
                            {
                                request = pendingRequests.FirstOrDefault(r => r.Status == InvokeRequest.InvokeStatus.Pending);
                            }
                        }
                        if (request != null)
                        {
                            semaphore.WaitOne();
                            request.Status = InvokeRequest.InvokeStatus.Running;

                            ThreadPool.QueueUserWorkItem((WaitCallback) delegate(object r)
                            {
                                try
                                {
                                    HandleRequest((InvokeRequest)r);
                                }
                                catch (Exception) { }
                                finally
                                {
                                    semaphore.Release();
                                }
                            }, request);
                        }
                        lock (lockObject)
                        {
                            bool requestsPending =
                                pendingRequests.Count(r => r.Status == InvokeRequest.InvokeStatus.Pending) > 0;
                            if (!requestsPending)
                            {
                                releaseEvent.Reset();
                            }
                        }
                        releaseEvent.WaitOne();
                    }
                });
            dispather.Start();
        }
        /// <summary>
        /// Gets if the object is disposed
        /// </summary>
        public bool IsDisposed { get; protected set; }
        /// <summary>
        /// Gets the maximum number of threads used
        /// </summary>
        public int NumberOfThreads
        {
            get { return numberOfThreads; }
        }
        /// <summary>
        /// Raised when a request is finished
        /// </summary>
        public event EventHandler<InvokeEventArgs> RequestFinished;
        /// <summary>
        /// Adds a new invoke request
        /// </summary>
        /// <param name="request"></param>
        public void Add(InvokeRequest request)
        {
            lock (lockObject)
            {
                pendingRequests.Add(request);
                releaseEvent.Set();
            }
        }
        /// <summary>
        /// Removes all pending requests
        /// </summary>
        public void ClearPendingRequests()
        {
            lock (lockObject)
            {
                pendingRequests.RemoveAll(
                    r => r.Status == InvokeRequest.InvokeStatus.Pending);
            }
        }
        /// <summary>
        /// Disposes the object
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion

        #region Protected
        /// <summary>
        /// Number of threads to use
        /// </summary>
        protected int numberOfThreads;
        /// <summary>
        /// Object used for locking
        /// </summary>
        protected object lockObject = new object();
        /// <summary>
        /// Used to block the dispather thread if there is no work to be done
        /// </summary>
        protected ManualResetEvent releaseEvent = new ManualResetEvent(false);
        /// <summary>
        /// List of pending requests
        /// </summary>
        protected List<InvokeRequest> pendingRequests = new List<InvokeRequest>();
        /// <summary>
        /// Thread that takes care of dispathing requests
        /// </summary>
        protected Thread dispather;
        /// <summary>
        /// Indicates to the dispather that it should stop
        /// </summary>
        protected bool stopDispatcher;
        /// <summary>
        /// Semaphore used to control the number of simultaneous invokes
        /// </summary>
        protected Semaphore semaphore;
        /// <summary>
        /// Disposes the object
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (IsDisposed)
            {
                throw new ObjectDisposedException("InvokeScheduler");
            }
            if (disposing)
            {
                stopDispatcher = true;
                releaseEvent.Set();
                dispather.Join();
                IsDisposed = true;
            }
            //no unmanaged resources to release
        }
        /// <summary>
        /// Handles a request
        /// </summary>
        /// <param name="request"></param>
        protected virtual void HandleRequest(InvokeRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException("request");
            }
            //TODO: optimize if input and file are empty
            if (String.IsNullOrEmpty(request.FileName))
            {
                request.FileName =
                    RandomFileNameGenerator.GenerateTemp(".tex");
                File.WriteAllText(request.FileName,
                    request.Input);
            }
            DateTime start = DateTime.Now;
            request.Result =
                request.Invoker.Invoke(request.FileName);
            Debug.WriteLine("Request took: " + 
                DateTime.Now.Subtract(start).ToString()); 
            request.Status =
                InvokeRequest.InvokeStatus.Finished;
            pendingRequests.Remove(request);

            if (RequestFinished != null)
            {
                RequestFinished(this, new InvokeEventArgs() { Request = request });
            }
        }
        #endregion
    }
}
