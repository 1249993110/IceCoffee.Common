using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IceCoffee.Common.Templates
{
    /// <summary>
    /// Configuration options for <see cref="StringTemplate"/>
    /// </summary>
    public class StringTemplateConfiguration
    {
        public readonly static StringTemplateConfiguration Default = new StringTemplateConfiguration();

        /// <summary>
        /// The Open token (default "{")
        /// </summary>
        public string OpenToken { get; set; } = "{";
        /// <summary>
        /// The Close token (default "}")
        /// </summary>
        public string CloseToken { get; set; } = "}";
        /// <summary>
        /// The Foreach token (default "foreach")
        /// </summary>
        public string ForeachToken { get; set; } = "foreach";
        /// <summary>
        /// The If token (default "if")
        /// </summary>
        public string IfToken { get; set; } = "if";
    }

    /// <summary>
    /// Fluent configuration interface for <see cref="StringTemplate"/>'s <see cref="StringTemplateConfiguration"/>
    /// </summary>
    public class FluentStringTemplateConfiguration
    {
        private readonly StringTemplateConfiguration _cfg;
        /// <summary>
        /// Default constructor
        /// </summary>
        public FluentStringTemplateConfiguration() 
        {
            _cfg = StringTemplateConfiguration.Default;
        }

        /// <summary>
        /// Constructs a new <see cref="FluentStringTemplateConfiguration"/> instance using the supplied <see cref="StringTemplateConfiguration"/>
        /// </summary>
        /// <param name="cfg">The configuration</param>
        public FluentStringTemplateConfiguration(StringTemplateConfiguration cfg)
        {
            _cfg = cfg;
        }
        /// <summary>
        /// Sets the Open Token <see cref="StringTemplateConfiguration.OpenToken"/>
        /// </summary>
        /// <param name="openToken">The Open Token</param>
        /// <returns></returns>
        public FluentStringTemplateConfiguration OpenToken(string openToken)
        {
            _cfg.OpenToken = openToken;
            return this;
        }
        /// <summary>
        /// Sets the Close Token
        /// </summary>
        /// <param name="closeToken">The Close Token <see cref="StringTemplateConfiguration.CloseToken"/></param>
        /// <returns></returns>
        public FluentStringTemplateConfiguration CloseToken(string closeToken)
        {
            _cfg.CloseToken = closeToken;
            return this;
        }
        /// <summary>
        /// Sets the Foreach Token <see cref="StringTemplateConfiguration.ForeachToken"/>
        /// </summary>
        /// <param name="foreachToken">The Foreach Token</param>
        /// <returns></returns>
        public FluentStringTemplateConfiguration ForeachToken(string foreachToken)
        {
            _cfg.ForeachToken = foreachToken;
            return this;
        }
        /// <summary>
        /// Sets te If Token <see cref="StringTemplateConfiguration.IfToken"/>
        /// </summary>
        /// <param name="ifToken">The If Token</param>
        /// <returns></returns>
        public FluentStringTemplateConfiguration IfToken(string ifToken)
        {
            _cfg.IfToken = ifToken;
            return this;
        }
        /// <summary>
        /// Exposes the internal <see cref="StringTemplateConfiguration"/>
        /// </summary>
        /// <returns></returns>
        public StringTemplateConfiguration ExposeConfiguration()
        {
            return _cfg;
        }
    }
}
