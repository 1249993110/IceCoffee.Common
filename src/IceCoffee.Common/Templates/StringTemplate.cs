﻿using System.Collections;
using System.Dynamic;
using System.Reflection;
using System.Text.RegularExpressions;
#pragma warning disable CS8602,CS8604 // 解引用可能出现空引用。

namespace IceCoffee.Common.Templates
{
    /// <summary>
    /// Lightweight string templating class
    /// </summary>
    public static class StringTemplate
    {
        private static readonly StringTemplateConfiguration _cfg = StringTemplateConfiguration.Default;
        /// <summary>
        /// The global <see cref="FluentStringTemplateConfiguration"/>
        /// </summary>
        public static FluentStringTemplateConfiguration Configure { get; private set; } = new FluentStringTemplateConfiguration(_cfg);
        /// <summary>
        /// Renders a string template using the supplied object
        /// </summary>
        /// <param name="template">the template</param>
        /// <param name="obj">any POCO</param>
        /// <returns></returns>
        public static string Render(string template, object obj) => ReplaceText(template, BuildPropertyDictionary(obj), _cfg);
        /// <summary>
        /// Renders a string template using the supplied object
        /// </summary>
        /// <param name="template">the template</param>
        /// <param name="obj">any POCO</param>
        /// <param name="cfg">override configuration</param>
        /// <returns></returns>
        public static string Render(string template, object obj, StringTemplateConfiguration cfg) => ReplaceText(template, BuildPropertyDictionary(obj), cfg);
        /// <summary>
        /// Renders a string template using the supplied object
        /// </summary>
        /// <param name="template">the template</param>
        /// <param name="obj">any POCO</param>
        /// <param name="replacements">additional dictionary of replacement values</param>
        /// <returns></returns>
        public static string Render(string template, object obj, Dictionary<string, object> replacements) => ReplaceText(template, BuildPropertyDictionary(obj).Union(replacements).ToDictionary(x => x.Key, x => x.Value), _cfg);
        /// <summary>
        /// Renders a string template using the supplied object
        /// </summary>
        /// <param name="template">the template</param>
        /// <param name="obj">any POCO</param>
        /// <param name="replacements">additional dictionary of replacement values</param>
        /// <param name="cfg">override configuration</param>
        /// <returns></returns>
        public static string Render(string template, object obj, Dictionary<string, object> replacements, StringTemplateConfiguration cfg)
            => ReplaceText(template, BuildPropertyDictionary(obj).Union(replacements).ToDictionary(x => x.Key, x => x.Value), cfg);
        /// <summary>
        /// Renders a string template using the supplied object
        /// </summary>
        /// <param name="template">the template</param>
        /// <param name="replacements">dictionary of replacement values</param>
        /// <returns></returns>
        public static string Render(string template, Dictionary<string, object> replacements) => ReplaceText(template, replacements, _cfg);
        /// <summary>
        /// Renders a string template using the supplied object
        /// </summary>
        /// <param name="template">the template</param>
        /// <param name="replacements">dictionary of replacement values</param>
        /// <param name="cfg">override configuration</param>
        /// <returns></returns>
        public static string Render(string template, Dictionary<string, object> replacements, StringTemplateConfiguration cfg) => ReplaceText(template, replacements, cfg);
        /// <summary>
        /// Builds a property dictionary of key:value from the object instance
        /// </summary>
        /// <param name="obj">the object instance</param>
        /// <returns></returns>
        public static Dictionary<string, object> BuildPropertyDictionary(object obj)
        {
            if (obj is IDynamicMetaObjectProvider)
            {
                throw new NotSupportedException("The dynamic type is not supported.");
            }

            string prefix(string p) => string.IsNullOrEmpty(p) ? "" : $"{p}.";

            IEnumerable<KeyValuePair<string, object>> CollectProperties(string pre, object o) =>
                o.GetType().GetTypeInfo().GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(p => p.GetMethod?.IsPublic ?? false)
                    .SelectMany(prop => new[] { new KeyValuePair<string, object>($"{prefix(pre)}{prop.Name}", prop.GetValue(o)) }
                    .Concat((prop.PropertyType.GetTypeInfo().IsClass && prop.PropertyType != typeof(string) && !typeof(IEnumerable).GetTypeInfo().IsAssignableFrom(prop.PropertyType.GetTypeInfo())) ?
                        CollectProperties($"{prefix(pre)}{prop.Name}", prop.GetValue(o))
                        .Select(kvp => new KeyValuePair<string, object>($"{prefix(pre)}{kvp.Key}", kvp.Value)) : new KeyValuePair<string, object>[0]));

            return CollectProperties(string.Empty, obj).ToDictionary(x => x.Key, x => x.Value);
        }
        
        /// <summary>
        /// This performs all of the token replacements and recursion
        /// </summary>
        /// <param name="text">The snippet to process for the supplied replacements</param>
        /// <param name="replacements">The replacements</param>
        /// <param name="cfg">The configuration</param>
        /// <returns></returns>
        internal static string ReplaceText(string text, Dictionary<string, object> replacements, StringTemplateConfiguration cfg) =>
            replacements.ToList().OrderBy((kvp) => (kvp.Value is IEnumerable && kvp.Value.GetType() != typeof(string)) ? 1 : 2).Aggregate(text, (c, k) =>
                (k.Value is IEnumerable enumerable && !(k.Value is string) && c.IndexOf($"{cfg.OpenToken}{cfg.ForeachToken} {k.Key}{cfg.CloseToken}") >= 0 && c.IndexOf($"{cfg.OpenToken}/{cfg.ForeachToken} {k.Key}{cfg.CloseToken}") > 0) ?
                    new Regex(string.Format(
                            @"{0}(?<inner>(?>{0}(?<LEVEL>)|{1}(?<-LEVEL>)|(?!{0}|{1}).)+(?(LEVEL)(?!))){1}",
                            string.Join("", $@"{cfg.OpenToken}{cfg.ForeachToken} {k.Key}{cfg.CloseToken}".ToCharArray().Select(ch => $"\\u{((int)ch).ToString("X4")}")),
                            string.Join("", $@"{cfg.OpenToken}/{cfg.ForeachToken} {k.Key}{cfg.CloseToken}".ToCharArray().Select(ch => $"\\u{((int)ch).ToString("X4")}"))
                            ),
                        RegexOptions.IgnorePatternWhitespace | RegexOptions.Singleline)
                    .Matches(text).Cast<Match>().Aggregate(c, (prev, match) => prev.Replace(match.Captures[0].Value,
                        string.Join("", enumerable.Cast<object>().Select(item => ReplaceText(match.Groups[1].Value, BuildPropertyDictionary(item), cfg)))))
                :
                ReplaceToken(c, k.Key, k.Value, cfg)
            );

        internal static string ReplaceToken(string original, string key, object value, StringTemplateConfiguration cfg)
        {
            var typeInfo = value?.GetType().GetTypeInfo();
            var toStringMethod = (typeInfo?.IsEnum ?? false ? typeInfo?.BaseType.GetTypeInfo() : typeInfo)?
                .GetDeclaredMethods("ToString")
                .Where(p =>
                    p.GetParameters().Select(q => q.ParameterType).SequenceEqual(new Type[] { typeof(string) })
                ).FirstOrDefault();

            original = (value is bool condition && original.IndexOf($"{cfg.OpenToken}{cfg.IfToken} {key}{cfg.CloseToken}") >= 0 && original.IndexOf($"{cfg.OpenToken}/{cfg.IfToken} {key}{cfg.CloseToken}") > 0) ?
                new Regex(string.Format(
                            @"{0}(?<inner>(?>{0}(?<LEVEL>)|{1}(?<-LEVEL>)|(?!{0}|{1}).)+(?(LEVEL)(?!))){1}",
                            string.Join("", $@"{cfg.OpenToken}{cfg.IfToken} {key}{cfg.CloseToken}".ToCharArray().Select(ch => $"\\u{((int)ch).ToString("X4")}")),
                            string.Join("", $@"{cfg.OpenToken}/{cfg.IfToken} {key}{cfg.CloseToken}".ToCharArray().Select(ch => $"\\u{((int)ch).ToString("X4")}"))
                            ),
                        RegexOptions.IgnorePatternWhitespace | RegexOptions.Singleline)
                    .Matches(original).Cast<Match>().Aggregate(original, (prev, match) => prev.Replace(match.Captures[0].Value, condition ? match.Groups[1].Value : string.Empty)) : original;

            return Regex.Matches((original = original.Replace($"{cfg.OpenToken}{key}{cfg.CloseToken}", value?.ToString() ?? string.Empty))
                    , $@"{cfg.OpenToken}(?<key>{key})(,(?<pad>-*?\d+))*?(:(?<fmt>[^}}]+))*?{cfg.CloseToken}")
                .Cast<Match>()
                .Aggregate(original, (s, match) =>
                {
                    var v = toStringMethod == null ? value?.ToString() : toStringMethod.Invoke(value, new[] { match.Groups["fmt"]?.Value ?? string.Empty }) as string;
                    if (int.TryParse(match.Groups["pad"]?.Value ?? string.Empty, out int padding))
                    {
                        v = padding < 0 ? v.PadRight(Math.Abs(padding)) : v.PadLeft(Math.Abs(padding));
                    }
                    return s.Replace(match.Value, v);
                });
        }
    }
}

#pragma warning restore CS8602,CS8604 // 解引用可能出现空引用。

