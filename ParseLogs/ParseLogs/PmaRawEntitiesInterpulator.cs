
using System.Collections.Generic;
using PmaEntities;

namespace ParseLogs
{
    public class PmaRawEntitiesInterpulator
    {
        public List<PmaRawEntity> ProcessRawList(List<PmaRawEntity> rawList)
        {
            return rawList;
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
