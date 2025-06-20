using System;
using System.Threading.Tasks;
using Prometheus;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

class Program
{
    static void Main(string[] args)
    {
        var requestCounter = Metrics.CreateCounter("myservice_requuest_total", "Total number of request");
        var queueGauge = Metrics.CreateGauge("rabbit_queue_size", "Current queue size");

        var builder = WebApplication.CreateBuilder(args);
        var app = builder.Build();

        app.UseMetricServer();

        var rnd = new Random();
        _ = Task.Run(async () =>
        {
            while (true)
            {
                requestCounter.Inc(rnd.Next(1, 5));
                queueGauge.Set(rnd.Next(0, 25));
                await Task.Delay(5000);
            }
        });

        app.Run("http://0.0.0.0:8000");
    }
}