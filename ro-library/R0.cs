using System;
using System.Collections.Generic;
using System.Linq;

namespace CloseAssociate.R0_library
{
    public static class R0
    {
        public static List<double> CalculateReffOfT(double gamma, double c, List<int> i_t)
        {
            /*
            Pad the data with three extra elements at the beginning and end, to get “paddeddata”. The method
            chosen for padding at the beginning is not important, since the early part of the data series, where the
            infection numbers are low, is unreliable anyway. At the end of the data series, linear padding seems a
            reasonable choice.In any event, the choice of padding will affect Reff only at the final two times.
            */
            Enumerable.Range(1, 3)
                    .ToList().ForEach(x => i_t.Insert(0, 1));

            Enumerable.Range(1, 3)
                .ToList()
                .ForEach(x =>
                    i_t.Add(i_t[i_t.Count-1] + (i_t[i_t.Count-1] - i_t[i_t.Count-2]))
                );

            /*
            Then we average the padded data over each point and its two neighbors to the left and right(with
            stencil 1,2,3,2,1), to smooth out fluctuations locally, giving “avpaddeddata”. (One might wonder why
            we did not choose shorter variable names.) This averaging procedure cannot be applied at the first two
            entries or last two entries, because doing so would overshoot the endpoints, and so the length of the
            data series gets reduced by 2 at each end.
            */
            var avpaddeddata = new List<double>();
            Enumerable.Range(2, i_t.Count - 4)
                .ToList()
                .ForEach(x =>
                {
                    avpaddeddata.Add((double)
                        (i_t[x-2] + 2 * i_t[x-1] + 3 * i_t[x] + 2 * i_t[x+1] + i_t[x+2]) / 9
                    );
                });

            // Take the natural logarithm of the averaged, padded data.
            var logdata = new List<double>();
            Enumerable.Range(0, avpaddeddata.Count)
                .ToList()
                .ForEach(x => logdata.Add(Math.Log(avpaddeddata[x])));


            /*
            Take the first and second difference quotients, which reduces the length by another 1 at each end,
            hence getting back to the original length.
            Note: in order to export the averaged padded data from this file, the first and last entries should be
            dropped, in order to reduce to having the same number of entries as in the original data.
            */
            var logdatadiff1 = new List<double>();
            var logdatadiff1sq = new List<double>();
            Enumerable.Range(1, logdata.Count - 2)
                .ToList()
                .ForEach(x =>
                {
                    double diff = (logdata[x+1] - logdata[x-1]) / 2;
                    logdatadiff1.Add(diff);
                    logdatadiff1sq.Add(diff * diff);
                });

            var logdatadiff2 = new List<double>();
            Enumerable.Range(1, logdatadiff1sq.Count)
                .ToList()
                .ForEach(x =>
                {
                    logdatadiff2.Add(
                        logdata[x+1] - 2 * logdata[x] + logdata[x-1]
                    );
                });

            /*
             * Now calculate Reff from equations (9) and (10) of the technical report
             */
            var Reff = new List<double>();
            Enumerable.Range(0, logdatadiff1.Count)
                .ToList()
                .ForEach(x => Reff.Add((1 / gamma) * ((logdatadiff2[x] + logdatadiff1sq[x] + (c + 2*gamma) * logdatadiff1[x] + gamma * (c + gamma)) / (logdatadiff1[x] + c + gamma))));


            return Reff;

        }

        public static List<int> Create_I_t_from_DI_tByDt(List<int> dI_tBydt)
        {
            var result = new List<int>();

            Enumerable.Range(0, dI_tBydt.Count)
                .ToList()
                .ForEach(x => result.Add(
                                Enumerable.Range(Math.Max(0, x - 14), Math.Min(x, 14) + 1).ToList().Select(y => dI_tBydt[y]).Sum()
                            ));

            return result;
        }
    }
}
