﻿using BarcodeParserBuilder.Exceptions.EAN;
using BarcodeParserBuilder.Infrastructure;
using System.Linq;

namespace BarcodeParserBuilder.Barcodes.EAN
{
    internal class EanProductCodeParserBuilder : BaseFieldParserBuilder<ProductCode?>
    {
        protected override ProductCode? Parse(string? value) => ProductCode.ParseEan(value);
        protected override string? Build(ProductCode? obj) => string.IsNullOrWhiteSpace(obj?.Code) ? null : obj.Code;

        protected override bool Validate(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return true;

            if (!value.All(char.IsDigit) || (value.Length < 6 || value.Length > 13))
                throw new EanValidateException($"Invalid Ean value '{value}'.");

            return true;
        }

        protected override bool ValidateObject(ProductCode? obj)
        {
            if (obj == null)
                return true;

            switch(obj.Type)
            {
                case ProductCodeType.EAN:
                case ProductCodeType.NDC:
                    break;
                default:
                    throw new EanValidateException($"Invalid ProductCode type '{obj.Type}'.");
            }                

            return true;
        }
    }
}
