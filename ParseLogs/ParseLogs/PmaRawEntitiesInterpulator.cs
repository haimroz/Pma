using System.Collections.Generic;
using System;
using System.Linq;
using PmaEntities;

namespace ParseLogs
{
    public class PmaRawEntitiesInterpulator
    {
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
            if (newItem.HardeningRateMBs == -1)
                newItem.HardeningRateMBs = 0;
            else//fixing logparser/zvm bug that treats the LBs as Bytes
                newItem.HardeningRateMBs *= 512;

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
                        entity.HardeningRateMBs = match.HardeningRateMBs * 512; //fixing logparser/zvm bug that treats the LBs as Bytes
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
