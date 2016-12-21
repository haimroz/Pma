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

            newList.Add(rawList.First());

            for (current = current.AddSeconds(1); current <= finish; current = current.AddSeconds(1))
            {
                PmaRawEntity entity = new PmaRawEntity(newList.Last());
                IEnumerable<PmaRawEntity> matches = rawList.Where(obj => obj.TimeStamp == current);
                if (matches.Count() > 0)
                {
                    PmaRawEntity match = matches.First();
                    if (match.ProtectedVolumeWriteRateMbs != -1)
                        entity.ProtectedVolumeWriteRateMbs = match.ProtectedVolumeWriteRateMbs;
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
                    if (match.JournalSizeMB != -1)
                        entity.JournalSizeMB = match.JournalSizeMB;
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
            while (earliestList[earliestListEqualIndex].TimeStamp < laterList[0].TimeStamp)
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
                    mergedList.Add(MergePmaRawEntities(laterList[earliestListEqualIndex++], earliestList[latestListEqualIndex++]));
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
                ProtectedVolumeWriteRateMbs = protectedEntity.ProtectedVolumeWriteRateMbs,
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
