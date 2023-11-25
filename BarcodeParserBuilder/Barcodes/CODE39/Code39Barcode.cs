﻿using System;
using System.Collections.Generic;
using System.Text;
using BarcodeParserBuilder.Exceptions;
using BarcodeParserBuilder.Exceptions.CODE39;
using BarcodeParserBuilder.Infrastructure;

namespace BarcodeParserBuilder.Barcodes.CODE39;

public class Code39Barcode : Barcode
{
    
    public Code39Barcode() : base() { }

    public Code39Barcode(Code39ReaderModifier readerModifier) : base(readerModifier) { }

    public Code39Barcode(string readerModifierValue)
    {

    }

    public override ProductCode? ProductCode
    {
        get => (ProductCode?)BarcodeFields[nameof(ProductCode)].Value;
        set => BarcodeFields[nameof(ProductCode)].SetValue(value);
    }

    protected override FieldCollection BarcodeFields { get; } = new()
    {
        new BarcodeField<ProductCode>(BarcodeType.CODE39, nameof(ProductCode), 2, 55)
    };

    public override BarcodeType BarcodeType => BarcodeType.CODE39;
    

    public override BarcodeDateTime? ExpirationDate
    {
        get => throw new UnusedFieldException(nameof(ExpirationDate));
        set => throw new UnusedFieldException(nameof(ExpirationDate));
    }

    public override BarcodeDateTime? ProductionDate
    {
        get => throw new UnusedFieldException(nameof(ProductionDate));
        set => throw new UnusedFieldException(nameof(ProductionDate));
    }

    public override string? BatchNumber
    {
        get => throw new UnusedFieldException(nameof(BatchNumber));
        set => throw new UnusedFieldException(nameof(BatchNumber));
    }

    public override string? SerialNumber
    {
        get => throw new UnusedFieldException(nameof(SerialNumber));
        set => throw new UnusedFieldException(nameof(SerialNumber));
    }


    public static Code39ReaderModifier ParseReaderModifier(string readerModifierValue)
    {
        if (Code39ReaderModifier.GetAllValues<Code39ReaderModifier>().Contains(readerModifierValue))
        {
            return new Code39ReaderModifier(readerModifierValue);
        }

        throw new Code39ParseException("Invalid reader modifier");
    }

    public static string StripCheckCharacter(string inputString, Code39ReaderModifier readerModifier)
    {
        if (String.IsNullOrEmpty(inputString) || inputString!.Length < 2)
            return inputString;

        switch (readerModifier.Value)
        {
            case Code39ReaderModifier.NoFullASCIIMod43ChecksumTransmittedValue:
            case Code39ReaderModifier.FullASCIIMod43ChecksumTransmittedValue:
                return inputString[0..^1].ToString();
            default:
                return inputString;
        }
    }
}
