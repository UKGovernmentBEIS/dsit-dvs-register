using DVSRegister.CommonUtility;

namespace DVSRegister.BusinessLogic.Extensions
{
    public static class ExpiredDateValidator
    {

        public static bool CheckExpired(DateTime? dateTime)
        {
            bool expired = false;
            if (dateTime.HasValue)            
            {
                var daysPassed = (DateTime.Today - dateTime.Value).Days;
                var daysLeft = Constants.URNExpiryDays - daysPassed;
                if (daysLeft <= 0)
                {
                    expired= true;
                }               
            }
            return expired;            

        }

    }
}

