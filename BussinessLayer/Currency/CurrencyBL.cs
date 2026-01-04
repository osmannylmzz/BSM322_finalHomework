using System.Collections.Generic;
using System.Threading.Tasks;
using DataLayer.Currency;
using EntityLayer.Models;

namespace BusinessLayer.Currency
{
    public static class CurrencyBL
    {
        
        /// Döviz/altýn kurlarýný DataLayer üzerinden çeker.
 
        public static Task<List<EntityLayer.Models.Currency>> GetCurrenciesAsync()
            => CurrencyDL.GetCurrenciesAsync();
    }
}