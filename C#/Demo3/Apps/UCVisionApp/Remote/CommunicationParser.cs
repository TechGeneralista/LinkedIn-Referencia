using Common;
using Common.Communication;
using Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;


namespace UCVisionApp.Remote
{
    public class CommunicationParser : ICanCommunication
    {
        readonly ICanRemote canRemote;
        readonly object lockObject = new object();


        public CommunicationParser(ICanRemote canRemote)
        {
            if (canRemote.IsNull())
                throw new ArgumentNullException(nameof(canRemote));

            this.canRemote = canRemote;
        }

        public string Communication(string receivedData)
        {
            lock(lockObject)
            {
                string[] splitted = receivedData.Split('/');
                string response = null;

                if (splitted.Length > 1)
                {
                    List<string> list = new List<string>(splitted);
                    string command = list.Last();
                    list.Remove(list.Last());
                    response = canRemote.Remote(command, list.ToArray());
                }

                if (string.IsNullOrEmpty(response))
                    response = Responses.IdNotExist.ToString();

                return response;
            }
        }
    }
}
