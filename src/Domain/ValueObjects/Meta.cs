using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.ValueObjects
{
    public class Meta : ValueObject
    {
        static Meta()
        {
        }

        private Meta()
        {
        }

        public Meta(string? title, string? icon, string? active, string? color, MetaType? type, bool? fullpage, bool? hidden, bool? hiddenBreadcrumb, string? tag)
        {
            Title = title;
            Icon = icon;
            Active = active;
            Color = color;
            Type = type;
            Fullpage = fullpage;
            Hidden = hidden;
            HiddenBreadcrumb = hiddenBreadcrumb;
            Tag = tag;
        }

        public string? Title { get; private set; }
        public string? Icon { get; private set; }
        public string? Active { get; private set; }
        public string? Color { get; private set; }
        public MetaType? Type { get; private set; }
        public bool? Fullpage { get; private set; }
        public bool? Hidden { get; private set; }
        public bool? HiddenBreadcrumb { get; private set; }
        public string? Tag { get; private set; }

        public static implicit operator string(Meta  meta)
        {
            return meta.ToString()!;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Title!;
            yield return Icon!;
            yield return Active!;
            yield return Color!;
            yield return Type!;
            yield return Fullpage!;
            yield return Hidden!;
            yield return HiddenBreadcrumb!;
            yield return Tag!;
        }
    }
}
