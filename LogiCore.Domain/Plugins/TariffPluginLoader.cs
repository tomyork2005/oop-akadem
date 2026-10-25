using System.Reflection;
using LogiCore.Domain.Pricing;

namespace LogiCore.Domain.Plugins;

public class TariffPluginLoader
{
    public const string PluginSearchPattern = "*.dll";

    public IReadOnlyCollection<string> LoadFromDirectory(string directory, TariffRegistry registry)
    {
        var log = new List<string>();
        if (!Directory.Exists(directory))
        {
            log.Add("папка плагинов не найдена: " + directory);
            return log;
        }

        foreach (var file in Directory.GetFiles(directory, PluginSearchPattern))
        {
            Assembly assembly;
            try
            {
                assembly = Assembly.LoadFrom(file);
            }
            catch (Exception ex) when (ex is BadImageFormatException || ex is IOException)
            {
                log.Add($"{Path.GetFileName(file)}: не удалось загрузить ({ex.Message})");
                continue;
            }

            foreach (var type in assembly.GetTypes())
            {
                var attribute = type.GetCustomAttribute<TariffStrategyAttribute>();
                if (attribute == null || type.IsAbstract || !typeof(ITariffStrategy).IsAssignableFrom(type))
                {
                    continue;
                }
                var tariff = Activator.CreateInstance(type) as ITariffStrategy;
                if (tariff == null)
                {
                    continue;
                }
                if (registry.Register(attribute.Name, tariff))
                {
                    log.Add($"загружен тариф {attribute.Name} из {Path.GetFileName(file)}");
                }
                else
                {
                    log.Add($"тариф {attribute.Name} уже есть, пропущен");
                }
            }
        }
        return log;
    }
}
