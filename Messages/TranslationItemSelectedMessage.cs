using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace D2MTranslator.Messages
{
    public class TranslationItemSelectedMessage
    {
        public int Id { get; }

        public TranslationItemSelectedMessage(int id)
        {
            Id = id;
        }
        // TODO: Create ViewModel and Sync with Configuration Service
    }
}
