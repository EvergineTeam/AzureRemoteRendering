﻿// <auto-generated/>
#pragma warning disable

using System;
using System.Collections.Generic;

namespace Microsoft.Azure.RemoteRendering
{
    /// <summary>
    /// This class provides statistics about the video stream from the service.
    /// <para>The instance needs to be updated every frame to produce averages.</para>
    /// </summary>
    public class ARRServiceStats
    {
        public struct Stats
        {
            public float TimeSinceLastPresentAvg;
            public float TimeSinceLastPresentMax;
            public UInt32 VideoFramesSkipped;
            public UInt32 VideoFramesReused;
            public UInt32 VideoFramesReceived;
            public float VideoFrameMinDelta;
            public float VideoFrameMaxDelta;
            public float LatencyPoseToReceiveAvg;
            public float LatencyReceiveToPresentAvg;
            public float LatencyPresentToDisplayAvg;
            public UInt32 VideoFramesDiscarded;
            public UInt32 VideoFramesDiscardedTotal;
            public UInt32 FramesUsedForAverage;

            public PerformanceAssessment CurrentPerformanceAssessment;

            public override string ToString()
            {
                return $"Render: {FramesUsedForAverage} fps - {(TimeSinceLastPresentAvg * 1000).ToString("F2")} / {(TimeSinceLastPresentMax * 1000).ToString("F2")} ms (avg / max)\r\n"
                + $"Video frames: {VideoFramesSkipped} / {VideoFramesReused} / {VideoFramesReceived} skipped / reused / received\r\n"
                + $"Video frames delta: {(VideoFrameMinDelta * 1000).ToString("F2")} / {(VideoFrameMaxDelta * 1000).ToString("F2")} ms (min / max)\r\n"
                + $"Latency: {(LatencyPoseToReceiveAvg * 1000).ToString("F2")} / {(LatencyReceiveToPresentAvg * 1000).ToString("F2")} / {(LatencyPresentToDisplayAvg * 1000).ToString("F2")} ms (avg) pose/receive/display  \r\n"
                + $"Video frames discarded: {VideoFramesDiscarded} / {VideoFramesDiscardedTotal} frames (last sec / total)\r\n"
                + $"Frame time CPU/GPU: {CurrentPerformanceAssessment.timeCPU.aggregate.ToString("F2") } ms ({CurrentPerformanceAssessment.timeCPU.rating}) / {CurrentPerformanceAssessment.timeGPU.aggregate.ToString("F2")} ms ({CurrentPerformanceAssessment.timeGPU.rating})\n"
                + $"Utilization CPU/GPU: {CurrentPerformanceAssessment.utilizationCPU.aggregate.ToString("F2") } % ({CurrentPerformanceAssessment.utilizationCPU.rating}) / {CurrentPerformanceAssessment.utilizationGPU.aggregate.ToString("F2")} % ({CurrentPerformanceAssessment.utilizationGPU.rating})\n"
                + $"Memory CPU/GPU: {CurrentPerformanceAssessment.memoryCPU.aggregate.ToString("F2") } % ({CurrentPerformanceAssessment.memoryCPU.rating}) / {CurrentPerformanceAssessment.memoryGPU.aggregate.ToString("F2")} % ({CurrentPerformanceAssessment.memoryGPU.rating})\n"
                + $"Network roundtrip: {CurrentPerformanceAssessment.networkLatency.aggregate.ToString("F2") } ms ({CurrentPerformanceAssessment.networkLatency.rating})\n"
                + $"Polygons rendered: {CurrentPerformanceAssessment.polygonsRendered.aggregate.ToString("N0")} ({CurrentPerformanceAssessment.polygonsRendered.rating})";
            }
        }

        private double _currWindowStartTime = DateTime.Now.TimeOfDay.TotalSeconds;
        private uint _videoFramesDiscardedTotal = 0;
        private List<FrameStatistics> _currWindowFrameStats = new List<FrameStatistics>();
        private List<FrameStatistics> _lastWindowFrameStats = new List<FrameStatistics>();
        private PerformanceAssessmentAsync m_runningPerformanceAssesment;
        private PerformanceAssessment m_lastPerformanceAssessment = new PerformanceAssessment();

        /// <summary>
        /// Call every frame to collect statistics for given frame from the graphics binding.
        /// </summary>
        public void Update(AzureSession session)
        {
            UpdateStats(session);
        }

        /// <summary>
        /// Get statistics for last second of the video stream.
        /// </summary>
        public Stats GetStats()
        {
            Stats s = new Stats();

            foreach (FrameStatistics frameStatistics in _lastWindowFrameStats)
            {
                s.TimeSinceLastPresentAvg += frameStatistics.timeSinceLastPresent;
                s.TimeSinceLastPresentMax = Math.Max(s.TimeSinceLastPresentMax, frameStatistics.timeSinceLastPresent);

                s.VideoFramesSkipped += frameStatistics.videoFramesSkipped;
                s.VideoFramesReused += frameStatistics.videoFrameReusedCount > 0u ? 1u : 0u;
                s.VideoFramesReceived += frameStatistics.videoFramesReceived;

                if (frameStatistics.videoFramesReceived > 0)
                {
                    if (s.VideoFrameMinDelta == 0.0f)
                    {
                        s.VideoFrameMinDelta = frameStatistics.videoFrameMinDelta;
                        s.VideoFrameMaxDelta = frameStatistics.videoFrameMaxDelta;
                    }
                    else
                    {
                        s.VideoFrameMinDelta = Math.Min(s.VideoFrameMinDelta, frameStatistics.videoFrameMinDelta);
                        s.VideoFrameMaxDelta = Math.Max(s.VideoFrameMaxDelta, frameStatistics.videoFrameMaxDelta);
                    }
                }

                s.LatencyPoseToReceiveAvg += frameStatistics.latencyPoseToReceive;
                s.LatencyReceiveToPresentAvg += frameStatistics.latencyReceiveToPresent;
                s.LatencyPresentToDisplayAvg += frameStatistics.latencyPresentToDisplay;
                s.VideoFramesDiscarded += frameStatistics.videoFramesDiscarded;
            }

            int frameStatsCount = _lastWindowFrameStats.Count;
            if (frameStatsCount > 0)
            {
                s.TimeSinceLastPresentAvg /= (float)frameStatsCount;
                s.LatencyPoseToReceiveAvg /= (float)frameStatsCount;
                s.LatencyReceiveToPresentAvg /= (float)frameStatsCount;
                s.LatencyPresentToDisplayAvg /= (float)frameStatsCount;
            }
            s.VideoFramesDiscardedTotal = _videoFramesDiscardedTotal;
            s.FramesUsedForAverage = (uint)frameStatsCount;

            s.CurrentPerformanceAssessment = m_lastPerformanceAssessment;

            return s;
        }

        /// <summary>
        /// Utility call to get the statistics in formatted string.
        /// </summary>
        public string GetStatsString()
        {
            Stats s = GetStats();
            return s.ToString();
        }

        private void UpdateStats(AzureSession session)
        {
            if( !session.IsConnected )
            {
                return;
            }

            FrameStatistics frameStatistics;
            if (session.GraphicsBinding.GetLastFrameStatistics(out frameStatistics) != Result.Success)
            {
                return;
            }

            if (m_runningPerformanceAssesment == null)
            {
                m_runningPerformanceAssesment = session.Actions.QueryServerPerformanceAssessmentAsync();
            }
            else if (m_runningPerformanceAssesment.IsCompleted)
            {
                if (m_runningPerformanceAssesment.IsRanToCompletion)
                {
                    m_lastPerformanceAssessment = m_runningPerformanceAssesment.Result;
                }
                m_runningPerformanceAssesment = null;
            }

            // If 1 second has past, clear the last stats list.
            var now = DateTime.Now.TimeOfDay.TotalSeconds;
            if (now > _currWindowStartTime + 1)
            {
                System.Diagnostics.Debug.Assert(!ReferenceEquals(_lastWindowFrameStats, _currWindowFrameStats));
                Swap(ref _lastWindowFrameStats, ref _currWindowFrameStats);
                _currWindowFrameStats.Clear();

                // Next list clearing should happen at least 1 second from now
                do
                {
                    _currWindowStartTime += 1;
                } while (now > _currWindowStartTime + 1);
            }

            _currWindowFrameStats.Add(frameStatistics);
            _videoFramesDiscardedTotal += frameStatistics.videoFramesDiscarded;
        }

        private void Swap<T>(ref T a, ref T b)
        {
            T t = a;
            a = b;
            b = t;
        }
    }
}

