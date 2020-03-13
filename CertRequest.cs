namespace UwebServerCert
{
    class CertRequest
    {
        public CertRequest(string account, string[] domains)
        {
            Account = account;
            Domains = domains;
        }
        public string Account { get; }
        public string[] Domains { get; }
    }
}