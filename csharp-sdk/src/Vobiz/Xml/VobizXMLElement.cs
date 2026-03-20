using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Vobiz.Xml
{
    public abstract class VobizXMLElement
    {
        private readonly HashSet<string> _nestable;
        private readonly List<VobizXMLElement> _children = [];
        private readonly List<KeyValuePair<string, string>> _attributes = [];

        protected VobizXMLElement(string name, IEnumerable<string>? nestable = null, string? content = null)
        {
            Name = name;
            _nestable = nestable is null
                ? new HashSet<string>(StringComparer.Ordinal)
                : new HashSet<string>(nestable, StringComparer.Ordinal);
            Content = content;
        }

        public string Name { get; }

        protected string? Content { get; set; }

        protected IReadOnlyList<VobizXMLElement> Children => _children;

        public VobizXMLElement Add(VobizXMLElement element)
        {
            if (!_nestable.Contains(element.Name))
            {
                throw new VobizXmlException(
                    $"{element.Name} is not nestable in {Name} (allowed: {string.Join(", ", _nestable)})");
            }

            _children.Add(element);
            return this;
        }

        protected void SetAttribute(string key, object? value)
        {
            if (value is null)
            {
                return;
            }

            string mapped = value switch
            {
                bool boolValue => boolValue.ToString().ToLowerInvariant(),
                _ => value.ToString() ?? string.Empty,
            };

            _attributes.Add(new KeyValuePair<string, string>(key, mapped));
        }

        protected virtual void Validate()
        {
        }

        public virtual XElement ToXElement()
        {
            Validate();

            XElement element = new(Name);
            foreach (KeyValuePair<string, string> attribute in _attributes)
            {
                element.SetAttributeValue(attribute.Key, attribute.Value);
            }

            if (!string.IsNullOrEmpty(Content))
            {
                element.Value = Content;
            }

            foreach (VobizXMLElement child in _children)
            {
                element.Add(child.ToXElement());
            }

            return element;
        }

        public string ToString(bool pretty = true)
        {
            string xml = ToXElement().ToString(pretty ? SaveOptions.None : SaveOptions.DisableFormatting);
            return xml.Replace("<cont>", " ").Replace("</cont>", " ");
        }
    }
}