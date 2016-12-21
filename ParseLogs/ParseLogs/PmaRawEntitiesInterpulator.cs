
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
            return protectedPmaRawEntities;
        }
    }
}
