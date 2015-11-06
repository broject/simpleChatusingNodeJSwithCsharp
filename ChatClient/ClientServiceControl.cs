using System;
using System.Collections.Generic;
using System.Text;

namespace ChatClient
{
    interface ClientServiceControl
    {
        void ShowPrompt(Prompt prompt);
        void ShowMessage(Message message);
    }
}
