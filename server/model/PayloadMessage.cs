namespace Paradigm.Server.Model
{
    using System;
    using Newtonsoft.Json;

    public class PayloadMessage
    {
        public PayloadMessage()
        {
            this.MessageType = PayloadMessageType.Success;
            this.Text = "";
            this.Title = "";
        }
        public string Text { get; set; }
        public string Title { get; set; }
        public string MessageTypeId
        {
            get
            {
                return Enum.GetNames(typeof(PayloadMessageType)).GetValue((int)this.MessageType).ToString();
            }
            set
            {
                this.MessageType = (PayloadMessageType)Enum.Parse(typeof(PayloadMessageType), value);
            }
        }

        [JsonIgnore]
        public PayloadMessageType MessageType { get; set; }
    }
}
