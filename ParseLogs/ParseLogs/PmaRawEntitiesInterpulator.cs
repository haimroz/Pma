using System.Collections.Generic;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using PmaEntities;

namespace ParseLogs
{
    public class PmaRawEntitiesInterpulator
    {
        private class KnownDataPoint
        {
            public DateTime TimeStamp { get; }
            public double Value { get; }
            public double NextPointIncrement { get; set; }

            public KnownDataPoint(DateTime timeStamp, double value, double nextPointIncrement = 0)
            {
                TimeStamp = timeStamp;
                Value = value;
                NextPointIncrement = nextPointIncrement;
            }
        }

        public List<PmaRawEntity> BuildInterpolatedList(List<PmaRawEntity> rawList)
        {
            List<PmaRawEntity> interpolatedList = new List<PmaRawEntity>();
            if (!rawList.Any())
            {
                return interpolatedList;
            }
            Dictionary<int, List<KnownDataPoint>> mesaurmentsKnownDataPoints = BuildKnownPointsLists(rawList);
            Dictionary<int, int> currentKnownDataPointsInd = new Dictionary<int, int>();
            for (int mesaurmentInd = 0; mesaurmentInd < mesaurmentsKnownDataPoints.Count; mesaurmentInd++)
            {
                currentKnownDataPointsInd[mesaurmentInd] = 0;
            }

            PmaRawEntity pmaEntity = null;
            foreach (var rawEntity in rawList)
            {
                if (interpolatedList.Count > 0 && HasTimeGap(interpolatedList.Last(), rawEntity))
                {
                    FillTimeGaps(interpolatedList.Last().TimeStamp, rawEntity.TimeStamp.AddSeconds(-1), mesaurmentsKnownDataPoints,
                        currentKnownDataPointsInd, interpolatedList);
                }
                if (interpolatedList.Count == 0 || interpolatedList.Last().TimeStamp != rawEntity.TimeStamp)
                {
                    pmaEntity = new PmaRawEntity();
                    interpolatedList.Add(pmaEntity);
                    pmaEntity.TimeStamp = rawEntity.TimeStamp;
                }
                for (int mesaurmentInd = 0; mesaurmentInd < PmaRawEntity.NumOfMesaurments; mesaurmentInd++)
                {
                    SetPmaMesaurment(rawEntity, pmaEntity, mesaurmentInd, mesaurmentsKnownDataPoints[mesaurmentInd],
                        currentKnownDataPointsInd);
                }
            }
            return interpolatedList;
        }

        private void SetPmaMesaurment(PmaRawEntity rawEntity, PmaRawEntity pmaEntity, int mesaurmentInd,
            List<KnownDataPoint> knownDataPoints, Dictionary<int, int> currentDataPointsInd) //List<KnownDataPoint>.Enumerator nextDataPointEnumerator)
        {
            KnownDataPoint nextDataPoint = currentDataPointsInd[mesaurmentInd] + 1 < knownDataPoints.Count
                ? knownDataPoints[currentDataPointsInd[mesaurmentInd] + 1]
                : null;

            if (nextDataPoint != null && nextDataPoint.TimeStamp <= rawEntity.TimeStamp)
            {
                currentDataPointsInd[mesaurmentInd]++;
            }

            double rawMesaurment = rawEntity.GetMesaurment(mesaurmentInd);
            if (rawMesaurment >= 0)
            {
                pmaEntity.SetMesaurment(mesaurmentInd, rawMesaurment);
            }
            else
            {
                SetInterpolatedValue(pmaEntity, mesaurmentInd, knownDataPoints[currentDataPointsInd[mesaurmentInd]]);
            }
        }

        private Dictionary<int, List<KnownDataPoint>> BuildKnownPointsLists(List<PmaRawEntity> rawList)
        {
            Dictionary<int, List<KnownDataPoint>> mesaurmentsKnownDataPoints =
                new Dictionary<int, List<KnownDataPoint>>(PmaRawEntity.NumOfMesaurments);
            for (int mesaurmentInd = 0; mesaurmentInd < PmaRawEntity.NumOfMesaurments; mesaurmentInd++)
            {
                mesaurmentsKnownDataPoints.Add(mesaurmentInd, new List<KnownDataPoint>());
            }
            foreach (var rawEntity in rawList)
            {
                for (int mesaurmentInd = 0; mesaurmentInd < PmaRawEntity.NumOfMesaurments; mesaurmentInd++)
                {
                    double mesaurmentVal = rawEntity.GetMesaurment(mesaurmentInd);
                    if (mesaurmentVal >= 0)
                    {
                        if (mesaurmentsKnownDataPoints[mesaurmentInd].Any())
                        {
                            Debug.Assert(
                                !mesaurmentsKnownDataPoints[mesaurmentInd].LastOrDefault()
                                    .TimeStamp.Equals(rawEntity.TimeStamp),
                                string.Format(
                                    "Found a duplicate value during interpolations. mesaurmentInd = {0}. Timestamp = {1}",
                                    mesaurmentInd, rawEntity.TimeStamp));
                        }
                        AddKnownPoint(mesaurmentsKnownDataPoints[mesaurmentInd], rawEntity.TimeStamp, mesaurmentVal);
                    }
                }
            }
            foreach (var knownDataPoints in mesaurmentsKnownDataPoints)
            {
                if (!knownDataPoints.Value.Any())
                {
                    KnownDataPoint defaultDataPoint = new KnownDataPoint(rawList[knownDataPoints.Key].TimeStamp, 0);
                    knownDataPoints.Value.Add(defaultDataPoint);
                }
            }

            return mesaurmentsKnownDataPoints;
        }

        private void AddKnownPoint(List<KnownDataPoint> knownDataPoints, DateTime timeStamp, double value)
        {
            knownDataPoints.Add(new KnownDataPoint(timeStamp, value));
            if (knownDataPoints.Count > 1)
            {
                KnownDataPoint previousPoint = knownDataPoints[knownDataPoints.Count - 2];
                double incrementBy = (value - previousPoint.Value) /
                                                   (timeStamp.Subtract(previousPoint.TimeStamp).TotalSeconds);
                previousPoint.NextPointIncrement = incrementBy;
                knownDataPoints.Last().NextPointIncrement = incrementBy;
            }
        }

        private bool HasTimeGap(PmaRawEntity firstEntity, PmaRawEntity secondEntity)
        {
            if (secondEntity.TimeStamp.TimeOfDay.TotalSeconds - firstEntity.TimeStamp.TimeOfDay.TotalSeconds > 1)
            {
                return true;
            }
            return false;
        }
        private void FillTimeGaps(DateTime firstTime, DateTime lastTime, Dictionary<int, List<KnownDataPoint>> knownDataPoints, Dictionary<int, int> currentKnownDataPointsInd, List<PmaRawEntity> interpolatedList)
        {
            DateTime currentTime = firstTime.AddSeconds(1);
            while (currentTime <= lastTime)
            {
                PmaRawEntity interpolatedEntity = new PmaRawEntity();
                interpolatedEntity.TimeStamp = currentTime;
                for (int mesaurmentInd = 0; mesaurmentInd < PmaRawEntity.NumOfMesaurments; mesaurmentInd++)
                {
                    KnownDataPoint knownData = knownDataPoints[mesaurmentInd][currentKnownDataPointsInd[mesaurmentInd]];
                    interpolatedEntity.SetMesaurment(mesaurmentInd,
                        interpolatedList.Last().GetMesaurment(mesaurmentInd) +
                        knownData.NextPointIncrement);
                }
                interpolatedList.Add(interpolatedEntity);
                currentTime = currentTime.AddSeconds(1);
            }
        }

        private void SetInterpolatedValue(PmaRawEntity pmaEntity, int mesaurmentInd, KnownDataPoint knownDataPoint)
        {
            double timeDifference = pmaEntity.TimeStamp.TimeOfDay.TotalSeconds -
                                 knownDataPoint.TimeStamp.TimeOfDay.TotalSeconds;
            pmaEntity.SetMesaurment(mesaurmentInd, knownDataPoint.Value + knownDataPoint.NextPointIncrement * timeDifference);
        }

        public List<PmaRawEntity> MergeLists(List<PmaRawEntity> protectedPmaRawEntities,
            List<PmaRawEntity> recoveryPmaRawEntities)
        {
            ClearCpuPerc(protectedPmaRawEntities, true);
            ClearCpuPerc(recoveryPmaRawEntities, false);

            List<PmaRawEntity> mergedList = new List<PmaRawEntity>();
            List<PmaRawEntity> earliestList;
            List<PmaRawEntity> laterList;
            bool isEarliestProtected;
            if (protectedPmaRawEntities[0].TimeStamp <= recoveryPmaRawEntities[0].TimeStamp)
            {
                earliestList = protectedPmaRawEntities;
                laterList = recoveryPmaRawEntities;
                isEarliestProtected = true;
            }
            else
            {
                earliestList = recoveryPmaRawEntities;
                laterList = protectedPmaRawEntities;
                isEarliestProtected = false;
            }
            int earliestListEqualIndex = 0;
            while (earliestListEqualIndex < earliestList.Count &&
                earliestList[earliestListEqualIndex].TimeStamp < laterList[0].TimeStamp)
            {
                //mergedList.Add(earliestList[earliestListEqualIndex++]);
                earliestListEqualIndex++;
            }

            int latestListEqualIndex = 0;
            while (earliestListEqualIndex < earliestList.Count && latestListEqualIndex < laterList.Count)
            {
                if (isEarliestProtected)
                {
                    mergedList.Add(MergePmaRawEntities(earliestList[earliestListEqualIndex++], laterList[latestListEqualIndex++]));
                }
                else
                {
                    mergedList.Add(MergePmaRawEntities(laterList[latestListEqualIndex++], earliestList[earliestListEqualIndex++]));
                }
            }
            //List<PmaRawEntity> latestList = null;
            //int firstEntityToAddIndex = 0;
            //if (earliestListEqualIndex < earliestList.Count)
            //{s
            //    latestList = earliestList
            //    firstEntityToAddIndex = earliestListEqualIndex;
            //}
            //else if (latestListEqualIndex < laterList.Count)
            //{
            //    latestList = laterList;
            //    firstEntityToAddIndex = latestListEqualIndex;
            //}
            //if (latestList != null)
            //{
            //    MergeLatestListTail(latestList, mergedList, firstEntityToAddIndex);
            //}

            return mergedList;
        }

        private void ClearCpuPerc(List<PmaRawEntity> pmaRawEntities, bool isProtectedList)
        {
            if (isProtectedList)
            {
                foreach (var pmaRawEntity in pmaRawEntities)
                {
                    pmaRawEntity.RecoveryCpuPerc = 0;
                }
            }
            else
            {
                foreach (var pmaRawEntity in pmaRawEntities)
                {
                    pmaRawEntity.ProtectedCpuPerc = 0;
                }
            }
        }

        private PmaRawEntity MergePmaRawEntities(PmaRawEntity protectedEntity, PmaRawEntity recoveryEntity)
        {
            PmaRawEntity mergedEntity = new PmaRawEntity
            {
                TimeStamp = protectedEntity.TimeStamp,
                ProtectedVolumeWriteRateMBs = protectedEntity.ProtectedVolumeWriteRateMBs,
                ProtectedVolumeCompressedWriteRateMBs = protectedEntity.ProtectedVolumeCompressedWriteRateMBs,
                ProtectedCpuPerc = protectedEntity.ProtectedCpuPerc,
                ProtectedVraBufferUsagePerc = protectedEntity.ProtectedVraBufferUsagePerc,
                ProtectedTcpBufferUsagePerc = protectedEntity.ProtectedTcpBufferUsagePerc,
                NetworkOutgoingRateMBs = protectedEntity.NetworkOutgoingRateMBs,
                RecoveryTcpBufferUsagePerc = recoveryEntity.RecoveryTcpBufferUsagePerc,
                RecoveryCpuPerc = recoveryEntity.RecoveryCpuPerc,
                RecoveryVraBufferUsagePerc = recoveryEntity.RecoveryVraBufferUsagePerc,
                HardeningRateMBs = recoveryEntity.HardeningRateMBs,
                ApplyRateMBs = recoveryEntity.ApplyRateMBs
            };
            return mergedEntity;
        }

        private void MergeLatestListTail(List<PmaRawEntity> latestList, List<PmaRawEntity> mergedList, int firstEntityToAddIndex)
        {
            for (int entityToAddIndex = firstEntityToAddIndex; entityToAddIndex < latestList.Count; entityToAddIndex++)
            {
                mergedList.Add(latestList[entityToAddIndex++]);
            }
        }
    }
}
