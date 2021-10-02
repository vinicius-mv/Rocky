using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Globalization;

// REF: https://stackoverflow.com/questions/40442444/set-cultureinfo-in-asp-net-core-to-have-a-as-currencydecimalseparator-instead - benraay
namespace Rocky.Utility
{
    public static class LocalizationExtensions
    {
        public static void ConfigureLocalizationMiddleware(this IApplicationBuilder app)
        {
            var ci = new CultureInfo("en-US");
            ci.NumberFormat.NumberDecimalSeparator = ".";
            ci.NumberFormat.CurrencyDecimalSeparator = ".";

            // Configure the Localization middleware
            app.UseRequestLocalization(new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture(ci),
                SupportedCultures = new List<CultureInfo>
                {
                    ci,
                },
                SupportedUICultures = new List<CultureInfo>
                {
                    ci,
                }
            });
        }
    }
}
