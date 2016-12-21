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
            return protectedPmaRawEntities;
        }
    }
}
