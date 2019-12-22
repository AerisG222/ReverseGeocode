using System;


namespace ReverseGeocode.Data
{
    public abstract class Database
    {
        string _connString;


        public Database(string connString)
        {
            if(string.IsNullOrEmpty(connString))
            {
                throw new ArgumentNullException(nameof(connString));
            }

            _connString = connString;
        }
    }
}
