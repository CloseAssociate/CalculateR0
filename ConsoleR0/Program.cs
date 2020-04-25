using CloseAssociate.R0_library;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

namespace CloseAssociate.consoleR0
{
    class Program
    {
        static void Main(string[] args)
        {
            const double gamma = (double)1 / 15;
            const double c = (double)1 / 15;

            /*
             * NZ Ministry of health (confirmed and probables by day) 
             * https://www.health.govt.nz/our-work/diseases-and-conditions/covid-19-novel-coronavirus/covid-19-current-situation/covid-19-current-cases/covid-19-current-cases-details 
             * downloaded in excel spreadsheet, see NZData-MOH-downloaded-25apr20, worksheet Error Checks 
             */
            List<int> dI_tBydt = new int[] { 1, 0, 0, 0, 0, 1, 0, 2, 2, 1, 0, 0, 0, 0, 0, 1, 1, 1, 1, 7, 12, 5, 18, 11, 24, 45, 42, 59, 83, 77, 76, 70, 76, 51, 77, 74, 82, 74, 77, 70, 53, 37, 28, 45, 27, 25, 20, 11, 14, 11, 17, 12, 10, 9, 3, 4, 4, 5 }.ToList();

            // Now calculate I_t from dI_t/dt
            var I_t = R0.Create_I_t_from_DI_tByDt(dI_tBydt);

            // Now calculate Reff 
            var Reff = R0.CalculateReffOfT(gamma, c, I_t);

             // 26 Feb 2020 is the first date of a recorded infection in New Zealand time series
            var startDate = new DateTime(2020, 2, 26);
            
            // Now display the results and Copyright notice
            Enumerable.Range(0, Reff.Count)
                .ToList()
                .ForEach(x =>
                {
                    Console.WriteLine($"day({x})\t {startDate.AddDays(x).ToShortDateString()} \tReff(t) = {Reff[x].ToString("0.0")}" );
                });

            Console.WriteLine("===================================================");
            Console.WriteLine("Copyright 2020 CloseAssociate, all rights reserved.");
            Console.WriteLine("===================================================");
            Console.Read();
        }
    }
}

