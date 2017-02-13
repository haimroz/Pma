using System.Collections.Generic;
using System;
using System.ComponentModel;
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
            public double? NextPointIncrement { get; set; }

            public KnownDataPoint(DateTime timeStamp, double value)
            {
                TimeStamp = timeStamp;
                Value = value;
                NextPointIncrement = null;
            }
        }

        public List<PmaRawEntity> BuildInterpolatedList(List<PmaRawEntity> rawList)
        {
            Dictionary<int, List<KnownDataPoint>> mesaurmentsKnownDataPoints = BuildKnownPointsLists(rawList);
            Dictionary<int, KnownDataPoint> knownDataPoints =
                new Dictionary<int, KnownDataPoint>(mesaurmentsKnownDataPoints.Count);
            for (int mesaurmentInd = 0; mesaurmentInd < mesaurmentsKnownDataPoints.Count; mesaurmentInd++)
            {
                knownDataPoints[mesaurmentInd] = mesaurmentsKnownDataPoints[mesaurmentInd].First();
            }

            List<PmaRawEntity> interpolatedList = new List<PmaRawEntity>();
            foreach (var rawEntity in rawList)
            {
                if (interpolatedList.Count > 0 && HasTimeGap(interpolatedList.Last(), rawEntity))
                {
                    FillTimeGaps(interpolatedList.Last().TimeStamp, rawEntity.TimeStamp, knownDataPoints, interpolatedList);
                }
                PmaRawEntity pmaEntity = new PmaRawEntity();
                for (int mesaurmentInd = 0; mesaurmentInd < PmaRawEntity.NumOfMesaurments; mesaurmentInd++)
                {
                    double rawMesaurment = rawEntity.GetMesaurment(mesaurmentInd);
                    if (rawMesaurment >= 0)
                    {
                        pmaEntity.SetMesaurment(mesaurmentInd, rawMesaurment);
                    }
                    else
                    {
                        pmaEntity.SetMesaurment(mesaurmentInd,
                            interpolatedList.Last().GetMesaurment(mesaurmentInd) +
                            knownDataPoints[mesaurmentInd].NextPointIncrement.Value);
                    }
                }
                interpolatedList.Add(pmaEntity);
            }
            return interpolatedList;
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
                        AddKnownPoint(mesaurmentsKnownDataPoints[mesaurmentInd], rawEntity.TimeStamp, mesaurmentVal);
                    }
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
                previousPoint.NextPointIncrement = (value - previousPoint.Value)/
                                                   (timeStamp.Subtract(previousPoint.TimeStamp).TotalSeconds);
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
        private void FillTimeGaps(DateTime firstTime, DateTime lastTime, Dictionary<int, KnownDataPoint> knownDataPoints, List<PmaRawEntity> interpolatedList)
        {
            DateTime currentTime = firstTime;
            while (currentTime <= lastTime)
            {
                PmaRawEntity interpolatedEntity = new PmaRawEntity();
                interpolatedEntity.TimeStamp = currentTime;
                for (int mesaurmentInd = 0; mesaurmentInd < PmaRawEntity.NumOfMesaurments; mesaurmentInd++)
                {
                    KnownDataPoint knownData = knownDataPoints[mesaurmentInd];
                    interpolatedEntity.SetMesaurment(mesaurmentInd,
                        knownData.Value + interpolatedList.Last().GetMesaurment(mesaurmentInd) +
                        knownData.NextPointIncrement.Value);
                }
                interpolatedList.Add(interpolatedEntity);
                currentTime = currentTime.AddSeconds(1);
            }
        }

        public List<PmaRawEntity> ProcessRawList(List<PmaRawEntity> rawList)
        {
            List<PmaRawEntity> newList = new List<PmaRawEntity>();
            if (rawList.Count == 0)
                return null;
            DateTime start = rawList.First().TimeStamp;
            DateTime finish = rawList.Last().TimeStamp;

            DateTime current = start;

            PmaRawEntity newItem = new PmaRawEntity(rawList.First());


            if (newItem.ProtectedIOsInDriverMBs == -1)
                newItem.ProtectedIOsInDriverMBs = 0;
            if (newItem.ProtectedVolumeWriteRateMBs == -1)
                newItem.ProtectedVolumeWriteRateMBs = 0;
            if (newItem.ProtectedVolumeCompressedWriteRateMBs == -1)
                newItem.ProtectedVolumeCompressedWriteRateMBs = 0;
            if (newItem.ProtectedCpuPerc == -1)
                newItem.ProtectedCpuPerc = 0;
            if (newItem.ProtectedVraBufferUsagePerc == -1)
                newItem.ProtectedVraBufferUsagePerc = 0;
            if (newItem.ProtectedTcpBufferUsagePerc == -1)
                newItem.ProtectedTcpBufferUsagePerc = 0;
            if (newItem.NetworkOutgoingRateMBs == -1)
                newItem.NetworkOutgoingRateMBs = 0;
            if (newItem.RecoveryTcpBufferUsagePerc == -1)
                newItem.RecoveryTcpBufferUsagePerc = 0;
            if (newItem.RecoveryCpuPerc == -1)
                newItem.RecoveryCpuPerc = 0;
            if (newItem.RecoveryVraBufferUsagePerc == -1)
                newItem.RecoveryVraBufferUsagePerc = 0;
            if (newItem.ApplyRateMBs == -1)
                newItem.ApplyRateMBs = 0;

            newList.Add(newItem);

            for (current = current.AddSeconds(1); current <= finish; current = current.AddSeconds(1))
            {
                PmaRawEntity entity = new PmaRawEntity(newList.Last());
                IEnumerable<PmaRawEntity> matches = rawList.Where(obj => obj.TimeStamp == current);
                entity.TimeStamp = current;
                if (matches.Count() > 0)
                {
                    PmaRawEntity match = matches.First();
                    if (match.ProtectedIOsInDriverMBs != -1)
                        entity.ProtectedIOsInDriverMBs = match.ProtectedIOsInDriverMBs;
                    if (match.ProtectedVolumeWriteRateMBs != -1)
                        entity.ProtectedVolumeWriteRateMBs = match.ProtectedVolumeWriteRateMBs;
                    if (match.ProtectedVolumeCompressedWriteRateMBs != -1)
                        entity.ProtectedVolumeCompressedWriteRateMBs = match.ProtectedVolumeCompressedWriteRateMBs;
                    if (match.ProtectedCpuPerc != -1)
                        entity.ProtectedCpuPerc = match.ProtectedCpuPerc;
                    if (match.ProtectedVraBufferUsagePerc != -1)
                        entity.ProtectedVraBufferUsagePerc = match.ProtectedVraBufferUsagePerc;
                    if (match.ProtectedTcpBufferUsagePerc != -1)
                        entity.ProtectedTcpBufferUsagePerc = match.ProtectedTcpBufferUsagePerc;
                    if (match.NetworkOutgoingRateMBs != -1)
                        entity.NetworkOutgoingRateMBs = match.NetworkOutgoingRateMBs;
                    if (match.RecoveryTcpBufferUsagePerc != -1)
                        entity.RecoveryTcpBufferUsagePerc = match.RecoveryTcpBufferUsagePerc;
                    if (match.RecoveryCpuPerc != -1)
                        entity.RecoveryCpuPerc = match.RecoveryCpuPerc;
                    if (match.RecoveryVraBufferUsagePerc != -1)
                        entity.RecoveryVraBufferUsagePerc = match.RecoveryVraBufferUsagePerc;
                    if (match.HardeningRateMBs != -1)
                        entity.HardeningRateMBs = match.HardeningRateMBs;
                    if (match.ApplyRateMBs != -1)
                        entity.ApplyRateMBs = match.ApplyRateMBs;
                }
                newList.Add(entity);
            }
            return newList;
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
