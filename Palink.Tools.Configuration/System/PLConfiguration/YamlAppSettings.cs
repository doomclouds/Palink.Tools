﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using Palink.Tools.Extensions.PLString;
using YamlDotNet.Core;
using YamlDotNet.RepresentationModel;

namespace Palink.Tools.System.PLConfiguration;

public class YamlAppSettings
{
    /// <summary>
    /// 配置文件的根节点
    /// </summary>
    public static readonly IConfigurationRoot Config;

    /// <summary>
    /// Constructor
    /// </summary>
    static YamlAppSettings()
    {
        // 加载appsettings.yaml，并构建IConfigurationRoot
        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddYamlFile("appsettings.yml", true, true);
        Config = builder.Build();
    }
}

internal static class YamlConfigurationExtensions
{
    internal static IConfigurationBuilder AddYamlFile(this IConfigurationBuilder builder,
        string path)
    {
        return AddYamlFile(builder, null, path, false, false);
    }

    internal static IConfigurationBuilder AddYamlFile(this IConfigurationBuilder builder,
        string path, bool optional)
    {
        return AddYamlFile(builder, null, path, optional, false);
    }

    internal static IConfigurationBuilder AddYamlFile(this IConfigurationBuilder builder,
        string path, bool optional, bool reloadOnChange)
    {
        return AddYamlFile(builder, null, path, optional, reloadOnChange);
    }

    internal static IConfigurationBuilder AddYamlFile(this IConfigurationBuilder builder,
        IFileProvider? provider, string path, bool optional, bool reloadOnChange)
    {
        if (builder == null)
            throw new ArgumentNullException(nameof(builder));

        if (string.IsNullOrEmpty(path))
            throw new ArgumentException("File path must be a non-empty string.",
                nameof(path));

        return builder.AddYamlFile(s =>
        {
            s.FileProvider = provider;
            s.Path = path;
            s.Optional = optional;
            s.ReloadOnChange = reloadOnChange;
            s.ResolveFileProvider();
        });
    }

    internal static IConfigurationBuilder AddYamlFile(this IConfigurationBuilder builder,
        Action<YamlConfigurationSource> configureSource)
    {
        return builder.Add(configureSource);
    }

    internal static Dictionary<string, object> ToDictionary(this IConfiguration section,
        params string[]? sectionsToSkip)
    {
        sectionsToSkip ??= Array.Empty<string>();

        var dic = new Dictionary<string, object>();
        foreach (var value in section.GetChildren())
        {
            if (string.IsNullOrEmpty(value.Key) ||
                sectionsToSkip.Contains(value.Key, StringComparer.OrdinalIgnoreCase))
                continue;

            if (value.Value != null)
                dic[value.Key] = value.Value;

            var subDic = ToDictionary(value);
            if (subDic.Count > 0)
                dic[value.Key] = subDic;
        }

        return dic;
    }
}

internal class YamlConfigurationSource : FileConfigurationSource
{
    public override IConfigurationProvider Build(IConfigurationBuilder builder)
    {
        EnsureDefaults(builder);
        return new YamlConfigurationProvider(this);
    }
}

internal class YamlConfigurationProvider : FileConfigurationProvider
{
    internal YamlConfigurationProvider(YamlConfigurationSource source) : base(source)
    {
    }

    public override void Load(Stream stream)
    {
        var parser = new YamlConfigurationFileParser();
        try
        {
            Data = parser.Parse(stream);
        }
        catch (YamlException ex)
        {
            string errorLine = String.Empty;
            if (stream.CanSeek)
            {
                stream.Seek(0, SeekOrigin.Begin);

                using var streamReader = new StreamReader(stream);
                var fileContent = ReadLines(streamReader);
                errorLine = RetrieveErrorContext(ex, fileContent);
            }

            throw new FormatException(
                "Could not parse the YAML file. " +
                $"Error on line number '{ex.Start.Line}': '{errorLine}'.", ex);
        }
    }

    private static string RetrieveErrorContext(YamlException ex,
        IEnumerable<string> fileContent)
    {
        var possibleLineContent = fileContent.Skip(ex.Start.Line - 1).FirstOrDefault();
        return possibleLineContent ?? string.Empty;
    }

    private static IEnumerable<string> ReadLines(TextReader streamReader)
    {
        string line;
        do
        {
            line = streamReader.ReadLine() ?? string.Empty;
            yield return line;
        } while (line.IsNullOrEmpty());
    }
}

internal class YamlConfigurationFileParser
{
    private readonly Stack<string> _context = new Stack<string>();

    private readonly IDictionary<string, string> _data =
        new SortedDictionary<string, string>(StringComparer.OrdinalIgnoreCase);

    private string? _currentPath;

    internal IDictionary<string, string> Parse(Stream stream)
    {
        _data.Clear();

        var yamlStream = new YamlStream();
        yamlStream.Load(new StreamReader(stream));

        if (!yamlStream.Documents.Any())
            return _data;

        if (yamlStream.Documents[0].RootNode is not YamlMappingNode mappingNode)
            return _data;

        foreach (var nodePair in mappingNode.Children)
        {
            var context = ((YamlScalarNode)nodePair.Key).Value ?? string.Empty;
            VisitYamlNode(context, nodePair.Value);
        }

        return _data;
    }

    private void VisitYamlNode(string context, YamlNode node)
    {
        switch (node)
        {
            case YamlScalarNode scalarNode:
                VisitYamlScalarNode(context, scalarNode);
                break;
            case YamlMappingNode mappingNode:
                VisitYamlMappingNode(context, mappingNode);
                break;
            case YamlSequenceNode sequenceNode:
                VisitYamlSequenceNode(context, sequenceNode);
                break;
            default:
                throw new ArgumentOutOfRangeException(
                    nameof(node),
                    $"Unsupported YAML node type '{node.GetType().Name} was found. " +
                    $"Path '{_currentPath}', line {node.Start.Line} position {node.Start.Column}.");
        }
    }

    private void VisitYamlScalarNode(string context, YamlScalarNode scalarNode)
    {
        EnterContext(context);
        var currentKey = _currentPath ?? string.Empty;

        if (_data.ContainsKey(currentKey))
            throw new FormatException($"A duplicate key '{currentKey}' was found.");

        _data[currentKey] = scalarNode.Value ?? string.Empty;
        ExitContext();
    }

    private void VisitYamlMappingNode(string context, YamlMappingNode mappingNode)
    {
        EnterContext(context);

        foreach (var nodePair in mappingNode.Children)
        {
            var innerContext = ((YamlScalarNode)nodePair.Key).Value ?? string.Empty;
            VisitYamlNode(innerContext, nodePair.Value);
        }

        ExitContext();
    }

    private void VisitYamlSequenceNode(string context, YamlSequenceNode sequenceNode)
    {
        EnterContext(context);

        for (int i = 0; i < sequenceNode.Children.Count; ++i)
            VisitYamlNode(i.ToString(), sequenceNode.Children[i]);

        ExitContext();
    }

    private void EnterContext(string context)
    {
        _context.Push(context);
        _currentPath = ConfigurationPath.Combine(_context.Reverse());
    }

    private void ExitContext()
    {
        _context.Pop();
        _currentPath = ConfigurationPath.Combine(_context.Reverse());
    }
}