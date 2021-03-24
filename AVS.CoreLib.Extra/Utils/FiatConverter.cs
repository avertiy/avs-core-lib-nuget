namespace AVS.CoreLib.Utils
{
    //no usages not implemented
    class FiatConverter
    {
        const string ApiUrl = "http://data.fixer.io/api/latest?access_key=";
        private string _accessKey;

        public FiatConverter(string accessKey = "bd14a48efb7e3ffe9d09b45d8b3c0883")
        {
            _accessKey = accessKey;
        }

        //public void FetchRates()
        //{
        //    //http://data.fixer.io/api/latest?access_key=bd14a48efb7e3ffe9d09b45d8b3c0883&symbols=USD,UAH
        //    var url = ApiUrl + _accessKey + "&symbols=USD,UAH,BTC";
        //    //use flurl nuget package
        //    var json = await url.GetStringAsync().ConfigureAwait(false);
        //}
    }
}