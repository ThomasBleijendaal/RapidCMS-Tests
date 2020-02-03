using RapidCMS.Core.Abstractions.Data;

namespace RapidCMSTests.Extensions
{
    public static class ParentExtensions
    {
        public static IParent? GetRootParent(this IParent? parent)
        {
            return parent?.Parent?.GetRootParent() ?? parent;
        }
    }
}
