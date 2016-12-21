using System.Web.Http;
using PmaEntities;
using Ppa.Services;

namespace Ppa.Controllers
{
    public class PmaController : ApiController
    {
        private readonly PmaRepository m_pmaRepository;

        public PmaController()
        {
            m_pmaRepository = new PmaRepository();
        }

        public PmaRawEntity[] Get()
        {
            return m_pmaRepository.GetAll();
        }
    }
}
