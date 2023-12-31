namespace Paradigm.Server
{
    using System;        

    using Wangkanai.Detection.Services;

    using Paradigm.Contract.Model;
    using Paradigm.Contract.Interface;

    public sealed class HttpDeviceProfiler : IDeviceProfiler
    {
        private const string EncryptionSalt = "UqA24EjKxWOUHpXqBvjd8mWhJLa7dxVPbew/7JfUejI=";
        private readonly ICryptoService crypto;
        private readonly IDetectionService detection;

        public HttpDeviceProfiler(ICryptoService crypto, IDetectionService detection)
        {
            this.crypto = crypto;
            this.detection = detection;
        }

        public string DeriveFingerprint(IUser user)
        {
            if (user == null)
                throw new ArgumentNullException($"{nameof(user)}");

            string browserName = detection?.Browser.Name.ToString();
            string browserVersion = detection?.Browser.Version.ToString();
            string browserType = detection?.Engine.Name.ToString();
            string deviceType = detection?.Device.Type.ToString();

            var blocks = new string[]
            {
                browserName, browserVersion, browserType, deviceType, user.Email
            };

            string data = string.Empty;

            foreach (string block in blocks)
            {
                if (!string.IsNullOrWhiteSpace(block))
                    data = data + block.Trim();
            }

            return this.crypto.CreateKey(EncryptionSalt, data);
        }
    }
}
