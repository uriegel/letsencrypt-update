namespace UwebServerCert
{
    class CertRequest
    {
        public CertRequest(string account)
        {
            this.account = account;
        }
        public string account { get; }
    }
}