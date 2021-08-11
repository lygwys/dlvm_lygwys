﻿using Mzg.Business.FormStateRule.Domain;
using Mzg.Core.Components.Platform;
using Mzg.Core.Data;
using Mzg.Infrastructure.Utility;
using Mzg.Sdk.Extensions;

namespace Mzg.Business.FormStateRule
{
    public static class SystemFormExtensions
    {
        public static bool IsDisabled(this SystemFormStateRule form, Entity record = null)
        {
            bool disabled = false;
            if (form.CommandRules.IsNotEmpty())
            {
                var rules = new FormCommandDefinition();
                rules = rules.DeserializeFromJson(form.CommandRules);
                if (!disabled && record != null && rules.Values.NotEmpty())
                {
                    foreach (var item in rules.Values)
                    {
                        var flag = (item.Value.IsCaseInsensitiveEqual("null") && record[item.Field] == null)
                            || (item.Value.IsCaseInsensitiveEqual(record.GetStringValue(item.Field)));
                        if (flag && !rules.Enabled)//符合条件并且规则为禁用
                        {
                            disabled = true;
                        }
                        if (disabled)
                        {
                            break;
                        }
                    }
                }
            }
            return disabled;
        }
    }

    public class FormCommandDefinition
    {
        public bool Enabled { get; set; }

        public ValueRule[] Values { get; set; }
    }
}