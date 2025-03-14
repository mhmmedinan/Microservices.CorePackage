using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;
using OpenTelemetry.Trace;
using OpenTelemetry.Resources;
using OpenTelemetry.Context.Propagation;
using Core.Tracing.Domain;
using Core.Tracing.Mediator;
using Core.Tracing.Transports;
using OpenTelemetry;
using System.Reactive;

namespace Core.Tracing;

public static class OTelExtensions
{
    public static IServiceCollection AddInMemoryMessagingTelemetry(this IServiceCollection services)
    {
        DiagnosticListener.AllListeners.Subscribe(listener =>
        {
            if (listener.Name == InMemoryTransportListener.InBoundName ||
                listener.Name == InMemoryTransportListener.OutBoundName)
            {
                listener.SubscribeWithAdapter(new InMemoryTransportListener());
            }
        });

        return services;
    }


    public static IServiceCollection AddOTelIntegration(
           this IServiceCollection services,
           IConfiguration configuration,
           Action<OpenTelemetryOptions> openTelemetryOptions = null)
    {
        var options = configuration.GetSection(nameof(OpenTelemetryOptions)).Get<OpenTelemetryOptions>();
        services.AddOptions<OpenTelemetryOptions>().Bind(configuration.GetSection(nameof(OpenTelemetryOptions)))
            .ValidateDataAnnotations();

        services.AddOpenTelemetry().WithTracing(builder =>
        {
            openTelemetryOptions?.Invoke(options);
            ConfigureSampler(builder, options);
            ConfigureInstrumentation(builder, options);
            ConfigureExporters(configuration, builder, options);

            if (options.Services is not null)
            {
                foreach (var service in options.Services)
                {
                    builder.SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(service)); // "ECommerce.Services.ECommerce.Services.Identity.Api"
                }
            }
        });

        return services;
    }

    private static void ConfigureExporters(
           IConfiguration configuration,
           TracerProviderBuilder builder,
           OpenTelemetryOptions options)
    {
        builder.AddZipkinExporter(o =>
        {
            configuration.Bind("OtelZipkin", o);
            o.Endpoint = options.ZipkinExporterOptions.Endpoint; // "http://localhost:9411/api/v2/spans"
        });
    }

    private static void ConfigureSampler(TracerProviderBuilder builder, OpenTelemetryOptions options)
    {
        if (options.AlwaysOnSampler)
        {
            builder.SetSampler(new AlwaysOnSampler());
            builder.SetSampler(new AlwaysOnSampler());
        }
    }

    private static void ConfigureInstrumentation(
            TracerProviderBuilder builder,
            OpenTelemetryOptions options)
    {
        Sdk.SetDefaultTextMapPropagator(GetPropagator(options));
        builder.AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation()
            .AddSource(OTelMediatROptions.OTelMediatRName)
            .AddSource(OTelDomainOptions.OTelEventHandlerName)
            .AddSource(OTelTransportOptions.InMemoryConsumerActivityName)
            .AddSource(OTelTransportOptions.InMemoryProducerActivityName);
    }

    private static TextMapPropagator GetPropagator(OpenTelemetryOptions openTelemetryOptions)
    {
        var propagators = new List<TextMapPropagator>
            {
                new TraceContextPropagator(),
                new BaggagePropagator(),
            };

        if (openTelemetryOptions.Istio)
        {
            propagators.Add(new B3Propagator());
        }

        return new CompositeTextMapPropagator(propagators);
    }




    public static IDisposable Subscribe<T>(this IObservable<T> source, Action<T> onNext)
    {
        if (source == null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        if (onNext == null)
        {
            throw new ArgumentNullException(nameof(onNext));
        }

        //
        // [OK] Use of unsafe Subscribe: non-pretentious constructor for an observer; this overload is not to be used internally.
        //
        return source.Subscribe/*Unsafe*/(new AnonymousObserver<T>(onNext));
    }
}
