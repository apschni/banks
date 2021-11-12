namespace Banks
{
    public class Client
    {
        private string _name;
        private string _address;
        private string _passportData;

        public Client(string name, string address = default, string passportData = default)
        {
            _name = name;
            _address = address;
            _passportData = passportData;
        }

        public bool GetStatus()
        {
            return _address != default && _passportData != default;
        }

        public void SetAddress(string address)
        {
            _address = address;
        }

        public void SetPassportData(string passportData)
        {
            _passportData = passportData;
        }
    }
}