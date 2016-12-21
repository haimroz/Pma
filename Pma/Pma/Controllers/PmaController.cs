using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using PmaEntities;
using Ppa.Models;
using Ppa.Services;

namespace Ppa.Controllers
{
    public class PmaController : ApiController
    {
        //private ContactRepository contactRepository;
        private PmaRepository m_pmaRepository;

        public PmaController()
        {
            //this.contactRepository = new ContactRepository();
            m_pmaRepository = new PmaRepository();
        }

        public PmaRawEntity[] Get()
        {
            return m_pmaRepository.GetAll();
        }

        /*public Contact[] Get()
        {
            return contactRepository.GetAllContacts();
        }*/

        /*public Contact[] Get()
        {
            return new Contact[]
            {
                new Contact
                {
                    Id = 1,
                    Name = "Glenn Block"
                },
                new Contact
                {
                Id = 2,
                Name = "Dan Roth"
            }
            };
        }*/

        /*public string[] Get()
        {
            return new string[]
            {
             "Hello",
             "World"
            };
        }*/
    }
}
