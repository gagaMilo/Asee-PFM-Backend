using CsvHelper.Configuration.Attributes;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace PersonalFinanceManagementAPI.Models
{
    public enum Error
    {
        [Description("min-length")]
        minLength,
        [Description("max-length")]
        maxLength, 
        required,
        [Description("out-of-range")]
        outOfRange,
        [Description("invalid-format")]
        invalidFormat,
        [Description("unknown-enum")]
        unknownEnum,
        [Description("not-on-list")]
        notOnList,
        [Description("check-digit-invalid")]
        checkDigitInvalid,
        [Description("combination-required")]
        combinationRequired,
        [Description("read-only ")]
        readOnly 
    }
}
